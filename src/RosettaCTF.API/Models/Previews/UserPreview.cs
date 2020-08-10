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
using RosettaCTF.Data;

namespace RosettaCTF.Models
{
    /// <summary>
    /// Represents a brief, abridged view of <see cref="IUser"/>.
    /// </summary>
    public sealed class UserPreview
    {
        /// <summary>
        /// Gets the ID of the user.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the avatar url of the user.
        /// </summary>
        public Uri AvatarUrl { get; }

        /// <summary>
        /// Gets the team the user belongs to.
        /// </summary>
        public TeamPreview Team { get; }

        internal UserPreview(IUser user, TeamPreview team)
        {
            this.Id = user.Id;
            this.Username = user.Username;
            this.AvatarUrl = user.AvatarUrl;
            this.Team = team;
        }
    }
}
