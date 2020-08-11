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

namespace RosettaCTF.Data
{
    /// <summary>
    /// Specifies the type of endpoint the contestants will be connecting to.
    /// </summary>
    public enum CtfChallengeEndpointType : int
    {
        /// <summary>
        /// The type of the endpoint is not recognized.
        /// </summary>
        [EnumDisplayName("Unknown endpoint type")]
        Unknown = 0,

        /// <summary>
        /// The endpoint is a plain TCP endpoint.
        /// </summary>
        [EnumDisplayName("TCP/netcat")]
        Netcat = 1,

        /// <summary>
        /// The endpoint is an HTTP endpoint.
        /// </summary>
        [EnumDisplayName("HTTP")]
        Http = 2,

        /// <summary>
        /// The endpoint is an SSH endpoint.
        /// </summary>
        [EnumDisplayName("SSH")]
        Ssh = 3,

        /// <summary>
        /// The endpoint is a SSL/TLS over TCP endpoint.
        /// </summary>
        [EnumDisplayName("SSL")]
        Ssl = 4,

        /// <summary>
        /// The endpoint is an HTTPS endpoint.
        /// </summary>
        [EnumDisplayName("HTTPS")]
        Https = 5,
    }
}
