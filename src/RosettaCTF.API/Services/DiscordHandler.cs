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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using RosettaCTF.Data;
using RosettaCTF.Filters;
using RosettaCTF.Models;

namespace RosettaCTF.Services
{
    public sealed class DiscordHandler
    {
        private const string EndpointHostname = "discord.com";
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

        private static JsonSerializerOptions JsonOptionsCamelCase { get; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static JsonSerializerOptions JsonOptionsSnakeCase { get; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy()
        };

        private HttpClient Http { get; }
        private RosettaConfigurationDiscord Configuration { get; }

        public DiscordHandler(
            HttpClient http,
            IOptions<RosettaConfigurationDiscord> discordCfg)
        {
            this.Http = http;
            this.Configuration = discordCfg.Value;
        }

        public string GetRedirectUrl(HttpContext ctx)
            => new UriBuilder
            {
                Scheme = ctx.Request.Scheme,
                Host = this.Configuration.Hostname,
                Port = this.Configuration.Port,
                Path = "/session/callback"
            }.Uri.ToString();

        public string GetAuthenticationUrl(HttpContext ctx)
            => QueryHelpers.AddQueryString(this.GetDiscordUrl(EndpointAuthorize).ToString(), new Dictionary<string, string>(5)
            {
                ["client_id"] = this.Configuration.ClientId.AsString(),
                ["redirect_uri"] = this.GetRedirectUrl(ctx),
                ["response_type"] = ResponseType,
                ["scope"] = Scopes,
                ["state"] = Guid.NewGuid().ToString() // TODO: STATE
            });

        public async Task<OAuthAuthenticationResponse> CompleteLoginAsync(HttpContext ctx, OAuthAuthenticationData authData, CancellationToken cancellationToken = default)
            => await this.ExchangeTokenAsync(ctx, authData.Code, GrantTypeToken, cancellationToken);

        public async Task<OAuthAuthenticationResponse> RefreshTokenAsync(HttpContext ctx, string refreshToken, CancellationToken cancellationToken = default)
            => await this.ExchangeTokenAsync(ctx, refreshToken, GrantTypeRefresh, cancellationToken);

        public async Task<bool> LogoutAsync(HttpContext ctx, string token, CancellationToken cancellationToken = default)
        {
            using (var post = new HttpRequestMessage(HttpMethod.Post, this.GetDiscordUrl(EndpointTokenRevoke)))
            {
                post.Content = new FormUrlEncodedContent(new Dictionary<string, string>(2)
                {
                    ["token"] = token,
                    ["token_type_hint"] = RevokeTypeHint
                });

                post.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                    AbstractionUtilities.UTF8.GetBytes($"{this.Configuration.ClientId}:{this.Configuration.Secret}")));

                using (var res = await this.Http.SendAsync(post, cancellationToken))
                    return res.IsSuccessStatusCode;
            }
        }

        public async Task<DiscordUserModel> GetUserAsync(HttpContext ctx, string token, CancellationToken cancellationToken = default)
        {
            DiscordUserModel user;
            using (var get = new HttpRequestMessage(HttpMethod.Get, this.GetDiscordUrl(EndpointCurrentUser)))
            {
                this.AddToken(get, token);

                using (var res = await this.Http.SendAsync(get, cancellationToken))
                {
                    if (!res.IsSuccessStatusCode)
                        return null;

                    using (var dat = await res.Content.ReadAsStreamAsync())
                        user = await JsonSerializer.DeserializeAsync<DiscordUserModel>(dat, JsonOptionsCamelCase, cancellationToken);
                }
            }

            using (var get = new HttpRequestMessage(HttpMethod.Get, this.GetDiscordUrl(EndpointGuilds)))
            {
                this.AddToken(get, token);

                using (var res = await this.Http.SendAsync(get, cancellationToken))
                {
                    if (!res.IsSuccessStatusCode)
                        return null;

                    IEnumerable<DiscordGuildModel> guilds;
                    using (var dat = await res.Content.ReadAsStreamAsync())
                        guilds = await JsonSerializer.DeserializeAsync<IEnumerable<DiscordGuildModel>>(dat, JsonOptionsCamelCase, cancellationToken);

                    user.IsAuthorized = guilds.Any(x => x.Id == this.Configuration.GuildId);
                }
            }

            return user;
        }

        private async Task<OAuthAuthenticationResponse> ExchangeTokenAsync(HttpContext ctx, string credential, string grantType, CancellationToken cancellationToken = default)
        {
            using (var post = new HttpRequestMessage(HttpMethod.Post, this.GetDiscordUrl(EndpointTokenExchange)))
            {
                var postData = new Dictionary<string, string>(6)
                {
                    ["client_id"] = this.Configuration.ClientId.AsString(),
                    ["client_secret"] = this.Configuration.Secret,
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
                        return await JsonSerializer.DeserializeAsync<OAuthAuthenticationResponse>(dat, JsonOptionsSnakeCase, cancellationToken);
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
