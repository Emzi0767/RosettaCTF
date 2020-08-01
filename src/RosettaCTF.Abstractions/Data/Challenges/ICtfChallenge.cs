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
    /// Represents an individual CTF challenge, and any relevant data.
    /// </summary>
    public interface ICtfChallenge
    {
        /// <summary>
        /// Gets the ID of this challenge.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the title of this challenge.
        /// </summary>
        string Title { get; }
        
        /// <summary>
        /// Gets the category this challenge belongs to.
        /// </summary>
        ICtfChallengeCategory Category { get; }

        /// <summary>
        /// Gets the flag to expect for solving this challenge.
        /// </summary>
        string Flag { get; }

        /// <summary>
        /// Gets the difficulty of this challenge, as perceived by the author.
        /// </summary>
        CtfChallengeDifficulty Difficulty { get; }

        /// <summary>
        /// Gets the description of this challenge.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the author-provided hints for the challenge.
        /// </summary>
        IEnumerable<ICtfChallengeHint> Hints { get; }

        /// <summary>
        /// Gets the attachments for this challenge.
        /// </summary>
        IEnumerable<ICtfChallengeAttachment> Attachments { get; }

        /// <summary>
        /// Gets the endpoint the contestants will be connecting to as part of the challenge.
        /// </summary>
        ICtfChallengeEndpoint Endpoint { get; }

        /// <summary>
        /// Gets whether this challenge is hidden by default.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Gets the base score for this challenge.
        /// </summary>
        int BaseScore { get; }
    }
}
