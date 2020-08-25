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

namespace RosettaCTF.Authentication
{
    /// <summary>
    /// Represents information about a user obtained from OAuth2.
    /// </summary>
    public sealed class OAuthUser
    {
        /// <summary>
        /// Gets the ID of the user.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string Username { get; }
        
        /// <summary>
        /// Gets whether the user is authorized to participate in the event.
        /// </summary>
        public bool IsAuthorized { get; }

        /// <summary>
        /// Creates OAuth user encapsulator.
        /// </summary>
        /// <param name="id">ID of the user.</param>
        /// <param name="username">Name of the user.</param>
        /// <param name="authorized">Whether the user is authorized.</param>
        public OAuthUser(string id, string username, bool authorized)
        {
            this.Id = id;
            this.Username = username;
            this.IsAuthorized = authorized;
        }
    }
}
