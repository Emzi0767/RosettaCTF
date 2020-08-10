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

using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RosettaCTF.Data;
using RosettaCTF.Services;

// All controllers defined herein are API controllers
[assembly: ApiController]

namespace RosettaCTF.Controllers
{
    public abstract class RosettaControllerBase : ControllerBase
    {
        protected ILoggerFactory LoggerFactory { get; }
        protected IUserRepository UserRepository { get; }
        protected UserPreviewRepository UserPreviewRepository { get; }
        protected ICtfEvent EventConfiguration { get; }

        protected RosettaControllerBase(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            UserPreviewRepository userPreviewRepository,
            ICtfConfigurationLoader ctfConfigurationLoader)
        {
            this.LoggerFactory = loggerFactory;
            this.UserRepository = userRepository;
            this.UserPreviewRepository = userPreviewRepository;
            this.EventConfiguration = ctfConfigurationLoader.LoadEventData();
        }

        protected async Task<IUser> GetCurrentUserAsync(CancellationToken cancellationToken)
        {
            var uids = this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (uids == null)
                return null;

            var uid = uids.ParseAsLong();
            return await this.UserRepository.GetUserAsync(uid, cancellationToken);
        }
    }
}
