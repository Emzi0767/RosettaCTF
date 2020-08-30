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
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.Utilities;
using Microsoft.AspNetCore.WebUtilities;
using RosettaCTF.Authentication;
using RosettaCTF.Data;

namespace RosettaCTF
{
    internal sealed class CustomOAuthProvider : IOAuthProvider
    {
        internal const string ProviderType = "custom";

        private const string ResponseType = "code";
        private const string RevokeTypeHint = "access_token";
        private const string GrantTypeToken = "authorization_code";
        private const string GrantTypeRefresh = "refresh_token";

        private HttpClient Http { get; }
        private IReadOnlyDictionary<string, ConfigurationOAuthProvider> Configuration { get; }

        public CustomOAuthProvider(
            HttpClient http,
            OAuthConfigurationProvider cfgProvider)
        {
            this.Http = http;
            this.Configuration = cfgProvider.GetByType(ProviderType)
                .ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

            if (!this.ValidateConfiguration())
                throw new InvalidConfigurationException("Supplied custom OAuth provider configuration was invalid, one of the key properties is missing.");
        }

        public bool HasId(string id)
            => this.Configuration.ContainsKey(id);

        public bool SupportsReferrer(Uri referrer, out string id)
        {
            id = this.Configuration.SingleOrDefault(x => x.Value.Hostnames.Contains(referrer.Host, StringComparer.OrdinalIgnoreCase)).Key;
            return id != null;
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
        {
            var cfg = this.GetConfiguration(ctx.ProviderId);
            return new Uri(QueryHelpers.AddQueryString(cfg.AuthorizeUrl, new Dictionary<string, string>(5)
            {
                ["client_id"] = cfg.ClientId,
                ["redirect_uri"] = this.GetRedirectUrl(ctx),
                ["response_type"] = ResponseType,
                ["scope"] = string.Join(" ", cfg.Scopes),
                ["state"] = ctx.State
            }));
        }

        public async Task<OAuthResult> CompleteLoginAsync(AuthenticationContext ctx, string code, CancellationToken cancellationToken = default)
        {
            var cfg = this.GetConfiguration(ctx.ProviderId);
            return await this.ExchangeTokenAsync(ctx, code, GrantTypeToken, cfg, cfg.TokenUrl, cancellationToken);
        }

        public async Task<OAuthResult> RefreshTokenAsync(AuthenticationContext ctx, string refreshToken, CancellationToken cancellationToken = default)
        {
            var cfg = this.GetConfiguration(ctx.ProviderId);
            if (cfg.RefreshUrl == null)
                return null;

            return await this.ExchangeTokenAsync(ctx, refreshToken, GrantTypeRefresh, cfg, cfg.RefreshUrl, cancellationToken);
        }

        public async Task<bool> LogoutAsync(AuthenticationContext ctx, string token, CancellationToken cancellationToken = default)
        {
            var cfg = this.GetConfiguration(ctx.ProviderId);
            if (cfg.RevokeUrl == null)
                return false;

            using (var post = new HttpRequestMessage(HttpMethod.Post, cfg.RevokeUrl))
            {
                post.Content = new FormUrlEncodedContent(new Dictionary<string, string>(2)
                {
                    ["token"] = token,
                    ["token_type_hint"] = RevokeTypeHint
                });

                post.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                    AbstractionUtilities.UTF8.GetBytes($"{cfg.ClientId}:{cfg.ClientSecret}")));

                using (var res = await this.Http.SendAsync(post, cancellationToken))
                    return res.IsSuccessStatusCode;
            }
        }

        public async Task<OAuthUser> GetUserAsync(AuthenticationContext ctx, string token, CancellationToken cancellationToken = default)
        {
            var cfg = this.GetConfiguration(ctx.ProviderId);
            var map = cfg.Mappings;

            using (var get = await this.Http.GetAsync(cfg.UserUrl, cancellationToken))
            using (var res = await get.Content.ReadAsStreamAsync())
            {
                var ruser = await JsonSerializer.DeserializeAsync<IReadOnlyDictionary<string, object>>(res, AbstractionUtilities.DefaultJsonOptions, cancellationToken);

                var id = ruser[map.UserId].ToString();
                var un = ruser[map.Username].ToString();

                return new OAuthUser(id, un, true);
            }
        }

        private async Task<OAuthResult> ExchangeTokenAsync(AuthenticationContext ctx, string credential, string grantType, ConfigurationOAuthProvider cfg, string url, CancellationToken cancellationToken = default)
        {
            using (var post = new HttpRequestMessage(HttpMethod.Post, url))
            {
                var postData = new Dictionary<string, string>(6)
                {
                    ["client_id"] = cfg.ClientId,
                    ["client_secret"] = cfg.ClientSecret,
                    ["grant_type"] = grantType,
                    ["redirect_uri"] = this.GetRedirectUrl(ctx),
                    ["scope"] = string.Join(" ", cfg.Scopes)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ValidateConfiguration()
            => this.Configuration.Values.All(IsValidConfigurationItem);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ConfigurationOAuthProvider GetConfiguration(string id)
        {
            if (!this.Configuration.TryGetValue(id, out var cfg))
                throw new InvalidOperationException("This provider is not supported by this implementation.");

            return cfg;
        }

        private void AddToken(HttpRequestMessage req, string token)
            => req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        private static bool IsValidConfigurationItem(ConfigurationOAuthProvider cfg)
            => cfg.Id != null
            && cfg.ClientId != null
            && cfg.ClientSecret != null
            && cfg.Scopes?.Any() == true
            && cfg.AuthorizeUrl != null
            && cfg.TokenUrl != null
            && cfg.UserUrl != null
            && cfg.Mappings != null;
    }
}
