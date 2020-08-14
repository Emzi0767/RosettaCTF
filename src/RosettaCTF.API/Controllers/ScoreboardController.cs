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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RosettaCTF.Data;
using RosettaCTF.Filters;
using RosettaCTF.Models;
using RosettaCTF.Services;

namespace RosettaCTF.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(EventStartedFilter))]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class ScoreboardController : RosettaControllerBase
    {
        private ICtfChallengeRepository ChallengeRepository { get; }
        private ICtfChallengeCacheRepository ChallengeCacheRepository { get; }
        private ChallengePreviewRepository ChallengePreviewRepository { get; }

        public ScoreboardController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            UserPreviewRepository userPreviewRepository,
            ICtfConfigurationLoader ctfConfigurationLoader,
            ICtfChallengeRepository challengeRepository,
            ICtfChallengeCacheRepository ctfChallengeCacheRepository,
            ChallengePreviewRepository challengePreviewRepository)
            : base(loggerFactory, userRepository, userPreviewRepository, ctfConfigurationLoader)
        {
            this.ChallengeRepository = challengeRepository;
            this.ChallengeCacheRepository = ctfChallengeCacheRepository;
            this.ChallengePreviewRepository = challengePreviewRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<IEnumerable<ScoreboardEntryPreview>>>> Get(CancellationToken cancellationToken = default)
        {
            var solves = await this.ChallengeRepository.GetSuccessfulSolvesAsync(cancellationToken);
            var challenges = solves.Select(x => x.Challenge.Id).Distinct();
            var points = await this.ChallengeCacheRepository.GetScoresAsync(challenges, cancellationToken);

            var rteams = solves.Select(x => x.Team)
                .Distinct()
                .Select(x => new { id = x.Id, team = this.UserPreviewRepository.GetTeam(x) })
                .ToDictionary(x => x.id, x => x.team);

            var scoreboard = this.ChallengePreviewRepository.GetScoreboard(solves, points, rteams);
            return this.Ok(ApiResult.FromResult(scoreboard));
        }
    }
}
