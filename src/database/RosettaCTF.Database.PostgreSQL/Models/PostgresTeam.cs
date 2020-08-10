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
using System.Collections.Generic;
using RosettaCTF.Data;

namespace RosettaCTF.Models
{
    internal sealed class PostgresTeam : ITeam
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string AvatarUrlInternal { get; set; }

        public Uri AvatarUrl 
        {
            get => this.AvatarUrlInternal != null ? new Uri(this.AvatarUrlInternal) : null;
            set => this.AvatarUrlInternal = value?.ToString();
        }

        public IEnumerable<PostgresUser> MembersInternal { get; set; }

        public IEnumerable<IUser> Members => this.MembersInternal;

        public IEnumerable<PostgresSolveSubmission> SolvesInternal { get; set; }

        public IEnumerable<PostgresTeamInvite> InvitesInternal { get; set; }
    }
}
