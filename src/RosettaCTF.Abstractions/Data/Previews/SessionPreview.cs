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

using System;

namespace RosettaCTF.Data
{
    /// <summary>
    /// Represents a session.
    /// </summary>
    public sealed class SessionPreview
    {
        /// <summary>
        /// Gets whether this session is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; }

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        public UserPreview User { get; }

        /// <summary>
        /// Gets the token issued to the session's holder.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Gets the expiration timestamp of the token.
        /// </summary>
        public string ExpiresAt { get; }

        internal SessionPreview(UserPreview user)
            : this(user, null, null)
        { }

        internal SessionPreview(UserPreview user, string token, DateTimeOffset? expiresAt)
        {
            this.IsAuthenticated = user != null;
            this.User = user;
            this.Token = token;
            this.ExpiresAt = expiresAt?.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }
    }
}