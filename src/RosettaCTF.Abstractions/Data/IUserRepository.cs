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
        /// Retrieves a user by their name.
        /// </summary>
        /// <param name="username">Username of the user to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Requested user.</returns>
        Task<IUser> GetUserAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new user with specified parameters. This method will fail if a user with this name exists.
        /// </summary>
        /// <param name="username">Username for the user.</param>
        /// <param name="isAuthorized">Whether the user is authorized.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Created user.</returns>
        Task<IUser> CreateUserAsync(string username, bool isAuthorized, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">ID of the user to delete.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task DeleteUserAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a user's country.
        /// </summary>
        /// <param name="id">ID of the user to update the country for.</param>
        /// <param name="code">Country code to set the country to, or null to unset.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task<IUser> UpdateUserCountryAsync(long id, string code, CancellationToken cancellationToken = default);

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
        /// Updates a user with a new password, or disables password login.
        /// </summary>
        /// <param name="userId">ID of the user to update.</param>
        /// <param name="password">Password to set, or null to disable.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task UpdateUserPasswordAsync(long userId, byte[] password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the password hash to authenticate the user with.
        /// </summary>
        /// <param name="userId">ID of the user to get the password for.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The retrieved password hash.</returns>
        Task<byte[]> GetUserPasswordAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new external account connection for specified user.
        /// </summary>
        /// <param name="userId">ID of the user to create an external connection for.</param>
        /// <param name="extId">ID of the user at the external account provider.</param>
        /// <param name="extName">Username at the external account provider.</param>
        /// <param name="providerId">ID of the provider of this account.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Created connection.</returns>
        Task<IExternalUser> ConnectExternalAccountAsync(long userId, string extId, string extName, string providerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an external account connection for specified user.
        /// </summary>
        /// <param name="userId">ID of the user to remove an external connection from.</param>
        /// <param name="providerId">ID of the external account provider.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task RemoveExternalAccountAsync(long userId, string providerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an external account connection for specified user.
        /// </summary>
        /// <param name="userId">ID of the user to get an external connection for.</param>
        /// <param name="providerId">ID of the external account provider.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Retrieved account connection.</returns>
        Task<IExternalUser> GetExternalAccountAsync(long userId, string providerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an external account by its ID.
        /// </summary>
        /// <param name="id">ID of the external account.</param>
        /// <param name="providerId">ID of the external account provider.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Retrieved account connection.</returns>
        Task<IExternalUser> GetExternalAccountAsync(string id, string providerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates OAuth2 authentication data for a user.
        /// </summary>
        /// <param name="userId">ID of the user to update authentication data for.</param>
        /// <param name="providerId">ID of the external account provider.</param>
        /// <param name="token">New Discord OAuth2 authentication token.</param>
        /// <param name="refreshToken">New Discord OAuth2 refresh token.</param>
        /// <param name="tokenExpiresAt">New expiration timestamp for the token.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task UpdateTokensAsync(long userId, string providerId, string token, string refreshToken, DateTimeOffset tokenExpiresAt, CancellationToken cancellationToken = default);

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

        /// <summary>
        /// Gets the number of teams which solved the baseline challenge. A baseline challenge is the challenge with a base score of 1. This is used to compute solve rate.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>Number of teams that successfully solved the baseline challenge, and are therefore considered active.</returns>
        Task<int> GetBaselineSolveCount(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all defined countries.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>An enumerable of all defined countries.</returns>
        Task<IEnumerable<ICountry>> GetAllCountriesAsync(CancellationToken cancellationToken = default);
    }
}
