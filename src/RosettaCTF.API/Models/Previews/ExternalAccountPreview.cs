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

using RosettaCTF.Authentication;
using RosettaCTF.Data;

namespace RosettaCTF.Models
{
    /// <summary>
    /// Contains an abridged version of an external account.
    /// </summary>
    public sealed class ExternalAccountPreview
    {
        /// <summary>
        /// Gets the ID of the external account.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the username of the external account.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the provider of the external account.
        /// </summary>
        public LoginProviderPreview Provider { get; }

        internal ExternalAccountPreview(IExternalUser externalUser, IOAuthProvider provider)
        {
            var pid = externalUser.ProviderId;

            this.Id = externalUser.Id;
            this.Username = externalUser.Username;
            this.Provider = new LoginProviderPreview(pid, provider.GetName(pid), provider.GetColour(pid));
        }
    }
}
