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
    [ServiceFilter(typeof(ValidRosettaUserFilter))]
    [ValidateAntiForgeryToken]
    public class TeamController : RosettaControllerBase
    {
        public TeamController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            UserPreviewRepository userPreviewRepository,
            ICtfConfigurationLoader ctfConfigurationLoader)
            : base(loggerFactory, userRepository, userPreviewRepository, ctfConfigurationLoader)
        { }

        [HttpGet]
        [Authorize(Roles = JwtAuthenticationOptions.RoleParticipant)]
        [Route("{id?}")]
        public async Task<ActionResult<ApiResult<TeamPreview>>> Get(long? id = null, CancellationToken cancellationToken = default)
        {
            ITeam team;
            if (id == null)
                team = this.RosettaUser.Team;
            else
                team = await this.UserRepository.GetTeamAsync(id.Value);

            if (team == null)
                return this.NotFound(ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.TeamNotFound, "Specified team does not exist.")));

            var rteam = this.UserPreviewRepository.GetTeam(team);
            return this.Ok(ApiResult.FromResult(rteam));
        }

        [HttpPost]
        [Authorize(Roles = JwtAuthenticationOptions.RoleParticipant)]
        [Authorize(Roles = JwtAuthenticationOptions.RoleUnteamed)]
        [ServiceFilter(typeof(EventNotStartedFilter))]
        public async Task<ActionResult<ApiResult<TeamPreview>>> Create([FromBody] TeamCreateModel teamCreate, CancellationToken cancellationToken = default)
        {
            if (DateTimeOffset.UtcNow >= this.EventConfiguration.StartTime)
                return this.StatusCode(403, ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.EventStarted, "Cannot modify team composition after the event has started.")));

            var team = await this.UserRepository.CreateTeamAsync(teamCreate.Name, cancellationToken);
            if (team == null)
                return this.Conflict(ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.DuplicateTeamName, "A team with given name already exists.")));

            await this.UserRepository.AssignTeamMembershipAsync(this.RosettaUser.Id, team.Id, cancellationToken);

            var rteam = this.UserPreviewRepository.GetTeam(team);
            return this.StatusCode(201, ApiResult.FromResult(rteam));
        }

        [HttpDelete]
        [Authorize(Roles = JwtAuthenticationOptions.RoleParticipant)]
        [Authorize(Roles = JwtAuthenticationOptions.RoleTeamMember)]
        [ServiceFilter(typeof(EventNotStartedFilter))]
        [Route("members/{userId}")]
        public async Task<ActionResult<ApiResult<TeamPreview>>> Kick(long userId, CancellationToken cancellationToken = default)
        {
            if (DateTimeOffset.UtcNow >= this.EventConfiguration.StartTime)
                return this.StatusCode(403, ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.EventStarted, "Cannot modify team composition after the event has started.")));

            var tuser = await this.UserRepository.GetUserAsync(userId, cancellationToken);
            if (tuser == null)
                return this.NotFound(ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.UserNotFound, "Specified user does not exist.")));

            if (tuser.Team == null || tuser.Team.Id != this.RosettaUser.Team.Id)
                return this.NotFound(ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.UserNotFound, "Specified user is not part of this team.")));

            await this.UserRepository.AssignTeamMembershipAsync(tuser.Id, null, cancellationToken);

            if (this.RosettaUser.Id != tuser.Id)
            {
                var team = await this.UserRepository.GetTeamAsync(this.RosettaUser.Team.Id);
                var rteam = this.UserPreviewRepository.GetTeam(team);
                return this.Ok(ApiResult.FromResult(rteam));
            }
            else
            {
                return this.Ok(ApiResult.FromResult<TeamPreview>(null));
            }
        }

        [HttpPost]
        [Authorize(Roles = JwtAuthenticationOptions.RoleParticipant)]
        [Authorize(Roles = JwtAuthenticationOptions.RoleTeamMember)]
        [ServiceFilter(typeof(EventNotStartedFilter))]
        [Route("invites/{userId}")]
        public async Task<ActionResult<ApiResult<TeamPreview>>> Invite(long userId, CancellationToken cancellationToken = default)
        {
            if (DateTimeOffset.UtcNow >= this.EventConfiguration.StartTime)
                return this.StatusCode(403, ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.EventStarted, "Cannot modify team composition after the event has started.")));

            var tuser = await this.UserRepository.GetUserAsync(userId, cancellationToken);
            if (tuser == null)
                return this.NotFound(ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.UserNotFound, "Specified user does not exist.")));

            if (!tuser.IsAuthorized)
                return this.StatusCode(403, ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.MissingPermissions, "User is not allowed to participate.")));

            if (tuser.Team != null)
                return this.StatusCode(403, ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.UserAlreadyOnTeam, "Specified user already has a team.")));

            await this.UserRepository.CreateTeamInviteAsync(tuser.Id, this.RosettaUser.Team.Id, cancellationToken);

            var rteam = this.UserPreviewRepository.GetTeam(this.RosettaUser.Team);
            return this.Ok(ApiResult.FromResult(rteam));
        }

        [HttpPost]
        [Authorize(Roles = JwtAuthenticationOptions.RoleParticipant)]
        [Authorize(Roles = JwtAuthenticationOptions.RoleUnteamed)]
        [ServiceFilter(typeof(EventNotStartedFilter))]
        [Route("invites/{teamId}/accept")]
        public async Task<ActionResult<ApiResult<TeamPreview>>> AcceptInvite(long teamId, CancellationToken cancellationToken = default)
        {
            if (DateTimeOffset.UtcNow >= this.EventConfiguration.StartTime)
                return this.StatusCode(403, ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.EventStarted, "Cannot modify team composition after the event has started.")));

            var invite = await this.UserRepository.GetTeamInviteAsync(this.RosettaUser.Id, teamId, cancellationToken);
            if (invite == null)
                return this.NotFound(ApiResult.FromError<TeamPreview>(new ApiError(ApiErrorCode.Unauthorized, "You were not invited to that team.")));

            await this.UserRepository.AssignTeamMembershipAsync(this.RosettaUser.Id, invite.Team.Id, cancellationToken);
            await this.UserRepository.ClearTeamInvitesAsync(this.RosettaUser.Id, cancellationToken);

            var team = await this.UserRepository.GetTeamAsync(invite.Team.Id);
            var rteam = this.UserPreviewRepository.GetTeam(team);
            return this.Ok(ApiResult.FromResult(rteam));
        }

        [HttpGet]
        [Authorize(Roles = JwtAuthenticationOptions.RoleParticipant)]
        [Authorize(Roles = JwtAuthenticationOptions.RoleUnteamed)]
        [ServiceFilter(typeof(EventNotStartedFilter))]
        [Route("invites")]
        public async Task<ActionResult<ApiResult<IEnumerable<TeamInvitePreview>>>> GetInvites(CancellationToken cancellationToken = default)
        {
            if (DateTimeOffset.UtcNow >= this.EventConfiguration.StartTime)
                return this.StatusCode(403, ApiResult.FromError<IEnumerable<TeamInvitePreview>>(new ApiError(ApiErrorCode.EventStarted, "Cannot modify team composition after the event has started.")));

            var invites = await this.UserRepository.GetTeamInvitesAsync(this.RosettaUser.Id, cancellationToken);

            var rinvites = this.UserPreviewRepository.GetInvites(invites);
            return this.Ok(ApiResult.FromResult(rinvites));
        }
    }
}
