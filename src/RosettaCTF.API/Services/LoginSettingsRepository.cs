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

using System.Linq;
using Microsoft.Extensions.Options;
using RosettaCTF.Authentication;
using RosettaCTF.Data;
using RosettaCTF.Models.Previews;

namespace RosettaCTF.Services
{
    /// <summary>
    /// Allows retrieving login settings.
    /// </summary>
    public class LoginSettingsRepository
    {
        /// <summary>
        /// Gets the configured login settings.
        /// </summary>
        public LoginSettingsPreview LoginSettings { get; }

        /// <summary>
        /// Creates a new setting repository.
        /// </summary>
        /// <param name="cfg">OAuth configuration.</param>
        /// <param name="oAuthProviderSelector">Provider selector.</param>
        public LoginSettingsRepository(
            IOptions<ConfigurationAuthentication> cfg,
            OAuthProviderSelector oAuthProviderSelector)
        {
            var config = cfg.Value;
            var providers = config.OAuth.Enable
                ? config.OAuth.Providers.Select(x => ProviderToPreview(x, oAuthProviderSelector))
                : null;

            this.LoginSettings = new LoginSettingsPreview(config.LocalLogin, config.OAuth.Enable, providers);
        }

        private static LoginProviderPreview ProviderToPreview(ConfigurationOAuthProvider provider, OAuthProviderSelector selector)
        {
            var id = provider.Id ?? provider.Type;
            var prv = selector.GetById(id);
            return new LoginProviderPreview(id, prv.GetName(id), prv.GetColour(id));
        }
    }
}
