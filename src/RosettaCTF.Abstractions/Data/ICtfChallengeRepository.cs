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
using System.Threading;
using System.Threading.Tasks;

namespace RosettaCTF.Data
{
    /// <summary>
    /// Provides an abstraction over methods for querying and manipulating challenges.
    /// </summary>
    public interface ICtfChallengeRepository
    {
        /// <summary>
        /// Gets all available challenge categories.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>An enumerable of <see cref="ICtfChallengeCategory"/> instances.</returns>
        Task<IEnumerable<ICtfChallengeCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a specified challenge category by ID.
        /// </summary>
        /// <param name="id">ID of the category to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>Requested challenge category or <see langword="null"/> if the category does not exist.</returns>
        Task<ICtfChallengeCategory> GetCategoryAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all available challenges.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>An enumerable of <see cref="ICtfChallenge"/> instances.</returns>
        Task<IEnumerable<ICtfChallenge>> GetChallengesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a specified challenge by ID.
        /// </summary>
        /// <param name="id">ID of the challenge to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>Requested challenge or <see langword="null"/> if the category does not exist.</returns>
        Task<ICtfChallenge> GetChallengeAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs first-time installation steps for all challenges.
        /// </summary>
        /// <param name="categories">Categories to install.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task InstallAsync(IEnumerable<ICtfChallengeCategory> categories, CancellationToken cancellationToken = default);
    }
}
