// This file is part of RosettaCTF project.
//
// Copyright 2020 Emzi0767
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF.Authentication
{
    internal sealed class DiscordOAuthProvider : IOAuthProvider
    {
        internal const string ProviderType = "discord";

        private const string EndpointHostname = "discord.com";
        private const string EndpointHostnameOld = "discord.com";
        private const string EndpointAuthorize = "/api/v7/oauth2/authorize";
        private const string EndpointTokenExchange = "/api/v7/oauth2/token";
        private const string EndpointTokenRevoke = "/api/v7/oauth2/token/revoke";

        private const string EndpointCurrentUser = "/api/v7/users/@me";
        private const string EndpointGuilds = "/api/v7/users/@me/guilds";

        private const string Scopes = "guilds identify";
        private const string ResponseType = "code";
        private const string RevokeTypeHint = "access_token";
        private const string GrantTypeToken = "authorization_code";
        private const string GrantTypeRefresh = "refresh_token";

        private HttpClient Http { get; }
        private ConfigurationOAuthProvider Configuration { get; }

        public DiscordOAuthProvider(
            HttpClient http,
            OAuthConfigurationProvider cfgProvider)
        {
            this.Http = http;
            this.Configuration = cfgProvider.GetById(ProviderType);
        }

        public bool HasId(string id)
            => string.Equals(id, ProviderType, StringComparison.OrdinalIgnoreCase);

        public string GetName(string id)
            => string.Equals(id, ProviderType, StringComparison.OrdinalIgnoreCase)
                ? "Discord"
                : null;

        public bool SupportsReferrer(Uri referrer, out string id)
        {
            if (string.Equals(referrer.Host, EndpointHostname, StringComparison.OrdinalIgnoreCase) || string.Equals(referrer.Host, EndpointHostnameOld, StringComparison.OrdinalIgnoreCase))
            {
                id = ProviderType;
                return true;
            }

            id = null;
            return false;
        }

        public string GetRedirectUrl(AuthenticationContext ctx)
            => new UriBuilder
            {
                Scheme = ctx.CallbackUrl.Scheme,
                Host = ctx.CallbackUrl.Host,
                Port = ctx.CallbackUrl.Port,
                Path = "/session/callback"
            }.Uri.ToString();

        public Uri GetAuthenticationUrl(AuthenticationContext ctx)
            => new Uri(QueryHelpers.AddQueryString(this.GetDiscordUrl(EndpointAuthorize).ToString(), new Dictionary<string, string>(5)
            {
                ["client_id"] = this.Configuration.ClientId,
                ["redirect_uri"] = this.GetRedirectUrl(ctx),
                ["response_type"] = ResponseType,
                ["scope"] = Scopes,
                ["state"] = ctx.State
            }));

        public async Task<OAuthResult> CompleteLoginAsync(AuthenticationContext ctx, string code, CancellationToken cancellationToken = default)
            => await this.ExchangeTokenAsync(ctx, code, GrantTypeToken, cancellationToken);

        public async Task<OAuthResult> RefreshTokenAsync(AuthenticationContext ctx, string refreshToken, CancellationToken cancellationToken = default)
            => await this.ExchangeTokenAsync(ctx, refreshToken, GrantTypeRefresh, cancellationToken);

        public async Task<bool> LogoutAsync(AuthenticationContext ctx, string token, CancellationToken cancellationToken = default)
        {
            using (var post = new HttpRequestMessage(HttpMethod.Post, this.GetDiscordUrl(EndpointTokenRevoke)))
            {
                post.Content = new FormUrlEncodedContent(new Dictionary<string, string>(2)
                {
                    ["token"] = token,
                    ["token_type_hint"] = RevokeTypeHint
                });

                post.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                    AbstractionUtilities.UTF8.GetBytes($"{this.Configuration.ClientId}:{this.Configuration.ClientSecret}")));

                using (var res = await this.Http.SendAsync(post, cancellationToken))
                    return res.IsSuccessStatusCode;
            }
        }

        public async Task<OAuthUser> GetUserAsync(AuthenticationContext ctx, string token, CancellationToken cancellationToken = default)
        {
            DiscordUserModel duser;
            using (var get = new HttpRequestMessage(HttpMethod.Get, this.GetDiscordUrl(EndpointCurrentUser)))
            {
                this.AddToken(get, token);

                using (var res = await this.Http.SendAsync(get, cancellationToken))
                {
                    if (!res.IsSuccessStatusCode)
                        return null;

                    using (var dat = await res.Content.ReadAsStreamAsync())
                        duser = await JsonSerializer.DeserializeAsync<DiscordUserModel>(dat, AbstractionUtilities.DefaultJsonOptions, cancellationToken);
                }
            }

            OAuthUser user;
            using (var get = new HttpRequestMessage(HttpMethod.Get, this.GetDiscordUrl(EndpointGuilds)))
            {
                this.AddToken(get, token);

                using (var res = await this.Http.SendAsync(get, cancellationToken))
                {
                    if (!res.IsSuccessStatusCode)
                        return null;

                    IEnumerable<DiscordGuildModel> guilds;
                    using (var dat = await res.Content.ReadAsStreamAsync())
                        guilds = await JsonSerializer.DeserializeAsync<IEnumerable<DiscordGuildModel>>(dat, AbstractionUtilities.DefaultJsonOptions, cancellationToken);

                    var authorized = guilds.Select(x => x.Id.ParseAsUlong())
                        .Intersect(this.Configuration.AuthorizedGuilds)
                        .Any();

                    user = new OAuthUser(duser.Id, duser.Username, authorized);
                }
            }

            return user;
        }

        private async Task<OAuthResult> ExchangeTokenAsync(AuthenticationContext ctx, string credential, string grantType, CancellationToken cancellationToken = default)
        {
            using (var post = new HttpRequestMessage(HttpMethod.Post, this.GetDiscordUrl(EndpointTokenExchange)))
            {
                var postData = new Dictionary<string, string>(6)
                {
                    ["client_id"] = this.Configuration.ClientId,
                    ["client_secret"] = this.Configuration.ClientSecret,
                    ["grant_type"] = grantType,
                    ["redirect_uri"] = this.GetRedirectUrl(ctx),
                    ["scope"] = Scopes
                };

                switch (grantType)
                {
                    case GrantTypeToken:
                        postData["code"] = credential;
                        break;

                    case GrantTypeRefresh:
                        postData["refresh_token"] = credential;
                        break;
                }

                post.Content = new FormUrlEncodedContent(postData);

                using (var res = await this.Http.SendAsync(post, cancellationToken))
                {
                    if (!res.IsSuccessStatusCode)
                        return null;

                    using (var dat = await res.Content.ReadAsStreamAsync())
                    {
                        var result = await JsonSerializer.DeserializeAsync<OAuthResult>(dat, AbstractionUtilities.SnakeCaseJsonOptions, cancellationToken);
                        result.IsSuccess = true;
                        return result;
                    }
                }
            }
        }

        private Uri GetDiscordUrl(string endpoint)
            => new UriBuilder
            {
                Scheme = "https",
                Host = EndpointHostname,
                Path = endpoint
            }.Uri;

        private void AddToken(HttpRequestMessage req, string token)
            => req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
