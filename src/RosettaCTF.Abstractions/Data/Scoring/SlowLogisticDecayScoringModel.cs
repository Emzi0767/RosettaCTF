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

namespace RosettaCTF.Data.Scoring
{
    /// <summary>
    /// Challenge scores decay in a manner similar to a sigmoid function (a logistic curve). This variant decays slowly in the beginning, then speeds up.
    /// </summary>
    public sealed class SlowLogisticDecayScoringModel : IScoringModel
    {
        // MATLAB curve fitter is a wonderful tool, pt.2
        int IScoringModel.ComputeScore(int baseScore, double solveRate)
            => (int)Math.Min(baseScore,
                Math.Max(Math.Ceiling(baseScore * 0.1),
                    baseScore * (1771 / (1964 + Math.Exp(25.28 * solveRate)) + 0.09977)));
    }
}
