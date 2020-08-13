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

namespace RosettaCTF.Models
{
    /// <summary>
    /// Represents a preview version of a challenge category, that is, a simplified version, produced by the API for consumption.
    /// </summary>
    public sealed class ChallengeCategoryPreview
    {
        /// <summary>
        /// Gets the ID of this category.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the name of this category.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the challenges in this category.
        /// </summary>
        public IEnumerable<ChallengePreview> Challenges { get; }

        internal ChallengeCategoryPreview(ICtfChallengeCategory category, TimeSpan elapsed, EnumDisplayConverter enumCv, bool includeHidden)
            : this(category, elapsed, enumCv, includeHidden, null)
        { }

        internal ChallengeCategoryPreview(ICtfChallengeCategory category, TimeSpan elapsed, EnumDisplayConverter enumCv, bool includeHidden, IReadOnlyDictionary<string, int> scores)
        {
            this.Id = category.Id;
            this.Name = category.Name;
            this.Challenges = category.Challenges
                .Where(x => !x.IsHidden || includeHidden)
                .Select(x => scores != null && scores.TryGetValue(x.Id, out var score) ? new ChallengePreview(x, elapsed, enumCv, score) : new ChallengePreview(x, elapsed, enumCv))
                .ToList();
        }
    }
}
