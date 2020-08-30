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

namespace RosettaCTF.Authentication
{
    /// <summary>
    /// Represents the context of an authentication request.
    /// </summary>
    public sealed class AuthenticationContext
    {
        /// <summary>
        /// Gets the callback URL for the web application.
        /// </summary>
        public Uri CallbackUrl { get; }

        /// <summary>
        /// Gets the ID of the provider.
        /// </summary>
        public string ProviderId { get; }

        /// <summary>
        /// Creates a new authentication context.
        /// </summary>
        /// <param name="scheme">Current URI scheme.</param>
        /// <param name="host">Current hostname.</param>
        /// <param name="port">Current port.</param>
        /// <param name="providerId">ID of the authentication provider.</param>
        public AuthenticationContext(string scheme, string host, int port, string providerId)
        {
            this.CallbackUrl = new UriBuilder
            {
                Scheme = scheme,
                Host = host,
                Port = port,
                Path = "/session/callback"
            }.Uri;

            this.ProviderId = providerId;
        }
    }
}
