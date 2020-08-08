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
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RosettaCTF.Data;
using RosettaCTF.Models;
using RosettaCTF.Services;

namespace RosettaCTF.Controllers
{
    [Route("api/[controller]"), ValidateAntiForgeryToken]
    public class SessionController : RosettaControllerBase
    {
        private IUserRepository UserRepository { get; }
        private UserPreviewRepository UserPreviewRepository { get; }
        private DiscordHandler Discord { get; }
        private JwtHandler Jwt { get; }

        public SessionController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            UserPreviewRepository userPreviewRepository,
            DiscordHandler discord,
            JwtHandler jwt)
            : base(loggerFactory)
        {
            this.UserRepository = userRepository;
            this.UserPreviewRepository = userPreviewRepository;
            this.Discord = discord;
            this.Jwt = jwt;
        }

        [HttpGet, Route("endpoint"), AllowAnonymous]
        public ActionResult<ApiResult<string>> Endpoint()
            => ApiResult.FromResult(this.Discord.GetAuthenticationUrl(this.HttpContext));

        [HttpGet, Authorize]
        public async Task<ActionResult<ApiResult<SessionPreview>>> GetCurrent(CancellationToken cancellationToken = default)
        {
            var user = await this.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.NotLoggedIn, "User is not logged in.")));

            var ruser = this.UserPreviewRepository.GetUser(user);
            var token = this.Jwt.IssueToken(ruser);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(ruser, token)));
        }

        [HttpPut, AllowAnonymous]
        public async Task<ActionResult<ApiResult<SessionPreview>>> Login([FromBody] OAuthAuthenticationData data, CancellationToken cancellationToken = default)
        {
            var tokens = await this.Discord.CompleteLoginAsync(this.HttpContext, data, cancellationToken);
            if (tokens == null)
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.ExternalAuthenticationError, "Failed to authenticate with Discord.")));

            var expires = DateTimeOffset.UtcNow.AddSeconds(tokens.ExpiresIn - 10);
            var duser = await this.Discord.GetUserAsync(this.HttpContext, tokens.AccessToken, cancellationToken);
            if (duser == null || !duser.IsAuthorized)
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.Unauthorized, "You are not authorized to participate.")));

            var did = ulong.Parse(duser.Id, NumberStyles.Number, CultureInfo.InvariantCulture);
            var user = await this.UserRepository.GetUserAsync((long)did, cancellationToken);
            if (user == null)
                user = await this.UserRepository.CreateUserAsync($"{duser.Username}#{duser.Discriminator}", did, tokens.AccessToken, tokens.RefreshToken, expires, duser.IsAuthorized, cancellationToken);
            else
                await this.UserRepository.UpdateTokensAsync((long)did, tokens.AccessToken, tokens.RefreshToken, expires, cancellationToken);

            var ruser = this.UserPreviewRepository.GetUser(user);
            var token = this.Jwt.IssueToken(ruser);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(ruser, token)));
        }

        [HttpDelete, Authorize]
        public async Task<ActionResult<ApiResult<SessionPreview>>> Logout(CancellationToken cancellationToken = default)
        {
            var user = await this.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.NotLoggedIn, "User is not logged in.")));

            if (!await this.Discord.LogoutAsync(this.HttpContext, user.Token, cancellationToken))
                this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.NotLoggedIn, "User is not logged in.")));

            await this.UserRepository.UpdateTokensAsync(user.Id, null, null, DateTimeOffset.MinValue, cancellationToken);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(null)));
        }

        private async Task<IUser> GetCurrentUserAsync(CancellationToken cancellationToken)
        {
            var uids = this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (uids == null)
                return null;

            var uid = uids.ParseAsLong();
            return await this.UserRepository.GetUserAsync(uid, cancellationToken);
        }
    }
}
