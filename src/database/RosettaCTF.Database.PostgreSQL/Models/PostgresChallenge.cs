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
using RosettaCTF.Data;

namespace RosettaCTF.Models
{
    internal sealed class PostgresChallenge : ICtfChallenge
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string CategoryId { get; set; }

        public PostgresChallengeCategory CategoryInternal { get; set; }

        public ICtfChallengeCategory Category => this.CategoryInternal;

        public string Flag { get; set; }

        public CtfChallengeDifficulty Difficulty { get; set; }

        public string Description { get; set; }

        public IEnumerable<PostgresChallengeHint> HintsInternal { get; set; }

        public IEnumerable<ICtfChallengeHint> Hints => this.HintsInternal;

        public IEnumerable<PostgresChallengeAttachment> AttachmentsInternal { get; set; }

        public IEnumerable<ICtfChallengeAttachment> Attachments => this.AttachmentsInternal;

        public long? EndpointId { get; set; }

        public PostgresChallengeEndpoint EndpointInternal { get; set; }

        public ICtfChallengeEndpoint Endpoint { get; set; }

        public bool IsHidden { get; set; }

        public int BaseScore { get; set; }
    }
}
