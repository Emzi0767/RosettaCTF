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
    /// Represents an endpoint the contestants connect to as part of the challenge.
    /// </summary>
    public interface ICtfChallengeEndpoint
    {
        /// <summary>
        /// Gets the type of the challenge endpoint.
        /// </summary>
        CtfChallengeEndpointType Type { get; }

        /// <summary>
        /// Gets the hostname for the challenge endpoint.
        /// </summary>
        string Hostname { get; }

        /// <summary>
        /// Gets the port for the challenge endpoint.
        /// </summary>
        int Port { get; }
    }
}
