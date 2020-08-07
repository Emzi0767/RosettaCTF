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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RosettaCTF.Data;
using RosettaCTF.Discord;
using RosettaCTF.Models;

namespace RosettaCTF.Controllers
{
    [Route("api/[controller]"), ValidateAntiForgeryToken]
    public class SessionController : RosettaControllerBase
    {
        private IUserRepository UserRepository { get; }
        private UserPreviewRepository UserPreviewRepository { get; }
        private DiscordHandler Discord { get; }

        public SessionController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            UserPreviewRepository userPreviewRepository,
            DiscordHandler discord)
            : base(loggerFactory)
        {
            this.UserRepository = userRepository;
            this.UserPreviewRepository = userPreviewRepository;
            this.Discord = discord;
        }

        [HttpGet, Route("endpoint")]
        public ActionResult<ApiResult<string>> Endpoint()
            => ApiResult.FromResult(this.Discord.GetAuthenticationUrl(this.HttpContext));

        [HttpGet]
        public ActionResult<ApiResult<object>> Get()
            => ApiResult.FromResult<object>(new { authenticated = true, token = "asdf", user = new { id = "urmum", username = "Ur Mum#1234" }, team = new { name = "Test Team", members = new[] { new { id = "urmum", username = "Ur Mum#1234" } } } });

        [HttpPut, AllowAnonymous]
        public async Task<ActionResult<ApiResult<SessionPreview>>> Put([FromBody] OAuthAuthenticationData data, CancellationToken cancellationToken = default)
        {
            var token = await this.Discord.CompleteLoginAsync(this.HttpContext, data, cancellationToken);
            if (token == null)
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.ExternalAuthenticationError, "Failed to authenticate with Discord.")));

            var expires = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn - 10);
            var duser = await this.Discord.GetUserAsync(this.HttpContext, token.AccessToken, cancellationToken);
            if (duser == null || !duser.IsAuthorized)
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.Unauthorized, "You are not authorized to participate.")));

            var did = ulong.Parse(duser.Id, NumberStyles.Number, CultureInfo.InvariantCulture);
            var user = await this.UserRepository.GetUserAsync((long)did, cancellationToken);
            if (user == null)
                user = await this.UserRepository.CreateUserAsync($"{duser.Username}#{duser.Discriminator}", did, token.AccessToken, token.RefreshToken, expires, duser.IsAuthorized, cancellationToken);
            else
                await this.UserRepository.UpdateTokensAsync((long)did, token.AccessToken, token.RefreshToken, expires, cancellationToken);

            var ruser = this.UserPreviewRepository.GetUser(user);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(ruser)));
        }
    }
}
