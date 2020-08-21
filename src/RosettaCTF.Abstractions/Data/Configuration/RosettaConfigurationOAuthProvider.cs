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

using System.ComponentModel.DataAnnotations;

namespace RosettaCTF.Data
{
    /// <summary>
    /// Represents a configuration for an OAuth2 provider.
    /// </summary>
    public sealed class RosettaConfigurationOAuthProvider
    {
        /// <summary>
        /// Gets or sets the type of configured provider.
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the client ID for this provider.
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client scret for this provider.
        /// </summary>
        [Required]
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the IDs of Discord servers used to authorize the participants. If the user is not in that server, they won't be authorized. Discord-specific option.
        /// </summary>
        public ulong[] AuthorizedGuilds { get; set; }

        /// <summary>
        /// Gets or sets the URL of the OAuth login page.
        /// </summary>
        public string AuthorizeUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL of the token exchange endpoint.
        /// </summary>
        public string TokenUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL of the token refresh endpoint. Optional.
        /// </summary>
        public string RefreshUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL of the token revoke endpoint. Optional.
        /// </summary>
        public string RevokeUrl { get; set; }
    }
}
