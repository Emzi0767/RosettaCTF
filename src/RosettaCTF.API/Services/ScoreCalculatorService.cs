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
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF.Services
{
    /// <summary>
    /// Handles score computations.
    /// </summary>
    public sealed class ScoreCalculatorService
    {
        private ICtfChallengeRepository ChallengeRepository { get; }
        private ICtfChallengeCacheRepository ChallengeCacheRepository { get; }
        private IScoringModel ScoringModel { get; }
        private ScoreLockService ScoreLockService { get; }

        public ScoreCalculatorService(
            ICtfChallengeRepository ctfChallengeRepository,
            ICtfChallengeCacheRepository ctfChallengeCacheRepository,
            IScoringModel scoringModel,
            ScoreLockService scoreLockService)
        {
            this.ChallengeRepository = ctfChallengeRepository;
            this.ChallengeCacheRepository = ctfChallengeCacheRepository;
            this.ScoringModel = scoringModel;
            this.ScoreLockService = scoreLockService;
        }

        public async Task<ScoreInfo> ComputeCurrentScoreAsync(ICtfChallenge challenge, CancellationToken cancellationToken = default)
        {
            using var _ = await this.ScoreLockService.AcquireLockAsync(challenge.Id, cancellationToken);

            var solves = await this.ChallengeCacheRepository.IncrementSolveCountAsync(challenge.Id, cancellationToken);
            var baseline = await this.ChallengeCacheRepository.GetBaselineSolveCountAsync(cancellationToken);
            var rate = solves / (double)baseline;
            var postRate = (solves + 1.0) / baseline;
            var cscore = this.ScoringModel.ComputeScore(challenge.BaseScore, rate);
            var pscore = this.ScoringModel.ComputeScore(challenge.BaseScore, postRate);

            await this.ChallengeCacheRepository.UpdateScoreAsync(challenge.Id, pscore, cancellationToken);
            return new ScoreInfo(cscore, pscore);
        }

        public async Task UpdateAllScoresAsync(bool freezeScores, bool includeBaseline, CancellationToken cancellationToken = default)
        {
            var baseline = includeBaseline
                ? await this.RecountBaselineAsync(cancellationToken)
                : await this.ChallengeCacheRepository.GetBaselineSolveCountAsync(cancellationToken);

            if (includeBaseline)
                await this.ChallengeCacheRepository.SetBaselineSolveCountAsync(baseline, cancellationToken);

            await this.UpdateAllScoresJeopardyAsync(baseline, cancellationToken);
            if (freezeScores)
                await this.UpdateAllScoresFreezerAsync(baseline, cancellationToken);
        }

        private async Task<int> RecountBaselineAsync(CancellationToken cancellationToken = default)
        {
            var solves = await this.ChallengeRepository.GetSuccessfulSolvesAsync(cancellationToken);
            return solves.Count(x => x.Challenge.BaseScore <= 1);
        }

        private async Task UpdateAllScoresJeopardyAsync(double baseline, CancellationToken cancellationToken = default)
        {
            var challenges = await this.ChallengeRepository.GetChallengesAsync(cancellationToken);
            challenges = challenges.Where(x => x.BaseScore > 1);
            var locks = await Task.WhenAll(challenges.Select(x => this.ScoreLockService.AcquireLockAsync(x.Id, cancellationToken)));

            try
            {
                foreach (var challenge in challenges)
                {
                    var solves = await this.ChallengeCacheRepository.GetSolveCountAsync(challenge.Id, cancellationToken);
                    var rate = solves / baseline;
                    var score = this.ScoringModel.ComputeScore(challenge.BaseScore, rate);
                    await this.ChallengeCacheRepository.UpdateScoreAsync(challenge.Id, score, cancellationToken);
                }
            }
            finally
            {
                foreach (var @lock in locks)
                    @lock.Dispose();
            }
        }

        private async Task UpdateAllScoresFreezerAsync(double baseline, CancellationToken cancellationToken = default)
        {
            var solves = await this.ChallengeRepository.GetSuccessfulSolvesAsync(cancellationToken);
            solves.Where(x => x.Challenge.BaseScore > 1);
            var locks = await Task.WhenAll(solves.Select(x => x.Challenge.Id)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(x => this.ScoreLockService.AcquireLockAsync(x, cancellationToken)));

            try
            {
                var updates = this.CreateUpdates(solves, baseline);
                await this.ChallengeRepository.UpdateSolvesAsync(updates, cancellationToken);


            }
            finally
            {
                foreach (var @lock in locks)
                    @lock.Dispose();
            }
        }

        private IEnumerable<CtfSolveUpdate> CreateUpdates(IEnumerable<ICtfSolveSubmission> solves, double baseline)
        {
            foreach (var solveGroup in solves.GroupBy(x => x.Challenge.Id, StringComparer.OrdinalIgnoreCase))
            {
                var i = 0;
                foreach (var solve in solveGroup)
                {
                    var rate = i++ / baseline;
                    var score = this.ScoringModel.ComputeScore(solve.Challenge.BaseScore, rate);
                    yield return new CtfSolveUpdate(solve, score);
                }
            }
        }
    }
}
