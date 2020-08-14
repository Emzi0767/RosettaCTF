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

namespace RosettaCTF.Models
{
    public sealed class ScoreboardEntryPreview
    {
        public ChallengePreview Challenge { get; }

        public TeamPreview Team { get; }

        public UserPreview User { get; }

        public int Score { get; }

        public int Ordinal { get; }

        public string TimeTaken { get; }

        public ScoreboardEntryPreview(TeamPreview team, int score, int ordinal, TimeSpan? elapsed)
        {
            this.Team = team;
            this.Score = score;
            this.Ordinal = ordinal;
            this.TimeTaken = elapsed?.ToHumanString();
        }

        public ScoreboardEntryPreview(ChallengePreview challenge, UserPreview user, int score, int ordinal, TimeSpan? elapsed)
        {
            this.Challenge = challenge;
            this.User = user;
            this.Score = score;
            this.Ordinal = ordinal;
            this.TimeTaken = elapsed?.ToHumanString();
        }
    }
}
