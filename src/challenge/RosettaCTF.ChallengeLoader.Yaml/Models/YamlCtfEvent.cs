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
using RosettaCTF.Data;
using SharpYaml.Serialization;

namespace RosettaCTF
{
    internal sealed class YamlCtfEvent : ICtfEvent
    {
        /// <inheritdoc />
        [YamlMember("name")]
        public string Name { get; set; }

        /// <inheritdoc />
        [YamlMember("organizers")]
        public IEnumerable<string> Organizers { get; set; }

        /// <inheritdoc />
        [YamlMember("startTime")]
        public DateTimeOffset StartTime { get; set; }

        /// <inheritdoc />
        [YamlMember("endTime")]
        public DateTimeOffset EndTime { get; set; }

        /// <inheritdoc />
        [YamlMember("scoring")]
        public CtfScoringMode Scoring { get; set; }
    }
}
