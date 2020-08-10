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

namespace RosettaCTF.Models
{
    /// <summary>
    /// Represents a preview version of a challenge category, that is, a simplified version, produced by the API for consumption.
    /// </summary>
    public interface IChallengeCategoryPreview
    {
        /// <summary>
        /// Gets the ID of this category.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the name of this category.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the challenges in this category.
        /// </summary>
        IEnumerable<IChallengePreview> Challenges { get; }
    }
}
