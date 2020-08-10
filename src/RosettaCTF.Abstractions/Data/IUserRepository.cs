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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RosettaCTF.Data
{
    /// <summary>
    /// Provides an abstraction over querying and manipulating users and teams.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">ID of the user to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Requested user.</returns>
        Task<IUser> GetUserAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new user with specified parameters. This method will fail if a user with this name exists.
        /// </summary>
        /// <param name="username">Username for the user.</param>
        /// <param name="discordId">Discord ID of the user.</param>
        /// <param name="token">Discord OAuth2 authentication token.</param>
        /// <param name="refreshToken">Discord OAuth2 refresh token.</param>
        /// <param name="tokenExpiresAt">Exact timestamp at which this OAuth2 token expires.</param>
        /// <param name="isAuthorized">Whether the user is authorized.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Created user.</returns>
        Task<IUser> CreateUserAsync(string username, ulong discordId, string token, string refreshToken, DateTimeOffset tokenExpiresAt, bool isAuthorized, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">ID of the user to delete.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task DeleteUserAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a team by its ID.
        /// </summary>
        /// <param name="id">ID of the team to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Requested team.</returns>
        Task<ITeam> GetTeamAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new team with specified parameters. This method will fail if a team with this name exists.
        /// </summary>
        /// <param name="name">Name for the team.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Created team.</returns>
        Task<ITeam> CreateTeamAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a team by its ID.
        /// </summary>
        /// <param name="id">ID of the team to delete.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task DeleteTeamAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns a user to a team. A null team will unassign the user from the team.
        /// </summary>
        /// <param name="userId">ID of the user to assign to the team.</param>
        /// <param name="teamId">ID of the team to assign the user to. Null will unassign the user from the team.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task AssignTeamMembershipAsync(long userId, long? teamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates OAuth2 authentication data for a user.
        /// </summary>
        /// <param name="userId">ID of the user to update authentication data for.</param>
        /// <param name="token">New Discord OAuth2 authentication token.</param>
        /// <param name="refreshToken">New Discord OAuth2 refresh token.</param>
        /// <param name="tokenExpiresAt">New expiration timestamp for the token.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task UpdateTokensAsync(long userId, string token, string refreshToken, DateTimeOffset tokenExpiresAt, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a user's ability to view hidden challenges and categories.
        /// </summary>
        /// <param name="userId">ID of the user to update the relevant flags for.</param>
        /// <param name="enable">Whether hidden challenges should be visible or not.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task EnableHiddenChallengesAsync(long userId, bool enable, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets outstanding team invites for a given user.
        /// </summary>
        /// <param name="userId">ID of the user to get invites for.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>An enumerable of <see cref="ITeamInvite"/>.</returns>
        Task<IEnumerable<ITeamInvite>> GetTeamInvitesAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an outstanding team invite for specified team and user combo.
        /// </summary>
        /// <param name="userId">ID of the user to get the invite for.</param>
        /// <param name="teamId">ID of the team to get the invite for.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>The requested invite.</returns>
        Task<ITeamInvite> GetTeamInviteAsync(long userId, long teamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Clears any outstanding invites for a given user.
        /// </summary>
        /// <param name="userId">ID of the user to clear invites for.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task ClearTeamInvitesAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates an outstanding team invite for a given user.
        /// </summary>
        /// <param name="userId">ID of the user to create an invite for.</param>
        /// <param name="teamId">ID of the team to invite the user to.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>The created <see cref="ITeamInvite"/>.</returns>
        Task<ITeamInvite> CreateTeamInviteAsync(long userId, long teamId, CancellationToken cancellationToken = default);
    }
}
