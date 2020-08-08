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
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Emzi0767.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RosettaCTF.Data;

namespace RosettaCTF.Services
{
    /// <summary>
    /// Handles issuing and validation of JWTs.
    /// </summary>
    public sealed class JwtHandler
    {
        private RosettaConfigurationTokens Configuration { get; }

        private TokenValidationParameters JwtValidationParameters { get; }
        private SymmetricSecurityKey Key { get; }
        private SigningCredentials Credentials { get; }

        /// <summary>
        /// Creates a new handler.
        /// </summary>
        /// <param name="cfg">Configuration to use.</param>
        public JwtHandler(
            IOptions<RosettaConfigurationTokens> cfg,
            Argon2idKeyDeriver keyDeriver)
        {
            this.Configuration = cfg.Value;

            var async = new AsyncExecutor();
            this.Key = new SymmetricSecurityKey(
                async.Execute(
                    keyDeriver.DeriveKeyAsync(
                        value: AbstractionUtilities.UTF8.GetBytes(this.Configuration.Key), 
                        salt: null, 
                        byteCount: 512 / 8)));

            this.Credentials = new SigningCredentials(this.Key, SecurityAlgorithms.HmacSha512);
            this.JwtValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = this.Configuration.Issuer,
                ValidAudience = this.Configuration.Issuer,
                IssuerSigningKey = this.Key
            };
        }

        public string IssueToken(UserPreview user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(JwtRegisteredClaimNames.NameId, user.Username)
            };

            var token = new JwtSecurityToken(
                this.Configuration.Issuer,
                this.Configuration.Issuer,
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: this.Credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

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
