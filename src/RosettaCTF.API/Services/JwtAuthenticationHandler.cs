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

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RosettaCTF.Data;

namespace RosettaCTF.Services
{
    internal sealed class JwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationOptions>
    {
        private JwtHandler Jwt { get; }
        private IUserRepository Users { get; }

        public JwtAuthenticationHandler(
            JwtHandler jwt,
            IUserRepository users,
            IOptionsMonitor<JwtAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder urlEncoder,
            ISystemClock systemClock)
            : base(options, logger, urlEncoder, systemClock)
        {
            this.Jwt = jwt;
            this.Users = users;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!this.Request.Headers.TryGetValue("Authorization", out var values) || values.Count == 0)
                return AuthenticateResult.Fail("Missing credentials.");

            var tokenStr = values.First();
            var uid = this.Jwt.ValidateToken(tokenStr);
            if (uid == null)
                return AuthenticateResult.Fail("Invalid user ID or expired token.");

            var user = await this.Users.GetUserAsync(uid.Value);
            if (user == null)
                return AuthenticateResult.Fail("Invalid user ID.");

            var claims = new List<Claim>(4)
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.AsString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            if (user.IsAuthorized)
                claims.Add(new Claim(ClaimTypes.Role, JwtAuthenticationOptions.RoleParticipant));

            if (user.Team != null)
                claims.Add(new Claim(ClaimTypes.Role, JwtAuthenticationOptions.RoleTeamMember));
            else
                claims.Add(new Claim(ClaimTypes.Role, JwtAuthenticationOptions.RoleUnteamed));

            var identity = new ClaimsIdentity(claims, this.Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            this.Context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    }
}
