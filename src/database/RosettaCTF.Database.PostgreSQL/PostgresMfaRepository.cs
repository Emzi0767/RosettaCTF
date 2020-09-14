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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RosettaCTF.Authentication;
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF
{
    internal sealed class PostgresMfaRepository : IMfaRepository
    {
        private PostgresDbContext Database { get; }
        
        public PostgresMfaRepository(PostgresDbContext db)
        {
            this.Database = db;
        }
        public async Task<IMultiFactorSettings> GetMfaSettingsAsync(long userId, CancellationToken cancellationToken = default)
            => await this.Database.MfaSettings.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        public async Task<IMultiFactorSettings> ConfigureMfaAsync(
            long userId, 
            byte[] secret, 
            int digits, 
            MultiFactorHmac hmac, 
            int period, 
            MultiFactorType type, 
            byte[] additional, 
            long recoveryBase, 
            CancellationToken cancellationToken = default)
        {
            var mfa = new PostgresMfaSettings
            {
                UserId = userId,
                Secret = secret,
                Digits = digits,
                HmacAlgorithm = hmac,
                Period = period,
                Additional = additional,
                Type = type,
                RecoveryCounterBase = recoveryBase,
                RecoveryTripCount = 0
            };

            try
            {
                await this.Database.MfaSettings.AddAsync(mfa, cancellationToken);
                await this.Database.SaveChangesAsync(cancellationToken);
            }
            catch { return null; }

            return mfa;
        }

        public async Task RemoveMfaAsync(long userId, CancellationToken cancellationToken = default)
        {
            var mfa = await this.Database.MfaSettings.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (mfa != null)
            {
                this.Database.MfaSettings.Remove(mfa);
                await this.Database.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task TripRecoveryCodeAsync(long userId, CancellationToken cancellationToken = default)
        {
            var mfa = await this.Database.MfaSettings.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (mfa != null)
            {
                mfa.RecoveryTripCount++;
                await this.Database.SaveChangesAsync();
            }
        }
    }
}
