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
    /// Represents basic information about ongoing CTF event.
    /// </summary>
    public sealed class CtfEvent : ICtfEvent
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IEnumerable<string> Organizers { get; }

        /// <inheritdoc />
        public DateTimeOffset StartTime { get; }

        /// <inheritdoc />
        public DateTimeOffset EndTime { get; }

        /// <inheritdoc />
        public CtfScoringMode Scoring { get; }

        /// <summary>
        /// Creates a new instance with specified settings.
        /// </summary>
        /// <param name="name">Name of this event.</param>
        /// <param name="organizers">Organizers of this event.</param>
        /// <param name="start">Time at which the event starts.</param>
        /// <param name="end">Time at which the event ends.</param>
        /// <param name="scoring">Scoring mode for this event.</param>
        /// <param name="flags">Event flags.</param>
        public CtfEvent(string name, IEnumerable<string> organizers, DateTimeOffset start, DateTimeOffset end, CtfScoringMode scoring)
        {
            this.Name = name;
            this.Organizers = organizers;
            this.StartTime = start;
            this.EndTime = end;
            this.Scoring = scoring;
        }

        /// <summary>
        /// Creates a new instance with specified settings.
        /// </summary>
        /// <param name="name">Name of this event.</param>
        /// <param name="organizers">Organizers of this event.</param>
        /// <param name="start">Time at which the event starts.</param>
        /// <param name="duration">Duration of this event.</param>
        /// <param name="scoring">Scoring mode for this event.</param>
        public CtfEvent(string name, IEnumerable<string> organizers, DateTimeOffset start, TimeSpan duration, CtfScoringMode scoring)
            : this(name, organizers, start, start + duration, scoring)
        { }
    }
}
