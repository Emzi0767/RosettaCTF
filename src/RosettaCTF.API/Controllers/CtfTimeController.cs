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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RosettaCTF.Data;
using RosettaCTF.Filters;
using RosettaCTF.Models;
using RosettaCTF.Services;

namespace RosettaCTF.Controllers
{
    [Route("api/[controller]"), AllowAnonymous]
    [ServiceFilter(typeof(EventStartedFilter))]
    public sealed class CtfTimeController : RosettaControllerBase
    {
        private ICtfChallengeRepository ChallengeRepository { get; }
        private ICtfChallengeCacheRepository ChallengeCacheRepository { get; }
        private ScoreCalculatorService ScoreCalculator { get; }

        public CtfTimeController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            UserPreviewRepository userPreviewRepository,
            ICtfConfigurationLoader ctfConfigurationLoader,
            ICtfChallengeRepository challengeRepository,
            ICtfChallengeCacheRepository ctfChallengeCacheRepository)
            : base(loggerFactory, userRepository, userPreviewRepository, ctfConfigurationLoader)
        {
            this.ChallengeRepository = challengeRepository;
            this.ChallengeCacheRepository = ctfChallengeCacheRepository;
        }

        [HttpGet]
        [Route("scoreboard")]
        
        public async Task<ActionResult<CtfTimeScoreboard>> Scoreboard(CancellationToken cancellationToken = default)
        {
            var solves = await this.ChallengeRepository.GetSuccessfulSolvesAsync(cancellationToken);
            var challenges = await this.ChallengeRepository.GetChallengesAsync(cancellationToken);
            var points = await this.ChallengeCacheRepository.GetScoresAsync(challenges.Select(x => x.Id), cancellationToken);

            var scoreboard = new CtfTimeScoreboard
            {
                Tasks = challenges.Select(x => x.Title),
                Standings = this.CreateStandings(solves.GroupBy(x => x.Team), points)
                    .OrderByDescending(x => x.Score)
                    .ThenBy(x => x.LastAccept)
                    .Select((x, i) => { x.Pos = i + 1; return x; })
            };
            return this.Ok(scoreboard);
        }

        [HttpGet]
        [Route("feed")]
        public async Task<ActionResult<IEnumerable<CtfTimeEventFeedEntry>>> Scoreboard([FromQuery] long lastId = -1, CancellationToken cancellationToken = default)
        {
            var solves = await this.ChallengeRepository.GetAllSolvesAsync(lastId, cancellationToken);
            var feed = solves.Select(x => new CtfTimeEventFeedEntry
            {
                Id = x.Ordinal,
                Task = x.Challenge.Title,
                Team = x.Team.Name,
                Time = x.Timestamp.ToUnixTimeSeconds(),
                Type = x.IsValid ? CtfTimeEventFeedEntry.FlagCorrect : CtfTimeEventFeedEntry.FlagWrong
            });

            return this.Ok(feed);
        }

        private IEnumerable<CtfTimeStanding> CreateStandings(IEnumerable<IGrouping<ITeam, ICtfSolveSubmission>> solveGroups, IReadOnlyDictionary<string, int> points)
        {
            foreach (var solveGroup in solveGroups)
            {
                var solves = solveGroup.Select(x => new 
                { 
                    score = x.Score ?? points[x.Challenge.Id], 
                    title = x.Challenge.Title,
                    accepted = x.Timestamp.ToUnixTimeSeconds()
                });

                var standing = new CtfTimeStanding
                {
                    Team = solveGroup.Key.Name,
                    LastAccept = solveGroup.Max(x => x.Timestamp).ToUnixTimeSeconds(),
                    Score = solveGroup.Select(x => x.Score ?? points[x.Challenge.Id]).Sum(),
                    TaskStats = solves.ToDictionary(x => x.title, x => new CtfTimeTaskEntryInfo { Points = x.score, Time = x.accepted })
                };

                yield return standing;
            }
        }
    }
}
