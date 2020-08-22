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
using System.Threading.Tasks;
using RosettaCTF.Data;

namespace RosettaCTF.Models
{
    internal sealed class PostgresUserDecrypted : IUser
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public ulong DiscordId { get; set; }

        public Uri AvatarUrl { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public DateTimeOffset? TokenExpirationTime { get; set; }

        public bool IsAuthorized { get; set; }

        public bool HasHiddenAccess { get; set; }

        public ITeam Team { get; set; }

        public PostgresUserDecrypted(PostgresUser pgUser)
        {
            this.Id = pgUser.Id;
            this.Username = pgUser.Username;
            this.DiscordId = pgUser.DiscordId;
            this.AvatarUrl = pgUser.AvatarUrl;
            this.Token = pgUser.Token;
            this.RefreshToken = pgUser.RefreshToken;
            this.TokenExpirationTime = pgUser.TokenExpirationTime;
            this.IsAuthorized = pgUser.IsAuthorized;
            this.HasHiddenAccess = pgUser.HasHiddenAccess;
            this.Team = pgUser.Team;
        }

        public async Task<PostgresUserDecrypted> DecryptTokensAsync(OAuthTokenHandler tokenHandler)
        {
            if (this.Token != null)
                this.Token = await tokenHandler.DecryptAsync(this.Token);

            if (this.RefreshToken != null)
                this.RefreshToken = await tokenHandler.DecryptAsync(this.RefreshToken);

            return this;
        }
    }
}
