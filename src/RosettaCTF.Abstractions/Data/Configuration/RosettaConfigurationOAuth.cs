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
    /// Represents OAuth2 configuration.
    /// </summary>
    public sealed class RosettaConfigurationOAuth
    {
        /// <summary>
        /// Gets or sets whether OAuth authentication is enabled.
        /// </summary>
        [Required]
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets the key used to encrypt OAuth tokens at rest. This key is not used directly; rather, a hash is derived from it.
        /// </summary>
        public string TokenKey { get; set; }

        /// <summary>
        /// Gets or sets the collection of configured OAuth providers.
        /// </summary>
        public RosettaConfigurationOAuthProvider[] Providers { get; set; }
    }
}
