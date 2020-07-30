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
        /// <returns>An enumerable of <see cref="ICtfChallengeCategory"/> instances.</returns>
        IEnumerable<ICtfChallengeCategory> GetCategories();

        /// <summary>
        /// Gets a specified challenge category by ID.
        /// </summary>
        /// <param name="id">ID of the category to retrieve.</param>
        /// <returns>Requested challenge category or <see langword="null"/> if the category does not exist.</returns>
        ICtfChallengeCategory GetCategory(string id);

        /// <summary>
        /// Gets all available challenges.
        /// </summary>
        /// <returns>An enumerable of <see cref="ICtfChallenge"/> instances.</returns>
        IEnumerable<ICtfChallenge> GetChallenges();

        /// <summary>
        /// Gets a specified challenge by ID.
        /// </summary>
        /// <param name="id">ID of the challenge to retrieve.</param>
        /// <returns>Requested challenge or <see langword="null"/> if the category does not exist.</returns>
        ICtfChallenge GetChallenge(string id);

        // TODO: Context

        /// <summary>
        /// Gets all challenge category previews available in specified context.
        /// </summary>
        /// <returns>An enumerable of <see cref="IChallengeCategoryPreview"/> instances available in given context.</returns>
        IEnumerable<IChallengeCategoryPreview> GetCategoryPreviews();

        /// <summary>
        /// Gets a preview of specified challenge category by ID in specified context.
        /// </summary>
        /// <param name="id">ID of the challenge category to retrieve.</param>
        /// <returns>Requested challenge category preview or <see langword="null"/> if the category does not exist or is not available in the given context.</returns>
        IChallengeCategoryPreview GetCategoryPreview(string id);

        /// <summary>
        /// Gets previews of all available challenges in specified context.
        /// </summary>
        /// <returns>An enumerable of <see cref="IChallengePreview"/> instances available in given context.</returns>
        IEnumerable<IChallengePreview> GetChallengePreviews();

        /// <summary>
        /// Gets a preview of specified challenge by ID in specified context.
        /// </summary>
        /// <param name="id">ID of the challenge to retrieve.</param>
        /// <returns>Requested challenge preview or <see langword="null"/> if the challenge does not exist or is not available in the given context.</returns>
        IChallengePreview GetChallengePreview(string id);
    }
}
