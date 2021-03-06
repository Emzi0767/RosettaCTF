﻿// This file is part of RosettaCTF project.
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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF.Filters
{
    public sealed class ValidRosettaUserFilter : IAsyncAuthorizationFilter
    {
        private static ObjectResult UnauthorizedResult { get; } = new ObjectResult(ApiResult.FromError<object>(new ApiError(ApiErrorCode.NotLoggedIn, "User is not logged in.")))
        {
            StatusCode = 401
        };

        private IUserRepository UserRepository { get; }

        public ValidRosettaUserFilter(IUserRepository userRepository)
        {
            this.UserRepository = userRepository;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var principal = context.HttpContext.User;
            if (!principal.Identity.IsAuthenticated)
            {
                context.Result = UnauthorizedResult;
                return;
            }

            var uids = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (uids == null)
            {
                context.Result = UnauthorizedResult;
                return;
            }

            var uid = uids.ParseAsLong();
            var user = await this.UserRepository.GetUserAsync(uid);
            if (user == null)
            {
                context.Result = UnauthorizedResult;
                return;
            }

            context.HttpContext.Items["RosettaCTF:User"] = user;
        }
    }
}
