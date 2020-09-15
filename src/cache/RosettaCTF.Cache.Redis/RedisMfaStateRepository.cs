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
using RosettaCTF.Data;

namespace RosettaCTF
{
    internal sealed class RedisMfaStateRepository : IMfaStateRepository
    {
        private const string MfaKey = "mfa";
        private const string MfaTokenKey = "token";
        private const string MfaPrefix = "mfa/";

        private RedisClient Redis { get; }

        public RedisMfaStateRepository(RedisClient redis)
        {
            this.Redis = redis;
        }

        public async Task<string> GenerateStateAsync(string remoteAddress, ActionToken serverToken, CancellationToken cancellationToken = default)
        {
            var state = Guid.NewGuid().ToString();
            var tkstring = serverToken.ExportString();

            await this.Redis.CreateTemporaryValueAsync(remoteAddress, TimeSpan.FromMinutes(2), MfaKey, state);
            await this.Redis.CreateTemporaryValueAsync(tkstring, TimeSpan.FromMinutes(2), MfaKey, state, MfaTokenKey);
            return MfaPrefix + state;
        }

        public async Task<ActionToken> ValidateStateAsync(string remoteAddress, string state, CancellationToken cancellationToken = default)
        {
            if (!state.AsSpan().StartsWith(MfaPrefix))
                return null;

            var statestr = new string(state.AsSpan(MfaPrefix.Length));

            var refAddr = await this.Redis.GetValueAsync<string>(MfaKey, statestr);
            var srcTokn = await this.Redis.GetValueAsync<string>(MfaKey, statestr, MfaTokenKey);

            await this.Redis.DeleteValueAsync(MfaKey, statestr);
            await this.Redis.DeleteValueAsync(MfaKey, statestr, MfaTokenKey);

            if (refAddr != remoteAddress || !ActionToken.TryParse(srcTokn, out var actionToken))
                return null;

            return actionToken;
        }
    }
}
