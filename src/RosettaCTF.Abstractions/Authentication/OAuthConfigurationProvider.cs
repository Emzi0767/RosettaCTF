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
using Microsoft.Extensions.Options;
using RosettaCTF.Data;

namespace RosettaCTF.Authentication
{
    /// <summary>
    /// Provides configuration for OAuth providers.
    /// </summary>
    public sealed class OAuthConfigurationProvider
    {
        private ConfigurationOAuth Configuration { get; }

        /// <summary>
        /// Creates a new provider, with supplied options.
        /// </summary>
        /// <param name="config">OAuth configuration.</param>
        public OAuthConfigurationProvider(IOptions<ConfigurationOAuth> config)
        {
            this.Configuration = config.Value;
        }

        /// <summary>
        /// Retrieves and returns the configuration for specified provider.
        /// </summary>
        /// <param name="providerId">ID of the provider.</param>
        /// <returns>Requested configuration.</returns>
        public ConfigurationOAuthProvider GetById(string providerId)
            => this.Configuration.Providers.FirstOrDefault(x => string.Equals(x.Id ?? x.Type, providerId, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Retrieves and returns the configuration objects for specified provider type.
        /// </summary>
        /// <param name="providerType">Type of provider to get the configuration for.</param>
        /// <returns>Requested configuration objects.</returns>
        public IEnumerable<ConfigurationOAuthProvider> GetByType(string providerType)
            => this.Configuration.Providers.Where(x => string.Equals(x.Type, providerType, StringComparison.OrdinalIgnoreCase));
    }
}
