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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RosettaCTF.Data;
using RosettaCTF.Filters;
using RosettaCTF.Services;

// All controllers defined herein are API controllers
[assembly: ApiController]

namespace RosettaCTF.Controllers
{
    [RosettaUserPopulatorFilter]
    public abstract class RosettaControllerBase : ControllerBase
    {
        protected ILoggerFactory LoggerFactory { get; }
        protected IUserRepository UserRepository { get; }
        protected UserPreviewRepository UserPreviewRepository { get; }
        protected ICtfEvent EventConfiguration { get; }
        protected IUser RosettaUser { get; private set; }
        protected TimeSpan Elapsed { get; private set; }

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

        [NonAction]
        public void SetUser(IUser user)
        {
            if (this.RosettaUser != null)
                throw new InvalidOperationException("User should not be set multiple times.");

            this.RosettaUser = user;
        }

        [NonAction]
        public void SetElapsed(TimeSpan elapsed)
        {
            this.Elapsed = elapsed;
        }
    }
}
