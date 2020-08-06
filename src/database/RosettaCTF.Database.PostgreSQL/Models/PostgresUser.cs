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
    internal sealed class PostgresUser : IUser
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public long DiscordIdInternal { get; set; }

        public ulong DiscordId
        {
            get => (ulong)this.DiscordIdInternal;
            set => this.DiscordIdInternal = (long)value;
        }

        public Uri AvatarUrl { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public DateTimeOffset? TokenExpirationTime { get; set; }

        public long? TeamId { get; set; }

        public PostgresTeam TeamInternal { get; set; }

        public ITeam Team => this.TeamInternal;
    }
}
