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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF
{
    internal sealed class PostgresUserRepository : IUserRepository
    {
        private PostgresDbContext Database { get; }
        private DiscordTokenHandler TokenHandler { get; }

        public PostgresUserRepository(PostgresDbContext db, DiscordTokenHandler tokenHandler)
        {
            this.Database = db;
            this.TokenHandler = tokenHandler;
        }

        public async Task<IUser> GetUserAsync(long id, CancellationToken cancellationToken = default)
        {
            var user = await this.Database.Users
                .Include(x => x.TeamInternal)
                .ThenInclude(x => x.MembersInternal)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return user != null
                ? await new PostgresUserDecrypted(user).DecryptTokensAsync(this.TokenHandler)
                : null;
        }

        public async Task<IUser> CreateUserAsync(string username, ulong discordId, string token, string refreshToken, DateTimeOffset tokenExpiresAt, bool isAuthorized, CancellationToken cancellationToken = default)
        {
            if (token != null && refreshToken != null)
            {
                token = await this.TokenHandler.EncryptAsync(token);
                refreshToken = await this.TokenHandler.EncryptAsync(refreshToken);
            }
            else
            {
                token = null;
                refreshToken = null;
            }

            var user = new PostgresUser
            {
                Id = (long)discordId,
                Username = username,
                AvatarUrl = null,
                DiscordId = discordId,
                Token = token,
                RefreshToken = refreshToken,
                TokenExpirationTime = token != null && refreshToken != null
                    ? tokenExpiresAt as DateTimeOffset?
                    : null,
                IsAuthorized = isAuthorized,
                HasHiddenAccess = false
            };

            await this.Database.Users.AddAsync(user, cancellationToken);
            await this.Database.SaveChangesAsync(cancellationToken);

            return await new PostgresUserDecrypted(user).DecryptTokensAsync(this.TokenHandler);
        }

        public async Task DeleteUserAsync(long id, CancellationToken cancellationToken = default)
        {
            var user = await this.Database.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
                return;

            this.Database.Users.Remove(user);
            await this.Database.SaveChangesAsync(cancellationToken);
        }

        public async Task<ITeam> GetTeamAsync(long id, CancellationToken cancellationToken = default)
            => await this.Database.Teams
                .Include(x => x.MembersInternal)
                .ThenInclude(x => x.TeamInternal)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<ITeam> CreateTeamAsync(string name, CancellationToken cancellationToken = default)
        {
            var nid = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() << 22;
            var rng = new Random();
            nid |= rng.Next(1, int.MaxValue) % (1L << 22);

            var team = new PostgresTeam
            {
                Id = nid,
                Name = name,
                AvatarUrl = null
            };
            await this.Database.Teams.AddAsync(team, cancellationToken);
            await this.Database.SaveChangesAsync(cancellationToken);

            return team;
        }

        public async Task DeleteTeamAsync(long id, CancellationToken cancellationToken = default)
        {
            var team = await this.Database.Teams.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (team == null)
                return;

            this.Database.Teams.Remove(team);
            await this.Database.SaveChangesAsync(cancellationToken);
        }

        public async Task AssignTeamMembershipAsync(long userId, long? teamId, CancellationToken cancellationToken = default)
        {
            var user = await this.Database.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user == null)
                return;

            var team = teamId != null
                ? await this.Database.Teams.FirstOrDefaultAsync(x => x.Id == teamId, cancellationToken)
                : null;

            user.TeamInternal = team;
            this.Database.Users.Update(user);
            await this.Database.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateTokensAsync(long userId, string token, string refreshToken, DateTimeOffset tokenExpiresAt, CancellationToken cancellationToken = default)
        {
            if (token != null && refreshToken != null)
            {
                token = await this.TokenHandler.EncryptAsync(token);
                refreshToken = await this.TokenHandler.EncryptAsync(refreshToken);
            }
            else
            {
                token = null;
                refreshToken = null;
            }

            var user = await this.Database.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user == null)
                return;

            user.Token = token;
            user.RefreshToken = refreshToken;
            user.TokenExpirationTime = token != null && refreshToken != null
                ? tokenExpiresAt as DateTimeOffset?
                : null;

            this.Database.Users.Update(user);
            await this.Database.SaveChangesAsync(cancellationToken);
        }

        public async Task EnableHiddenChallengesAsync(long userId, bool enable, CancellationToken cancellationToken = default)
        {
            var user = await this.Database.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user == null)
                return;

            user.HasHiddenAccess = enable;

            this.Database.Users.Update(user);
            await this.Database.SaveChangesAsync(cancellationToken);
        }
    }
}
