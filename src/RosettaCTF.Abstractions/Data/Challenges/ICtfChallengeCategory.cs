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
    /// Represents a CTF challenge category, which contains several challenges, in form of <see cref="ICtfChallenge"/> instances.
    /// </summary>
    public interface ICtfChallengeCategory
    {
        /// <summary>
        /// Gets the name of this category.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the ID of this category.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets whether this category is hidden by default.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Gets the ordinality of this category.
        /// </summary>
        int Ordinality { get; }

        /// <summary>
        /// Gets the challenges contained within this category.
        /// </summary>
        IEnumerable<ICtfChallenge> Challenges { get; }
    }
}
