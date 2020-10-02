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
using System.Linq;
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
        private OAuthTokenHandler TokenHandler { get; }
        private IdGenerator IdGenerator { get; }

        public PostgresUserRepository(PostgresDbContext db, OAuthTokenHandler tokenHandler, IdGenerator idgen)
        {
            this.Database = db;
            this.TokenHandler = tokenHandler;
            this.IdGenerator = idgen;
        }

        public async Task<IUser> GetUserAsync(long id, CancellationToken cancellationToken = default)
            => await this.Database.Users
                .Include(x => x.TeamInternal)
                    .ThenInclude(x => x.MembersInternal)
                    .ThenInclude(x => x.CountryInternal)
                .Include(x => x.ConnectedAccountsInternal)
                .Include(x => x.CountryInternal)
                .Include(x => x.MfaInternal)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IUser> GetUserAsync(string username, CancellationToken cancellationToken = default)
            => await this.Database.Users
                .Include(x => x.TeamInternal)
                    .ThenInclude(x => x.MembersInternal)
                    .ThenInclude(x => x.CountryInternal)
                .Include(x => x.ConnectedAccountsInternal)
                .Include(x => x.CountryInternal)
                .Include(x => x.MfaInternal)
                .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);

        public async Task<IUser> CreateUserAsync(string username, bool isAuthorized, CancellationToken cancellationToken = default)
        {
            var user = new PostgresUser
            {
                Id = this.IdGenerator.Generate(),
                Username = username,
                AvatarUrl = null,
                IsAuthorized = isAuthorized
            };

            try
            {
                await this.Database.Users.AddAsync(user, cancellationToken);
                await this.Database.SaveChangesAsync(cancellationToken);

                return user;
            }
            catch { return null; }
        }

        public async Task DeleteUserAsync(long id, CancellationToken cancellationToken = default)
        {
            var user = await this.Database.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
                return;

            this.Database.Users.Remove(user);
            await this.Database.SaveChangesAsync(cancellationToken);
        }

        public async Task<IUser> UpdateUserCountryAsync(long id, string code, CancellationToken cancellationToken = default)
        {
            var user = await this.Database.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user == null)
                return null;

            if (code == null)
            {
                user.CountryCode = null;
            }
            else
            {
                var country = await this.Database.Countries.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
                if (country == null)
                    return null;

                user.CountryCode = country.Code;
            }

            this.Database.Users.Update(user);
            await this.Database.SaveChangesAsync(cancellationToken);

            return user;
        }

        public async Task<ITeam> GetTeamAsync(long id, CancellationToken cancellationToken = default)
            => await this.Database.Teams
                .Include(x => x.MembersInternal).ThenInclude(x => x.CountryInternal)
                .Include(x => x.MembersInternal).ThenInclude(x => x.TeamInternal)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<ITeam> CreateTeamAsync(string name, CancellationToken cancellationToken = default)
        {
            var team = new PostgresTeam
            {
                Id = this.IdGenerator.Generate(),
                Name = name,
                AvatarUrl = null
            };

            try
            {
                await this.Database.Teams.AddAsync(team, cancellationToken);
                await this.Database.SaveChangesAsync(cancellationToken);

                return team;
            }
            catch { return null; }
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

        public async Task UpdateUserPasswordAsync(long userId, byte[] password, CancellationToken cancellationToken = default)
        {
            var pwd = await this.Database.UserPasswords.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
            if (pwd != null)
            {
                if (password != null)
                {
                    pwd.PasswordHash = password;
                    this.Database.UserPasswords.Update(pwd);
                }
                else
                {
                    this.Database.UserPasswords.Remove(pwd);
                }
            }
            else
            {
                pwd = new PostgresUserPassword
                {
                    PasswordHash = password,
                    UserId = userId
                };
                await this.Database.UserPasswords.AddAsync(pwd, cancellationToken);
            }

            await this.Database.SaveChangesAsync(cancellationToken);
        }

        public async Task<byte[]> GetUserPasswordAsync(long userId, CancellationToken cancellationToken = default)
        {
            var pwd = await this.Database.UserPasswords.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
            return pwd?.PasswordHash;
        }

        public async Task<IExternalUser> ConnectExternalAccountAsync(long userId, string extId, string extName, string providerId, CancellationToken cancellationToken = default)
        {
            var extUser = new PostgresExternalUser
            {
                Id = extId,
                Username = extName,
                ProviderId = providerId,
                UserId = userId
            };

            try
            {
                await this.Database.ConnectedAccounts.AddAsync(extUser, cancellationToken);
                await this.Database.SaveChangesAsync(cancellationToken);

                return extUser;
            }
            catch { return null; }
        }

        public async Task RemoveExternalAccountAsync(long userId, string providerId, CancellationToken cancellationToken = default)
        {
            var extUser = await this.Database.ConnectedAccounts.FirstOrDefaultAsync(x => x.UserId == userId && x.ProviderId == providerId, cancellationToken);
            if (extUser == null)
                return;

            this.Database.ConnectedAccounts.Remove(extUser);
            await this.Database.SaveChangesAsync(cancellationToken);
        }

        public async Task<IExternalUser> GetExternalAccountAsync(long userId, string providerId, CancellationToken cancellationToken = default)
        {
            var extUser = await this.Database.ConnectedAccounts
                .Include(x => x.UserInternal)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProviderId == providerId, cancellationToken);
            if (extUser == null)
                return null;

            return await new PostgresExternalUserDecrypted(extUser).DecryptTokensAsync(this.TokenHandler);
        }

        public async Task<IExternalUser> GetExternalAccountAsync(string id, string providerId, CancellationToken cancellationToken = default)
        {
            var extUser = await this.Database.ConnectedAccounts
                .Include(x => x.UserInternal).ThenInclude(x => x.CountryInternal)
                .FirstOrDefaultAsync(x => x.Id == id && x.ProviderId == providerId, cancellationToken);
            if (extUser == null)
                return null;

            return await new PostgresExternalUserDecrypted(extUser).DecryptTokensAsync(this.TokenHandler);
        }

        public async Task<IEnumerable<IExternalUser>> GetExternalAccountsAsync(long userId, CancellationToken cancellationToken = default)
            => await this.Database.ConnectedAccounts
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);

        public async Task UpdateTokensAsync(long userId, string providerId, string token, string refreshToken, DateTimeOffset tokenExpiresAt, CancellationToken cancellationToken = default)
        {
            token = token != null
                ? await this.TokenHandler.EncryptAsync(token)
                : null;

            refreshToken = refreshToken != null
                ? await this.TokenHandler.EncryptAsync(refreshToken)
                : null;

            var extUser = await this.Database.ConnectedAccounts.FirstOrDefaultAsync(x => x.UserId == userId && x.ProviderId == providerId, cancellationToken);
            if (extUser == null)
                return;

            extUser.Token = token;
            extUser.RefreshToken = refreshToken;
            extUser.TokenExpirationTime = token != null && refreshToken != null
                ? tokenExpiresAt as DateTimeOffset?
                : null;

            this.Database.ConnectedAccounts.Update(extUser);
            await this.Database.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<ITeamInvite>> GetTeamInvitesAsync(long userId, CancellationToken cancellationToken = default)
            => await this.Database.TeamInvites
                .Include(x => x.TeamInternal)
                .Include(x => x.UserInternal)
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);

        public async Task<ITeamInvite> GetTeamInviteAsync(long userId, long teamId, CancellationToken cancellationToken = default)
            => await this.Database.TeamInvites
                .Include(x => x.TeamInternal)
                .Include(x => x.UserInternal)
                .SingleOrDefaultAsync(x => x.UserId == userId && x.TeamId == teamId, cancellationToken);

        public async Task ClearTeamInvitesAsync(long userId, CancellationToken cancellationToken = default)
        {
            var invites = this.Database.TeamInvites
                .Where(x => x.UserId == userId);

            this.Database.TeamInvites.RemoveRange(invites);
            await this.Database.SaveChangesAsync(cancellationToken);
        }

        public async Task<ITeamInvite> CreateTeamInviteAsync(long userId, long teamId, CancellationToken cancellationToken = default)
        {
            var invite = new PostgresTeamInvite
            {
                TeamId = teamId,
                UserId = userId
            };
            await this.Database.TeamInvites.AddAsync(invite, cancellationToken);
            await this.Database.SaveChangesAsync(cancellationToken);

            return await this.Database.TeamInvites.SingleOrDefaultAsync(x => x.UserId == userId && x.TeamId == teamId, cancellationToken);
        }

        public async Task<int> GetBaselineSolveCount(CancellationToken cancellationToken = default)
        {
            var baseline = await this.Database.Challenges
                .Include(x => x.SolvesInternal)
                .SingleOrDefaultAsync(x => x.BaseScore == 1, default);
            if (baseline == null)
                return -1;

            return baseline.SolvesInternal.Count(x => x.IsValid);
        }

        public async Task<IEnumerable<ICountry>> GetAllCountriesAsync(CancellationToken cancellationToken = default)
            => await this.Database.Countries
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
    }
}
