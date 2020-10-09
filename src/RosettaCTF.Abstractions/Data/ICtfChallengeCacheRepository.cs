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
    /// Provides methods for querying and manipulating cache data for challenges.
    /// </summary>
    public interface ICtfChallengeCacheRepository
    {
        /// <summary>
        /// Gets the number of participating teams that solved the baseline challenge.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The amount of teams that solved the baseline challenge.</returns>
        Task<int> GetBaselineSolveCountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Increments the number of teams that solved the baseline challenge.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The amount of teams that solved the baseline challenge.</returns>
        Task<int> IncrementBaselineSolveCountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stes the number of teams that solved the baseline challenge.
        /// </summary>
        /// <param name="count">Number of baseline solves to set.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task SetBaselineSolveCountAsync(int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current score of a challenge.
        /// </summary>
        /// <param name="challengeId">ID of the challenge to get current score for.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>Current point value of the challenge.</returns>
        Task<int> GetScoreAsync(string challengeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current score for specified challenges.
        /// </summary>
        /// <param name="challengeIds">IDs of challenges to get current score for.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>Mapping of ID to value.</returns>
        Task<IReadOnlyDictionary<string, int>> GetScoresAsync(IEnumerable<string> challengeIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the current score of a challenge.
        /// </summary>
        /// <param name="challengeId">ID of the challenge to update current score for.</param>
        /// <param name="score">New score.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task UpdateScoreAsync(string challengeId, int score, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the number of teams that solved the challenge.
        /// </summary>
        /// <param name="challengeId">ID of the challenge to get the solve count for.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>Number of teams which solved the challenge.</returns>
        Task<int> GetSolveCountAsync(string challengeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increments the number of teams that solved the challenge.
        /// </summary>
        /// <param name="challengeId">ID of the challenge to increment solve count for.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>Number of teams which solved the challenge.</returns>
        Task<int> IncrementSolveCountAsync(string challengeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes the cache with initial values, if necessary.
        /// </summary>
        /// <param name="baseScores">Dictionary of challenge ID to base score for all the challenges.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task InstallAsync(IDictionary<string, int> baseScores, CancellationToken cancellationToken = default);
    }
}
