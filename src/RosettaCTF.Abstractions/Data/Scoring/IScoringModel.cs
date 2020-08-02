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

namespace RosettaCTF.Data.Scoring
{
    /// <summary>
    /// Defines a basic interface for computing the score of a challenge.
    /// </summary>
    public interface IScoringModel
    {
        /// <summary>
        /// Computes the current score of a challenge, based on the solve rate.
        /// </summary>
        /// <param name="baseScore">Base score of the challenge.</param>
        /// <param name="solveRate">Percentage of participants who solved the challenge.</param>
        /// <returns>The computed score.</returns>
        int ComputeScore(int baseScore, double solveRate);
    }
}
