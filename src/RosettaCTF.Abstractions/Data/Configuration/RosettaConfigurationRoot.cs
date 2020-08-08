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
    /// Represents the root of RosettaCTF basic configuration.
    /// </summary>
    public sealed class RosettaConfigurationRoot
    {
        /// <summary>
        /// Gets the path to event configuration file.
        /// </summary>
        [ValidFilePath]
        public string EventConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the persistent datastore configuration.
        /// </summary>
        [Required]
        public RosettaConfigurationDatastore Database { get; set; }

        /// <summary>
        /// Gets or sets the cache datastore configuration.
        /// </summary>
        [Required]
        public RosettaConfigurationCache Cache { get; set; }

        /// <summary>
        /// Gets or sets the HTTP configuration.
        /// </summary>
        [Required]
        public RosettaConfigurationHttp Http { get; set; }

        /// <summary>
        /// Gets or sets the Discord OAuth configuration.
        /// </summary>
        [Required]
        public RosettaConfigurationDiscord Discord { get; set; }

        /// <summary>
        /// Gets or sets the application token configuration.
        /// </summary>
        [Required]
        public RosettaConfigurationTokens Tokens { get; set; }
    }
}
