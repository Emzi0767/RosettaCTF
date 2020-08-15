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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RosettaCTF.Data;
using RosettaCTF.Filters;
using RosettaCTF.Models;
using RosettaCTF.Services;

namespace RosettaCTF.Controllers
{
    [Route("api/[controller]")]
    [ValidateAntiForgeryToken]
    public class SessionController : RosettaControllerBase
    {
        private DiscordHandler Discord { get; }
        private JwtHandler Jwt { get; }
        private IOAuthStateRepository OAuthStateRepository { get; }

        public SessionController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            ICtfConfigurationLoader ctfConfigurationLoader,
            UserPreviewRepository userPreviewRepository,
            DiscordHandler discord,
            JwtHandler jwt,
            IOAuthStateRepository oAuthStateRepository)
            : base(loggerFactory, userRepository, userPreviewRepository, ctfConfigurationLoader)
        {
            this.Discord = discord;
            this.Jwt = jwt;
            this.OAuthStateRepository = oAuthStateRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("endpoint")]
        public async Task<ActionResult<ApiResult<string>>> Endpoint(CancellationToken cancellationToken = default)
        {
            var state = await this.OAuthStateRepository.GenerateStateAsync(this.HttpContext.Connection.RemoteIpAddress.ToString(), cancellationToken);
            return this.Ok(ApiResult.FromResult(this.Discord.GetAuthenticationUrl(this.HttpContext, state)));
        }

        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        [Route("refresh")]
        public ActionResult<ApiResult<SessionPreview>> Refresh()
        {
            var ruser = this.UserPreviewRepository.GetUser(this.RosettaUser);
            var token = this.Jwt.IssueToken(ruser);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(ruser, token.Token, token.ExpiresAt)));
        }

        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        public ActionResult<ApiResult<SessionPreview>> GetCurrent()
        {
            var ruser = this.UserPreviewRepository.GetUser(this.RosettaUser);
            var token = this.Jwt.IssueToken(ruser);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(ruser, token.Token, token.ExpiresAt)));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<SessionPreview>>> Login([FromBody] OAuthAuthenticationData data, CancellationToken cancellationToken = default)
        {
            if (data.State == null || !await this.OAuthStateRepository.ValidateStateAsync(this.HttpContext.Connection.RemoteIpAddress.ToString(), data.State, cancellationToken))
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.Unauthorized, "OAuth state validation failed.")));

            var tokens = await this.Discord.CompleteLoginAsync(this.HttpContext, data, cancellationToken);
            if (tokens == null)
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.ExternalAuthenticationError, "Failed to authenticate with Discord.")));

            var expires = DateTimeOffset.UtcNow.AddSeconds(tokens.ExpiresIn - 20);
            var duser = await this.Discord.GetUserAsync(this.HttpContext, tokens.AccessToken, cancellationToken);
            if (duser == null || !duser.IsAuthorized)
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.Unauthorized, "You are not authorized to participate.")));

            var did = duser.Id.ParseAsUlong();
            var user = await this.UserRepository.GetUserAsync((long)did, cancellationToken);
            if (user == null)
                user = await this.UserRepository.CreateUserAsync($"{duser.Username}#{duser.Discriminator}", did, tokens.AccessToken, tokens.RefreshToken, expires, duser.IsAuthorized, cancellationToken);
            else
                await this.UserRepository.UpdateTokensAsync((long)did, tokens.AccessToken, tokens.RefreshToken, expires, cancellationToken);

            var ruser = this.UserPreviewRepository.GetUser(user);
            var token = this.Jwt.IssueToken(ruser);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(ruser, token.Token, token.ExpiresAt)));
        }

        [HttpDelete]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        public async Task<ActionResult<ApiResult<SessionPreview>>> Logout(CancellationToken cancellationToken = default)
        {
            if (!await this.Discord.LogoutAsync(this.HttpContext, this.RosettaUser.Token, cancellationToken))
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.NotLoggedIn, "User is not logged in.")));

            await this.UserRepository.UpdateTokensAsync(this.RosettaUser.Id, null, null, DateTimeOffset.MinValue, cancellationToken);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(null)));
        }

        [HttpPost]
        [Authorize(Roles = JwtAuthenticationOptions.RoleParticipant)]
        [Authorize(Roles = JwtAuthenticationOptions.RoleTeamMember)]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        [Route("unhide")]
        public async Task<ActionResult<ApiResult<object>>> Unhide(CancellationToken cancellationToken = default)
        {
            await this.UserRepository.EnableHiddenChallengesAsync(this.RosettaUser.Id, true, cancellationToken);

            return this.Ok(ApiResult.FromResult<object>(null));
        }
    }
}
