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
    internal sealed class RedisOAuthStateRepository : IOAuthStateRepository
    {
        private const string OAuthKey = "oauth";
        private const string OAuthTokenKey = "token";
        private const string OAuthPrefix = "oauth/";

        private RedisClient Redis { get; }

        public RedisOAuthStateRepository(RedisClient redis)
        {
            this.Redis = redis;
        }

        public async Task<string> GenerateStateAsync(string remoteAddress, ActionToken actionToken, CancellationToken cancellationToken = default)
        {
            var state = Guid.NewGuid().ToString();
            var tkstring = actionToken.ExportString();

            await this.Redis.CreateTemporaryValueAsync(remoteAddress, TimeSpan.FromMinutes(2), OAuthKey, state);
            await this.Redis.CreateTemporaryValueAsync(tkstring, TimeSpan.FromMinutes(2), OAuthKey, state, OAuthTokenKey);
            return OAuthPrefix + state;
        }

        public async Task<ActionToken> ValidateStateAsync(string remoteAddress, string state, CancellationToken cancellationToken = default)
        {
            if (!state.AsSpan().StartsWith(OAuthPrefix))
                return null;

            var statestr = new string(state.AsSpan(OAuthPrefix.Length));

            var refAddr = await this.Redis.GetValueAsync<string>(OAuthKey, statestr);
            var srcTokn = await this.Redis.GetValueAsync<string>(OAuthKey, statestr, OAuthTokenKey);

            await this.Redis.DeleteValueAsync(OAuthKey, statestr);
            await this.Redis.DeleteValueAsync(OAuthKey, statestr, OAuthTokenKey);

            if (refAddr != remoteAddress || !ActionToken.TryParse(srcTokn, out var actionToken))
                return null;

            return actionToken;
        }
    }
}
