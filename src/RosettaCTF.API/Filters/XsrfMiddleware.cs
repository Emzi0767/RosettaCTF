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

using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace RosettaCTF.Filters
{
    internal sealed class XsrfMiddleware
    {
        private RequestDelegate Next { get; }

        private IAntiforgery Xsrf { get; }

        private string CookieName { get; }

        public XsrfMiddleware(RequestDelegate next, IAntiforgery xsrf, string cookieName)
        {
            this.Next = next;
            this.Xsrf = xsrf;
            this.CookieName = cookieName;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.GetEndpoint() != null)
            {
                var token = this.Xsrf.GetAndStoreTokens(context);
                context.Response.Cookies.Append(this.CookieName, token.RequestToken, new CookieOptions { HttpOnly = false });
            }

            await this.Next(context);
        }
    }
}
