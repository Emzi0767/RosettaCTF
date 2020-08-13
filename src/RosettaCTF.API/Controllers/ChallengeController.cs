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
    [Authorize(Roles = JwtAuthenticationOptions.RoleParticipant + "," + JwtAuthenticationOptions.RoleTeamMember)]
    [ServiceFilter(typeof(ValidRosettaUserFilter))]
    [ServiceFilter(typeof(EventStartedFilter))]
    [ElapsedPopulatorFilter]
    [ValidateAntiForgeryToken]
    public sealed class ChallengeController : RosettaControllerBase
    {
        private ICtfChallengeRepository ChallengeRepository { get; }
        private ChallengePreviewRepository ChallengePreviewRepository { get; }

        public ChallengeController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            UserPreviewRepository userPreviewRepository,
            ICtfConfigurationLoader ctfConfigurationLoader,
            ICtfChallengeRepository challengeRepository,
            ChallengePreviewRepository challengePreviewRepository)
            : base(loggerFactory, userRepository, userPreviewRepository, ctfConfigurationLoader)
        {
            this.ChallengeRepository = challengeRepository;
            this.ChallengePreviewRepository = challengePreviewRepository;
        }

        [HttpGet]
        [Route("categories")]
        public async Task<ActionResult<ApiResult<IEnumerable<ChallengeCategoryPreview>>>> GetCategories(CancellationToken cancellationToken = default)
        {
            var categories = await this.ChallengeRepository.GetCategoriesAsync(cancellationToken);
            var rcategories = this.ChallengePreviewRepository.GetChallengeCategories(categories, this.Elapsed, this.RosettaUser.HasHiddenAccess);
            return this.Ok(rcategories);

            // TODO: update scoring
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ApiResult<ChallengePreview>>> GetChallenge(string id, CancellationToken cancellationToken = default)
        {
            var challenge = await this.ChallengeRepository.GetChallengeAsync(id, cancellationToken);
            var rchallenge = this.ChallengePreviewRepository.GetChallenge(challenge, this.Elapsed);
            return this.Ok(rchallenge);

            // TODO: update scoring
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<ActionResult<ApiResult<bool>>> SubmitFlag([FromRoute] string id, [FromBody] ChallengeFlagModel challengeFlag, CancellationToken cancellationToken = default)
        {
            var challenge = await this.ChallengeRepository.GetChallengeAsync(id, cancellationToken);
            var flag = challengeFlag.Flag;

            return this.Forbid();
        }
    }
}
