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
using System.Collections.Generic;
using System.Linq;
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF.Services
{
    /// <summary>
    /// Contains methods for querying and manipulating abridged versions of challenges and categories.
    /// </summary>
    public sealed class ChallengePreviewRepository
    {
        private EnumDisplayConverter EnumDisplayConverter { get; }

        /// <summary>
        /// Creates a new repository.
        /// </summary>
        /// <param name="enumCv">Enum display value converter to use for converting display values.</param>
        public ChallengePreviewRepository(EnumDisplayConverter enumCv)
        {
            this.EnumDisplayConverter = enumCv;
        }

        /// <summary>
        /// Converts a <see cref="ICtfChallenge"/> to its abridged variant.
        /// </summary>
        /// <param name="challenge">Challenge to convert.</param>
        /// <param name="elapsed">Time elapsed since the beginning of the event.</param>
        /// <returns>An abridged challenge.</returns>
        public ChallengePreview GetChallenge(ICtfChallenge challenge, TimeSpan elapsed)
            => this.GetChallenge(challenge, elapsed, null);

        /// <summary>
        /// Converts a <see cref="ICtfChallenge"/> to its abridged variant.
        /// </summary>
        /// <param name="challenge">Challenge to convert.</param>
        /// <param name="elapsed">Time elapsed since the beginning of the event.</param>
        /// <param name="score">Current score for this challenge.</param>
        /// <returns>An abridged challenge.</returns>
        public ChallengePreview GetChallenge(ICtfChallenge challenge, TimeSpan elapsed, int? score)
            => new ChallengePreview(challenge, elapsed, this.EnumDisplayConverter, score);

        /// <summary>
        /// Converts a <see cref="ICtfChallengeCategory"/> to its abridged variant.
        /// </summary>
        /// <param name="category">Category to convert.</param>
        /// <param name="elapsed">Time elapsed since the beginning of the event.</param>
        /// <param name="includeHidden">Whether to include hidden challenges in the listing.</param>
        /// <returns>An abridged challenge category.</returns>
        public ChallengeCategoryPreview GetChallengeCategory(ICtfChallengeCategory category, TimeSpan elapsed, bool includeHidden)
            => this.GetChallengeCategory(category, elapsed, includeHidden, null);

        /// <summary>
        /// Converts a <see cref="ICtfChallengeCategory"/> to its abridged variant with specified scoring.
        /// </summary>
        /// <param name="category">Category to convert.</param>
        /// <param name="elapsed">Time elapsed since the beginning of the event.</param>
        /// <param name="includeHidden">Whether to include hidden challenges in the listing.</param>
        /// <param name="scores">Scores for the challenges herein.</param>
        /// <returns>An abridged challenge category.</returns>
        public ChallengeCategoryPreview GetChallengeCategory(ICtfChallengeCategory category, TimeSpan elapsed, bool includeHidden, IReadOnlyDictionary<string, int> scores)
            => !category.IsHidden || includeHidden
                ? new ChallengeCategoryPreview(category, elapsed, this.EnumDisplayConverter, includeHidden, scores)
                : null;

        /// <summary>
        /// Converts an enumerable of <see cref="ICtfChallengeCategory"/> to an enumerable of its abridged variants.
        /// </summary>
        /// <param name="categories">Enumerable of categories to convert.</param>
        /// <param name="elapsed">Time elapsed since the beginning of the event.</param>
        /// <param name="includeHidden">Whether to include hidden challenges and categories in the listing.</param>
        /// <returns>An enumerable of abridged challenge categories.</returns>
        public IEnumerable<ChallengeCategoryPreview> GetChallengeCategories(IEnumerable<ICtfChallengeCategory> categories, TimeSpan elapsed, bool includeHidden)
            => this.GetChallengeCategories(categories, elapsed, includeHidden, null);

        /// <summary>
        /// Converts an enumerable of <see cref="ICtfChallengeCategory"/> to an enumerable of its abridged variants with specified scoring.
        /// </summary>
        /// <param name="categories">Enumerable of categories to convert.</param>
        /// <param name="elapsed">Time elapsed since the beginning of the event.</param>
        /// <param name="includeHidden">Whether to include hidden challenges and categories in the listing.</param>
        /// <param name="scores">Scores for the challenges herein.</param>
        /// <returns>An enumerable of abridged challenge categories.</returns>
        public IEnumerable<ChallengeCategoryPreview> GetChallengeCategories(IEnumerable<ICtfChallengeCategory> categories, TimeSpan elapsed, bool includeHidden, IReadOnlyDictionary<string, int> scores)
            => categories.Where(x => !x.IsHidden || includeHidden)
                .Select(x => this.GetChallengeCategory(x, elapsed, includeHidden, scores))
                .ToList();
    }
}
