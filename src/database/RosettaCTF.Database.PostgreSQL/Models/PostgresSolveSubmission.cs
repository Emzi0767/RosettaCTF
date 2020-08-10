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
using RosettaCTF.Data;

namespace RosettaCTF.Models
{
    internal sealed class PostgresSolveSubmission : ICtfSolveSubmission
    {
        public long Id { get; set; }

        public string Contents { get; set; }

        public bool IsValid { get; set; }

        public string ChallengeId { get; set; }

        public PostgresChallenge ChallengeInternal { get; set; }

        public ICtfChallenge Challenge => this.ChallengeInternal;

        public long UserId { get; set; }

        public PostgresUser UserInternal { get; set; }

        public IUser User => this.UserInternal;

        public long TeamId { get; set; }

        public PostgresTeam TeamInternal { get; set; }

        public ITeam Team => this.TeamInternal;

        public DateTimeOffset Timestamp { get; set; }

        public int? Score { get; set; }
    }
}
