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

namespace RosettaCTF.Data
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
        /// <param name="claimsPrincipal">Principal to encapsulate state for.</param>
        /// <returns>Encapsulated state.</returns>
        public SessionPreview GetSession(UserPreview user)
            => new SessionPreview(user);
    }
}
