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
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Emzi0767.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF.Services
{
    /// <summary>
    /// Handles issuing and validation of JWTs.
    /// </summary>
    public sealed class JwtHandler
    {
        private ConfigurationAuthentication Configuration { get; }

        private TokenValidationParameters JwtValidationParameters { get; }
        private SymmetricSecurityKey Key { get; }
        private SigningCredentials Credentials { get; }

        /// <summary>
        /// Creates a new handler.
        /// </summary>
        /// <param name="cfg">Configuration to use.</param>
        public JwtHandler(
            IOptions<ConfigurationAuthentication> cfg,
            IKeyDeriver keyDeriver)
        {
            this.Configuration = cfg.Value;

            var keyBase = AbstractionUtilities.UTF8.GetBytes(this.Configuration.TokenKey);
            if (keyBase.Length < 8)
                throw new ArgumentException("Token key must be at least 8 bytes-long.", nameof(cfg));

            var saltBase = MemoryMarshal.Read<long>(keyBase);
            saltBase *= 13;
            var saltBytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref saltBase, 1));

            var async = new AsyncExecutor();
            this.Key = new SymmetricSecurityKey(
                async.Execute(
                    keyDeriver.DeriveKeyAsync(
                        value: keyBase, 
                        salt: saltBytes.ToArray(), 
                        byteCount: 512 / 8)));

            this.Credentials = new SigningCredentials(this.Key, SecurityAlgorithms.HmacSha512);
            this.JwtValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = this.Configuration.TokenIssuer,
                ValidAudience = this.Configuration.TokenIssuer,
                IssuerSigningKey = this.Key,
                ClockSkew = TimeSpan.FromSeconds(1)
            };
        }

        /// <summary>
        /// Issues a new token for given user.
        /// </summary>
        /// <param name="user">User to issue token for.</param>
        /// <returns>The issued token and its expiration time.</returns>
        public JwtTokenModel IssueToken(UserPreview user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.NameId, user.Username)
            };

            var expires = DateTime.UtcNow.AddHours(2);
            var token = new JwtSecurityToken(
                this.Configuration.TokenIssuer,
                this.Configuration.TokenIssuer,
                claims,
                expires: expires,
                signingCredentials: this.Credentials);

            return new JwtTokenModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expires
            };
        }

        /// <summary>
        /// Validates given token and returns the user ID it's for.
        /// </summary>
        /// <param name="token">Token to validate and extract user ID from.</param>
        /// <returns>Extracted user ID, if the token validation was successful.</returns>
        public long? ValidateToken(string token)
        {
            var jwth = new JwtSecurityTokenHandler();
            try
            {
                _ = jwth.ValidateToken(token, this.JwtValidationParameters, out var stoken);
                var jwt = stoken as JwtSecurityToken;
                return jwt.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value.ParseAsLong();
            }
            catch (Exception) { }

            return null;
        }
    }
}
