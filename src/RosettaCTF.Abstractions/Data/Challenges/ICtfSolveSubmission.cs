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

namespace RosettaCTF.Data
{
    /// <summary>
    /// Represents a solve submission.
    /// </summary>
    public interface ICtfSolveSubmission
    {
        /// <summary>
        /// Gets the contents of the submission.
        /// </summary>
        string Contents { get; }

        /// <summary>
        /// Gets whether the submission is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets the challenge the submission is for.
        /// </summary>
        ICtfChallenge Challenge { get; }

        /// <summary>
        /// Gets the user who submitted the challenge.
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// Gets the team which submitted the challenge.
        /// </summary>
        ITeam Team { get; }

        /// <summary>
        /// Gets the timestamp at which the submission was created.
        /// </summary>
        DateTimeOffset Timestamp { get; }
    }
}
