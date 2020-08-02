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
    /// Challenge scores decay in a linear fashion.
    /// </summary>
    public sealed class LinearDecayScoringModel : IScoringModel
    {
        int IScoringModel.ComputeScore(int baseScore, double solveRate)
            => (int)Math.Round(Math.Max(Math.Ceiling(baseScore * 0.1), (solveRate * -1.8 + 1.0) * baseScore), MidpointRounding.AwayFromZero);
    }
}
