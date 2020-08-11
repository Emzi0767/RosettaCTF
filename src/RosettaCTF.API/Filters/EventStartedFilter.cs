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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF.Filters
{
    public sealed class EventStartedFilter : IAsyncAuthorizationFilter
    {
        private ICtfConfigurationLoader CtfConfigurationLoader { get; }

        public EventStartedFilter(ICtfConfigurationLoader ctfConfigurationLoader)
        {
            this.CtfConfigurationLoader = ctfConfigurationLoader;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var eventCfg = this.CtfConfigurationLoader.LoadEventData();
            var elapsed = DateTimeOffset.UtcNow - eventCfg.StartTime;
            if (elapsed < TimeSpan.Zero)
                context.Result = new ObjectResult(ApiResult.FromError<object>(new ApiError(ApiErrorCode.Unauthorized, "Event has not yet started.")))
                {
                    StatusCode = 403
                };

            context.HttpContext.Items["RosettaCTF:StartElapsed"] = elapsed;
            return Task.CompletedTask;
        }
    }
}
