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
using System.Threading;
using System.Threading.Tasks;
using RosettaCTF.Data;

namespace RosettaCTF
{
    internal sealed class RedisChallengeCacheRepository : ICtfChallengeCacheRepository
    {
        private const string BaselineKey = "baseline";
        private const string SolvesKey = "solves";
        private const string ChallengesKey = "challenge";
        private const string ScoreKey = "score";

        private RedisClient Redis { get; }

        public RedisChallengeCacheRepository(RedisClient redis)
        {
            this.Redis = redis;
        }

        public async Task<int> GetBaselineSolveCountAsync(CancellationToken cancellationToken = default)
            => await this.Redis.GetValueAsync<int>(BaselineKey, SolvesKey);

        public async Task<int> IncrementBaselineSolveCountAsync(CancellationToken cancellationToken = default)
            => (int)await this.Redis.IncrementValueAsync(BaselineKey, SolvesKey);

        public async Task<int> GetScoreAsync(string challengeId, CancellationToken cancellationToken = default)
            => await this.Redis.GetValueAsync<int>(ChallengesKey, challengeId, ScoreKey);

        public async Task<IReadOnlyDictionary<string, int>> GetScoresAsync(IEnumerable<string> challengeIds, CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<string, int>();
            foreach (var chid in challengeIds)
                dict[chid] = await this.Redis.GetValueAsync<int>(ChallengesKey, chid, ScoreKey);

            return dict;
        }

        public async Task UpdateScoreAsync(string challengeId, int score, CancellationToken cancellationToken = default)
            => await this.Redis.SetValueAsync(score, ChallengesKey, challengeId, ScoreKey);

        public async Task<int> GetSolveCountAsync(string challengeId, CancellationToken cancellationToken = default)
            => await this.Redis.GetValueAsync<int>(ChallengesKey, challengeId, SolvesKey);

        public async Task<int> IncrementSolveCountAsync(string challengeId, CancellationToken cancellationToken = default)
            => (int)await this.Redis.IncrementValueAsync(ChallengesKey, challengeId, SolvesKey);

        public async Task InstallAsync(IDictionary<string, int> baseScores, CancellationToken cancellationToken = default)
        {
            await this.Redis.CreateValueAsync(0, BaselineKey, SolvesKey);

            foreach (var (k, v) in baseScores)
                await this.Redis.CreateValueAsync(v, ChallengesKey, k, ScoreKey);
        }
    }
}
