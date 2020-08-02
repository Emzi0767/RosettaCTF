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

namespace RosettaCTF.Data
{
    /// <summary>
    /// Represents a group of <see cref="IUser"/> participating in the challenge.
    /// </summary>
    public interface ITeam
    {
        /// <summary>
        /// Gets the ID of the team.
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Gets the name of the team.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the URL of the team's avatar.
        /// </summary>
        Uri AvatarUrl { get; }

        /// <summary>
        /// Gets the users associated with this team as an enumerable of <see cref="IUser"/>.
        /// </summary>
        IEnumerable<IUser> Members { get; }
    }
}
