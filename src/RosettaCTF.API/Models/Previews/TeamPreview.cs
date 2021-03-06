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
using System.Collections.Generic;
using System.Linq;
using RosettaCTF.Data;

namespace RosettaCTF.Models
{
    /// <summary>
    /// Represents a brief, abridged view of <see cref="ITeam"/>.
    /// </summary>
    public sealed class TeamPreview
    {
        /// <summary>
        /// Gets the ID of the team.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the name of the team.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the avatar url of the team.
        /// </summary>
        public Uri AvatarUrl { get; }

        /// <summary>
        /// Gets the members of this team.
        /// </summary>
        public IEnumerable<UserPreview> Members { get; }

        internal TeamPreview(ITeam team)
        {
            this.Id = team.Id.AsString();
            this.Name = team.Name;
            this.AvatarUrl = team.AvatarUrl;
            this.Members = team.Members?.Select(x => new UserPreview(x, null /* this - object cycle */))
                .ToList();
        }
    }
}
