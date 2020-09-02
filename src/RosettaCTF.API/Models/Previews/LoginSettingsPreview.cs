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

using System.Collections.Generic;

namespace RosettaCTF.Models.Previews
{
    /// <summary>
    /// Represents login settings.
    /// </summary>
    public sealed class LoginSettingsPreview
    {
        /// <summary>
        /// Gets whether local login is enabled.
        /// </summary>
        public bool LocalLoginEnabled { get; }

        /// <summary>
        /// Gets whether external logins are enabled.
        /// </summary>
        public bool ExternalLoginEnabled { get; }

        /// <summary>
        /// Gets all registered external login providers.
        /// </summary>
        public IEnumerable<LoginProviderPreview> ExternalAccountProviders { get; }

        internal LoginSettingsPreview(bool local, bool external, IEnumerable<LoginProviderPreview> providers)
        {
            this.LocalLoginEnabled = local;
            this.ExternalLoginEnabled = external;
            this.ExternalAccountProviders = providers;
        }
    }
}
