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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RosettaCTF.Data;
using RosettaCTF.Models;
using RosettaCTF.Models.Previews;

namespace RosettaCTF.Controllers
{
    [Route("api/[controller]"), AllowAnonymous]
    public sealed class ConfigController : ControllerBase
    {
        private ICtfConfigurationLoader ConfigurationLoader { get; }
        private IUserRepository UserRepository { get; }

        public ConfigController(ICtfConfigurationLoader cfgLoader, IUserRepository userRepository)
        {
            this.ConfigurationLoader = cfgLoader;
            this.UserRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<CtfConfigurationPreview>>> Get(CancellationToken cancellationToken = default)
            => ApiResult.FromResult(
                new CtfConfigurationPreview(
                    this.ConfigurationLoader.LoadEventData(),
                    await this.UserRepository.GetAllCountriesAsync(cancellationToken)));

        [HttpGet, Route("wiggle")]
        public ActionResult<ApiResult<object>> Wiggle()
            => ApiResult.FromResult<object>(null);
    }
}
