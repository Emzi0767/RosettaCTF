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

using System.Collections.Generic;

namespace RosettaCTF.Models
{
    /// <summary>
    /// Represents a single CTFtime scoreboard entry.
    /// </summary>
    public sealed class CtfTimeStanding
    {
        /// <summary>
        /// Gets or sets the position in the overall standings.
        /// </summary>
        public int Pos { get; set; }

        /// <summary>
        /// Gets or sets the name of the team for this entry.
        /// </summary>
        public string Team { get; set; }

        /// <summary>
        /// Gets or sets the team's overall score.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the collection of task statistics for the team.
        /// </summary>
        public IDictionary<string, CtfTimeTaskEntryInfo> TaskStats { get; set; }

        /// <summary>
        /// Gets or sets the UNIX timestamp of the last accepted entry.
        /// </summary>
        public long LastAccept { get; set; }
    }
}
