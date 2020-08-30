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
using System.Linq;

namespace RosettaCTF.Authentication
{
    /// <summary>
    /// Selects an OAuth provider implementation based on ID.
    /// </summary>
    public sealed class OAuthProviderSelector
    {
        private IEnumerable<IOAuthProvider> Providers { get; }

        /// <summary>
        /// Creates a new selector.
        /// </summary>
        /// <param name="providers">Providers to provide.</param>
        public OAuthProviderSelector(IEnumerable<IOAuthProvider> providers)
        {
            this.Providers = providers;
        }

        /// <summary>
        /// Gets a provider by its ID.
        /// </summary>
        /// <param name="providerId">ID of the provider to retrieve.</param>
        /// <returns>Requested provider.</returns>
        public IOAuthProvider GetById(string providerId)
            => this.Providers.FirstOrDefault(x => x.HasId(providerId));
    }
}
