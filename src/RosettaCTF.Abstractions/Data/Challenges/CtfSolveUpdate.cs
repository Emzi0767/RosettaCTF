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
    /// Represents information for updating solve information.
    /// </summary>
    public struct CtfSolveUpdate
    {
        /// <summary>
        /// Gets the solve to update.
        /// </summary>
        public ICtfSolveSubmission Solve { get; }

        /// <summary>
        /// Gets the new score for the solve.
        /// </summary>
        public int? NewScore { get; }

        /// <summary>
        /// Creates new solve update info.
        /// </summary>
        /// <param name="solve">Solve to update.</param>
        /// <param name="newScore">Score to update with.</param>
        public CtfSolveUpdate(ICtfSolveSubmission solve, int? newScore)
        {
            this.Solve = solve;
            this.NewScore = newScore;
        }
    }
}
