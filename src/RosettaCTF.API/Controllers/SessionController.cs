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
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RosettaCTF.Data;

namespace RosettaCTF.Controllers
{
    [Route("api/[controller]"), ValidateAntiForgeryToken]
    public class SessionController : RosettaControllerBase
    {
        public SessionController(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        { }

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
