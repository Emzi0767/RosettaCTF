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
    /// Represents a result of OAuth authentication attempt.
    /// </summary>
    public sealed class OAuthResult
    {
        /// <summary>
        /// Gets or sets whether the result is a succssful result.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the token type. This should be set to bearer.
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds until the token expires.
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the refresh token. This should be used to refresh expired tokens.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the scopes for the token.
        /// </summary>
        public string Scope { get; set; }
    }
}
