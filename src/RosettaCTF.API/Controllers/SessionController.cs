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
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RosettaCTF.Authentication;
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
        private OAuthProviderSelector OAuthSelector { get; }
        private JwtHandler Jwt { get; }
        private PasswordHandler Password { get; }
        private IOAuthStateRepository OAuthStateRepository { get; }
        private LoginSettingsRepository LoginSettingsRepository { get; }

        public SessionController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            ICtfConfigurationLoader ctfConfigurationLoader,
            UserPreviewRepository userPreviewRepository,
            OAuthProviderSelector oAuthSelector,
            JwtHandler jwt,
            PasswordHandler pwdHandler,
            IOAuthStateRepository oAuthStateRepository,
            LoginSettingsRepository loginSettingsRepository)
            : base(loggerFactory, userRepository, userPreviewRepository, ctfConfigurationLoader)
        {
            this.OAuthSelector = oAuthSelector;
            this.Jwt = jwt;
            this.Password = pwdHandler;
            this.OAuthStateRepository = oAuthStateRepository;
            this.LoginSettingsRepository = loginSettingsRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("settings")]
        public ActionResult<ApiResult<LoginSettingsPreview>> Settings()
            => this.Ok(ApiResult.FromResult(this.LoginSettingsRepository.LoginSettings));

        [HttpGet]
        [AllowAnonymous]
        [Route("endpoint/{provider}")]
        public async Task<ActionResult<ApiResult<string>>> Endpoint(string provider, CancellationToken cancellationToken = default)
        {
            var oauth = this.OAuthSelector.GetById(provider);
            if (oauth == null)
                return this.NotFound(ApiResult.FromError<string>(new ApiError(ApiErrorCode.InvalidProvider, "Specified provider does not exist.")));

            var state = await this.OAuthStateRepository.GenerateStateAsync(this.HttpContext.Connection.RemoteIpAddress.ToString(), cancellationToken);
            return this.Ok(ApiResult.FromResult(oauth.GetAuthenticationUrl(this.CreateContext(provider, state))));
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
        [Route("oauth")]
        public async Task<ActionResult<ApiResult<SessionPreview>>> Login([FromBody] OAuthAuthenticationModel data, CancellationToken cancellationToken = default)
        {
            if (data.State == null || !await this.OAuthStateRepository.ValidateStateAsync(this.HttpContext.Connection.RemoteIpAddress.ToString(), data.State, cancellationToken))
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.Unauthorized, "OAuth state validation failed.")));

            var provider = this.OAuthSelector.IdFromReferrer(new Uri(data.Referrer));
            var oauth = this.OAuthSelector.GetById(provider);
            if (oauth == null)
                return this.NotFound(ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidProvider, "Specified provider does not exist.")));

            var ctx = this.CreateContext(provider, data.State);
            var tokens = await oauth.CompleteLoginAsync(ctx, data.Code, cancellationToken);
            if (tokens == null)
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.ExternalAuthenticationError, "Failed to authenticate with the OAuth provider.")));

            var expires = DateTimeOffset.UtcNow.AddSeconds(tokens.ExpiresIn - 20);
            var ouser = await oauth.GetUserAsync(ctx, tokens.AccessToken, cancellationToken);
            if (ouser == null || !ouser.IsAuthorized)
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.Unauthorized, "You are not authorized to participate.")));

            var oid = ouser.Id;
            var euser = await this.UserRepository.GetExternalAccountAsync(oid, provider, cancellationToken);
            var user = euser?.User;

            if (euser == null)
            {
                if (!AbstractionUtilities.NameRegex.IsMatch(ouser.Username))
                    return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidName, "Specified username contained invalid characters.")));

                var uid = this.GetUserId();
                if (uid == null)
                    user = await this.UserRepository.CreateUserAsync(ouser.Username, ouser.IsAuthorized, cancellationToken);
                else
                    user = await this.UserRepository.GetUserAsync(uid.Value, cancellationToken);

                euser = await this.UserRepository.ConnectExternalAccountAsync(user.Id, ouser.Id, ouser.Username, provider, cancellationToken);
            }

            await this.UserRepository.UpdateTokensAsync(user.Id, euser.ProviderId, tokens.AccessToken, tokens.RefreshToken, expires, cancellationToken);

            var ruser = this.UserPreviewRepository.GetUser(user);
            var token = this.Jwt.IssueToken(ruser);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(ruser, token.Token, token.ExpiresAt)));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<SessionPreview>>> Login([FromBody] UserAuthenticationModel data, CancellationToken cancellationToken = default)
        {
            var user = await this.UserRepository.GetUserAsync(data.Username, cancellationToken);
            if (user == null)
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            var pwd = await this.UserRepository.GetUserPasswordAsync(user.Id, cancellationToken);
            if (pwd == null)
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            if (!await this.Password.ValidatePasswordHashAsync(data.Password, pwd))
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            var ruser = this.UserPreviewRepository.GetUser(user);
            var token = this.Jwt.IssueToken(ruser);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(ruser, token.Token, token.ExpiresAt)));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<ActionResult<ApiResult<bool>>> Register([FromBody] UserRegistrationData data, CancellationToken cancellationToken = default)
        {
            var pwd = await this.Password.CreatePasswordHashAsync(data.Password);

            var user = await this.UserRepository.CreateUserAsync(data.Username, true, cancellationToken);
            if (user == null)
                return this.Conflict(ApiResult.FromError<bool>(new ApiError(ApiErrorCode.DuplicateUsername, "A user with given name already exists.")));

            await this.UserRepository.UpdateUserPasswordAsync(user.Id, pwd, cancellationToken);
            return this.Ok(ApiResult.FromResult(true));
        }

        [HttpPatch]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        [Route("password")]
        public async Task<ActionResult<ApiResult<bool>>> ChangePassword([FromBody] UserPasswordChangeData data, CancellationToken cancellationToken = default)
        {
            var user = this.RosettaUser;
            var pwd = await this.UserRepository.GetUserPasswordAsync(user.Id, cancellationToken);

            if (pwd != null && data.OldPassword == null)
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            if (pwd != null && !await this.Password.ValidatePasswordHashAsync(data.OldPassword, pwd))
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            var npwd = await this.Password.CreatePasswordHashAsync(data.NewPassword);
            await this.UserRepository.UpdateUserPasswordAsync(user.Id, npwd, cancellationToken);
            return this.Ok(ApiResult.FromResult(true));
        }

        [HttpPatch]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        [Route("password/remove")]
        public async Task<ActionResult<ApiResult<bool>>> RemovePassword([FromBody] UserPasswordRemoveData data, CancellationToken cancellationToken = default)
        {
            var user = this.RosettaUser;
            if (!user.ConnectedAccounts.Any())
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.GenericError, "To disable a password, at least one external account must be connected to this account.")));

            var pwd = await this.UserRepository.GetUserPasswordAsync(user.Id, cancellationToken);
            if (pwd == null || !await this.Password.ValidatePasswordHashAsync(data.Password, pwd))
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            await this.UserRepository.UpdateUserPasswordAsync(user.Id, null, cancellationToken);
            return this.Ok(ApiResult.FromResult(true));
        }

        [HttpPatch]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        [ServiceFilter(typeof(EventNotStartedFilter))]
        [Route("country")]
        public async Task<ActionResult<ApiResult<SessionPreview>>> UpdateUserCountry([FromBody] UserCountryModel data, CancellationToken cancellationToken = default)
        {
            var user = this.RosettaUser;
            user = await this.UserRepository.UpdateUserCountryAsync(user.Id, data.Code, cancellationToken);
            if (user == null)
                return this.BadRequest(ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidName, "Invalid country name specified.")));

            var ruser = this.UserPreviewRepository.GetUser(user);
            var token = this.Jwt.IssueToken(ruser);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(ruser, token.Token, token.ExpiresAt)));
        }

        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        [Route("connections")]
        public async Task<ActionResult<ApiResult<IEnumerable<ExternalAccountPreview>>>> GetConnections(CancellationToken cancellationToken = default)
        {
            var user = this.RosettaUser;
            var connections = await this.UserRepository.GetExternalAccountsAsync(user.Id, cancellationToken);
            var rconnections = this.UserPreviewRepository.GetConnections(connections, this.OAuthSelector);
            return this.Ok(ApiResult.FromResult(rconnections));
        }

        [HttpDelete]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        [Route("connections/{provider}")]
        public async Task<ActionResult<ApiResult<bool>>> RemoveConnection(string provider, CancellationToken cancellationToken = default)
        {
            var user = this.RosettaUser;
            await this.UserRepository.RemoveExternalAccountAsync(user.Id, provider, cancellationToken);
            return this.Ok(ApiResult.FromResult(true));
        }

        [HttpDelete]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        public async Task<ActionResult<ApiResult<SessionPreview>>> Logout(CancellationToken cancellationToken = default)
        {
            foreach (var xeuser in this.RosettaUser.ConnectedAccounts.Where(x => x.Token != null))
            {
                var oauth = this.OAuthSelector.GetById(xeuser.ProviderId);
                var euser = await this.UserRepository.GetExternalAccountAsync(this.RosettaUser.Id, xeuser.ProviderId, cancellationToken);
                await oauth.LogoutAsync(this.CreateContext(euser.ProviderId), euser.Token, cancellationToken);
                await this.UserRepository.UpdateTokensAsync(this.RosettaUser.Id, euser.ProviderId, null, null, DateTimeOffset.MinValue, cancellationToken);
            }

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

        private AuthenticationContext CreateContext(string provider = null, string state = null)
        {
            var req = this.HttpContext.Request;

            var scheme = req.Scheme;
            var host = req.Host.Host;
            var port = req.Host.Port ?? 0;

            return new AuthenticationContext(scheme, host, port, provider, state);
        }

        private long? GetUserId()
        {
            if (!this.User.Identity.IsAuthenticated)
                return null;

            var claim = this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            return claim.Value.ParseAsLong();
        }
    }
}
