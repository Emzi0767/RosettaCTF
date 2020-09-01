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

namespace RosettaCTF.Data
{
    /// <summary>
    /// Represents a user sourced from an external authentication provider.
    /// </summary>
    public interface IExternalUser
    {
        /// <summary>
        /// Gets the external ID of this account.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the external username of this account.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets the ID of the account provider.
        /// </summary>
        string ProviderId { get; }

        /// <summary>
        /// Gets the OAuth token for this user.
        /// </summary>
        string Token { get; }

        /// <summary>
        /// Gets the OAuth refresh token for this user.
        /// </summary>
        string RefreshToken { get; }

        /// <summary>
        /// Gets the time at which the token expires.
        /// </summary>
        DateTimeOffset? TokenExpirationTime { get; }

        /// <summary>
        /// Gets the user the account is associated with.
        /// </summary>
        IUser User { get; }
    }
}
