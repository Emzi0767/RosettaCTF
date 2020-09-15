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
using System.Buffers.Binary;
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
        private const string TokenActionOAuth = "oauth2";
        private const string TokenActionMFA = "multi-factor";
        private const char TokenSeparator = ':';

        private OAuthProviderSelector OAuthSelector { get; }
        private JwtHandler Jwt { get; }
        private ActionTokenPairHandler ActionTokenPairHandler { get; }
        private PasswordHandler Password { get; }
        private IOAuthStateRepository OAuthStateRepository { get; }
        private IMfaStateRepository MfaStateRepository { get; }
        private LoginSettingsRepository LoginSettingsRepository { get; }
        private MfaValidatorService MfaValidator { get; }
        private IMfaRepository MfaRepository { get; }

        public SessionController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            ICtfConfigurationLoader ctfConfigurationLoader,
            UserPreviewRepository userPreviewRepository,
            OAuthProviderSelector oAuthSelector,
            JwtHandler jwt,
            ActionTokenPairHandler actionTokenPairHandler,
            PasswordHandler pwdHandler,
            IOAuthStateRepository oAuthStateRepository,
            IMfaStateRepository mfaStateRepository,
            LoginSettingsRepository loginSettingsRepository,
            MfaValidatorService mfaValidator,
            IMfaRepository mfaRepository)
            : base(loggerFactory, userRepository, userPreviewRepository, ctfConfigurationLoader)
        {
            this.OAuthSelector = oAuthSelector;
            this.Jwt = jwt;
            this.ActionTokenPairHandler = actionTokenPairHandler;
            this.Password = pwdHandler;
            this.OAuthStateRepository = oAuthStateRepository;
            this.MfaStateRepository = mfaStateRepository;
            this.LoginSettingsRepository = loginSettingsRepository;
            this.MfaValidator = mfaValidator;
            this.MfaRepository = MfaRepository;
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

            var tkpair = this.ActionTokenPairHandler.IssueTokenPair(TokenActionOAuth);

            var stateId = await this.OAuthStateRepository.GenerateStateAsync(this.HttpContext.Connection.RemoteIpAddress.ToString(), tkpair.Server, cancellationToken);
            var clientToken = tkpair.Client;
            var state = this.PackState(stateId, clientToken);

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
            if (data.State == null)
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.Unauthorized, "OAuth state validation failed.")));

            var (stateId, clientTk) = this.UnpackState(data.State);
            var serverTk = await this.OAuthStateRepository.ValidateStateAsync(this.HttpContext.Connection.RemoteIpAddress.ToString(), stateId, cancellationToken);

            var tokenPair = new ActionTokenPair(clientTk, serverTk);
            if (!this.ActionTokenPairHandler.ValidateTokenPair(tokenPair, TokenActionOAuth))
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
        public async Task<ActionResult<ApiResult<SessionPreview>>> Login([FromBody] UserLoginModel data, CancellationToken cancellationToken = default)
        {
            var user = await this.UserRepository.GetUserAsync(data.Username, cancellationToken);
            if (user == null)
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            var pwd = await this.UserRepository.GetUserPasswordAsync(user.Id, cancellationToken);
            if (pwd == null)
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            if (!await this.Password.ValidatePasswordHashAsync(data.Password, pwd))
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            if (user.RequiresMfa)
            {
                var mfaState = new byte[8];
                BinaryPrimitives.WriteInt64BigEndian(mfaState, user.Id);
                var tokenPair = this.ActionTokenPairHandler.IssueTokenPair(TokenActionMFA, mfaState);

                var stateId = await this.MfaStateRepository.GenerateStateAsync(this.HttpContext.Connection.RemoteIpAddress.ToString(), tokenPair.Server, cancellationToken);
                var continuation = this.PackState(stateId, tokenPair.Client);

                return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(continuation)));
            }

            var ruser = this.UserPreviewRepository.GetUser(user);
            var token = this.Jwt.IssueToken(ruser);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(ruser, token.Token, token.ExpiresAt)));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("mfa")]
        public async Task<ActionResult<ApiResult<SessionPreview>>> LoginMfa([FromBody] MfaLoginModel data, CancellationToken cancellationToken = default)
        {
            if (data.ActionToken == null || data.MfaCode == null)
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.Unauthorized, "MFA state validation failed.")));

            var (stateId, clientTk) = this.UnpackState(data.ActionToken);
            var serverTk = await this.OAuthStateRepository.ValidateStateAsync(this.HttpContext.Connection.RemoteIpAddress.ToString(), stateId, cancellationToken);

            var tokenPair = new ActionTokenPair(clientTk, serverTk);
            if (!this.ActionTokenPairHandler.ValidateTokenPair(tokenPair, TokenActionOAuth))
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.Unauthorized, "MFA state validation failed.")));

            var userId = BinaryPrimitives.ReadInt64BigEndian(clientTk.State);
            var mfa = await this.MfaRepository.GetMfaSettingsAsync(userId, cancellationToken);
            if (mfa == null || !mfa.IsConfirmed)
                return this.StatusCode(400, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "MFA not configured.")));

            if (data.MfaCode.Length == 6)
            {
                if (!this.MfaValidator.ValidateCode(data.MfaCode, mfa))
                    return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Invalid MFA code provided.")));
            }
            else if (data.MfaCode.Length == 8)
            {
                if (!this.MfaValidator.ValidateRecoveryCode(data.MfaCode, mfa))
                    return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Invalid MFA code provided.")));

                await this.MfaRepository.TripRecoveryCodeAsync(userId, cancellationToken);
            }
            else
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Invalid MFA code provided.")));
            
            var user = await this.UserRepository.GetUserAsync(userId, cancellationToken);
            var ruser = this.UserPreviewRepository.GetUser(user);
            var token = this.Jwt.IssueToken(ruser);
            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(ruser, token.Token, token.ExpiresAt)));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<ActionResult<ApiResult<bool>>> Register([FromBody] UserRegistrationModel data, CancellationToken cancellationToken = default)
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
        public async Task<ActionResult<ApiResult<bool>>> ChangePassword([FromBody] UserPasswordChangeModel data, CancellationToken cancellationToken = default)
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
        public async Task<ActionResult<ApiResult<bool>>> RemovePassword([FromBody] UserPasswordRemoveModel data, CancellationToken cancellationToken = default)
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

        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        [Route("mfa")]
        public async Task<ActionResult<ApiResult<MfaSettingsModel>>> StartMfaEnable([FromBody] UserSudoModel data, CancellationToken cancellationToken = default)
        {
            var user = this.RosettaUser;
            var pwd = await this.UserRepository.GetUserPasswordAsync(user.Id, cancellationToken);
            if (pwd == null || !await this.Password.ValidatePasswordHashAsync(data.Password, pwd))
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            var mfa = await this.MfaRepository.GetMfaSettingsAsync(user.Id, cancellationToken);
            if (mfa != null)
                return this.StatusCode(400, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.AlreadyConfigured, "MFA is already configured.")));

            mfa = await this.MfaValidator.GenerateMfaAsync(this.MfaRepository, user.Id, false, cancellationToken);
            var rmfa = this.MfaValidator.GenerateClientData(mfa, user.Username, this.EventConfiguration.Name);
            return this.Ok(ApiResult.FromResult(rmfa));
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        [Route("mfa/enable")]
        public async Task<ActionResult<ApiResult<bool>>> MfaEnable([FromBody] MfaEnableModel data, CancellationToken cancellationToken = default)
        {
            var user = this.RosettaUser;
            var pwd = await this.UserRepository.GetUserPasswordAsync(user.Id, cancellationToken);
            if (pwd == null || !await this.Password.ValidatePasswordHashAsync(data.Password, pwd))
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            var mfa = await this.MfaRepository.GetMfaSettingsAsync(user.Id, cancellationToken);
            if (mfa == null)
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "MFA not configured.")));

            if (mfa.IsConfirmed)
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.AlreadyConfigured, "MFA is already configured.")));

            if (!this.MfaValidator.ValidateCode(data.MfaCode, mfa))
            {
                await this.MfaRepository.RemoveMfaAsync(user.Id, cancellationToken);
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Invalid MFA code provided.")));
            }

            await this.MfaRepository.ConfirmMfaAsync(user.Id, cancellationToken);
            return this.Ok(ApiResult.FromResult(true));
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ValidRosettaUserFilter))]
        [Route("mfa/disable")]
        public async Task<ActionResult<ApiResult<bool>>> MfaDisable([FromBody] MfaEnableModel data, CancellationToken cancellationToken = default)
        {
            var user = this.RosettaUser;
            var pwd = await this.UserRepository.GetUserPasswordAsync(user.Id, cancellationToken);
            if (pwd == null || !await this.Password.ValidatePasswordHashAsync(data.Password, pwd))
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Specified credentials were invalid.")));

            var mfa = await this.MfaRepository.GetMfaSettingsAsync(user.Id, cancellationToken);
            if (mfa == null || !mfa.IsConfirmed)
                return this.StatusCode(401, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "MFA is not configured.")));

            if (data.MfaCode.Length == 6 && !this.MfaValidator.ValidateCode(data.MfaCode, mfa) ||
                data.MfaCode.Length == 8 && !this.MfaValidator.ValidateRecoveryCode(data.MfaCode, mfa))
                return this.StatusCode(403, ApiResult.FromError<SessionPreview>(new ApiError(ApiErrorCode.InvalidCredentials, "Invalid MFA code provided.")));

            await this.MfaRepository.RemoveMfaAsync(user.Id, cancellationToken);

            await this.MfaRepository.ConfirmMfaAsync(user.Id, cancellationToken);
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

            return this.Ok(ApiResult.FromResult(this.UserPreviewRepository.GetSession(user: null)));
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

        private (string stateId, ActionToken clientToken) UnpackState(string state)
        {
            var startAt = state.LastIndexOf(TokenSeparator);

            var stateId = new string(state.AsSpan(0, startAt));
            var clientTokenRaw = state.AsSpan(startAt + 1);
            if (!ActionToken.TryParse(clientTokenRaw, out var clientToken))
                return default;

            return (stateId, clientToken);
        }

        private string PackState(string stateId, ActionToken clientToken)
        {
            var clientTokenString = clientToken.ExportString();
            return string.Create(stateId.Length + clientTokenString.Length + 1, (stateId, clientTokenString), FormatState);

            static void FormatState(Span<char> buff, (string sid, string ctkn) state)
            {
                var (sid, ctkn) = state;

                sid.AsSpan().CopyTo(buff);
                ctkn.AsSpan().CopyTo(buff.Slice(sid.Length + 1));
                buff[sid.Length] = TokenSeparator;
            }
        }
    }
}
