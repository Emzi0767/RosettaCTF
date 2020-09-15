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
using RosettaCTF.Authentication;
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF.Services
{
    /// <summary>
    /// Provides the ability to transform <see cref="IUser"/> and <see cref="ITeam"/> instances to their abridged variants.
    /// </summary>
    public sealed class UserPreviewRepository
    {
        /// <summary>
        /// Gets a redacted version of a user.
        /// </summary>
        /// <param name="user">User to transform.</param>
        /// <returns>An abridged version of the user.</returns>
        public UserPreview GetUser(IUser user)
            => user != null 
                ? new UserPreview(user, this.GetTeam(user.Team)) 
                : null;

        /// <summary>
        /// Gets a redacted version of a team.
        /// </summary>
        /// <param name="team">Team to transform.</param>
        /// <returns>An abridged version of the team.</returns>
        public TeamPreview GetTeam(ITeam team)
            => team != null 
                ? new TeamPreview(team) 
                : null;

        /// <summary>
        /// Gets a session from a principal.
        /// </summary>
        /// <param name="user">User to encapsulate session for.</param>
        /// <returns>Encapsulated session.</returns>
        public SessionPreview GetSession(UserPreview user)
            => new SessionPreview(user);

        /// <summary>
        /// Gets a session from a principal and updates the token.
        /// </summary>
        /// <param name="user">User to encapsulate session for.</param>
        /// <param name="token">Token to send along.</param>
        /// <param name="expiresAt">The timestamp at which the token expires.</param>
        /// <param name="requireMfa">A value indicating whether the current user requires MFA.</param>
        /// <returns>Encapsulated session.</returns>
        public SessionPreview GetSession(UserPreview user, string token, DateTimeOffset expiresAt, bool requireMfa)
            => new SessionPreview(user, token, expiresAt, requireMfa);

        /// <summary>
        /// Gets a session indicating authentication needs to be continued with MFA.
        /// </summary>
        /// <param name="continuation">Continuation token for the authentication process.</param>
        /// <returns>Encapsulated session.</returns>
        public SessionPreview GetSession(string continuation)
            => new SessionPreview(continuation);

        /// <summary>
        /// Gets redacted versions of team invites.
        /// </summary>
        /// <param name="invites">Team invites to transform.</param>
        /// <returns>An enumerable of abridged invites.</returns>
        public IEnumerable<TeamInvitePreview> GetInvites(IEnumerable<ITeamInvite> invites)
            => invites.Select(x => new TeamInvitePreview(x)).ToList();

        /// <summary>
        /// Gets redacted versions of account connections.
        /// </summary>
        /// <param name="externalUsers">External users to transform.</param>
        /// <returns>An enumerable of abridged connections.</returns>
        public IEnumerable<ExternalAccountPreview> GetConnections(IEnumerable<IExternalUser> externalUsers, OAuthProviderSelector oAuthProviderSelector)
            => externalUsers.Select(x => new ExternalAccountPreview(x, oAuthProviderSelector.GetById(x.ProviderId))).ToList();
    }
}
