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

using System.Threading;
using System.Threading.Tasks;

namespace RosettaCTF.Data
{
    /// <summary>
    /// Contains methods for querying and manipulating OAuth states.
    /// </summary>
    public interface IOAuthStateRepository
    {
        /// <summary>
        /// Generates a new state string which will be validated once the authentication completes.
        /// </summary>
        /// <param name="remoteAddress">Address of the remote party.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>Generated state string.</returns>
        Task<string> GenerateStateAsync(string remoteAddress, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates a previously-issued state string.
        /// </summary>
        /// <param name="remoteAddress">Address of the remote party.</param>
        /// <param name="state">State to validate.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>Generated state string.</returns>
        Task<bool> ValidateStateAsync(string remoteAddress, string state, CancellationToken cancellationToken = default);
    }
}
