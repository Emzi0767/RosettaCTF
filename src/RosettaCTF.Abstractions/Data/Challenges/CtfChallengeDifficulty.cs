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
    /// Represents an author-perceived difficulty of the challenge.
    /// </summary>
    public enum CtfChallengeDifficulty : int
    {
        /// <summary>
        /// Difficulty is not set, or challenge has no actual difficulty.
        /// </summary>
        [EnumDisplayName("None")]
        None = 0,

        /// <summary>
        /// Challenge is very easy. Estimated ~99% of active contestants will solve it.
        /// </summary>
        [EnumDisplayName("Very easy")]
        VeryEasy = 1,

        /// <summary>
        /// Challenge is easy. Estimated ~80% of active contentants will solve it.
        /// </summary>
        [EnumDisplayName("Easy")]
        Easy = 2,

        /// <summary>
        /// Challenge is moderately difficult. Estimated ~50% of active contestants will solve it.
        /// </summary>
        [EnumDisplayName("Medium")]
        Medium = 3,

        /// <summary>
        /// Challenge is hard. Estimated ~25% of active contestants will solve it.
        /// </summary>
        [EnumDisplayName("Hard")]
        Hard = 4,

        /// <summary>
        /// Challenge is very hard. Estimated ~5% of active contestants will solve it.
        /// </summary>
        [EnumDisplayName("Very hard")]
        VeryHard = 5,

        /// <summary>
        /// Challenge is hellishly difficult. Estimated <1% of active contestants will solve it.
        /// </summary>
        [EnumDisplayName("Ultra Nightmare")]
        UltraNightmare = 6
    }
}
