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

namespace RosettaCTF.Models
{
    /// <summary>
    /// Represents a single entry in the capture log feed.
    /// </summary>
    public sealed class CtfTimeEventFeedEntry
    {
        /// <summary>
        /// Reresents a correct submission.
        /// </summary>
        public const string FlagCorrect = "taskCorrect";

        /// <summary>
        /// Represents an incorrect submission.
        /// </summary>
        public const string FlagWrong = "taskWrong";

        /// <summary>
        /// Gets or sets the ID of the entry.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the event, as a UNIX timestamp.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// Gets or sets the type of the entry.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        public string Team { get; set; }

        /// <summary>
        /// Gets or sets the name of the task.
        /// </summary>
        public string Task { get; set; }
    }
}
