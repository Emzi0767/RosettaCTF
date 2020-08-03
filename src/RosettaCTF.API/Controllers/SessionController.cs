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
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RosettaCTF.Data;

namespace RosettaCTF.Controllers
{
    [Route("api/[controller]"), ValidateAntiForgeryToken]
    public class SessionController : RosettaControllerBase
    {
        private RosettaConfigurationDiscord DiscordConfiguration { get; }

        public SessionController(
            ILoggerFactory loggerFactory,
            IOptions<RosettaConfigurationDiscord> cfgDiscord)
            : base(loggerFactory)
        {
            this.DiscordConfiguration = cfgDiscord.Value;
        }

        [HttpGet, Route("endpoint")]
        public ActionResult<ApiResult<string>> Endpoint()
        {
            var ub = new UriBuilder
            {
                Scheme = "https",
                Host = "discord.com",
                Path = "/api/v7/oauth2/authorize"
            };

            var rub = new UriBuilder
            {
                Scheme = this.HttpContext.Request.Scheme,
                Host = this.DiscordConfiguration.Hostname,
                Port = this.DiscordConfiguration.Port,
                Path = "/session/callback"
            };

            var uri = QueryHelpers.AddQueryString(ub.Uri.ToString(), new Dictionary<string, string>(5)
            {
                ["client_id"] = this.DiscordConfiguration.ClientId.ToString(CultureInfo.InvariantCulture),
                ["redirect_uri"] = rub.Uri.ToString(),
                ["response_type"] = "code",
                ["scope"] = "guilds identify",
                ["state"] = Guid.NewGuid().ToString()
            });

            return ApiResult.FromResult(uri);
        }

        [HttpGet]
        public ActionResult<ApiResult<object>> Get()
            => ApiResult.FromResult<object>(new { authenticated = true, token = "asdf", user = new { id = "urmum", username = "Ur Mum#1234" }, team = new { name = "Test Team", members = new[] { new { id = "urmum", username = "Ur Mum#1234" } } } });

        [HttpPut, AllowAnonymous]
        public ActionResult Put()
        {
            return this.Forbid();
        }
    }
}
