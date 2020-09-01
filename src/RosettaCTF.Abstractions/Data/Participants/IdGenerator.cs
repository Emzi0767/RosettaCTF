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
using System.Diagnostics;
using System.Threading;
using Emzi0767;

namespace RosettaCTF.Data
{
    /// <summary>
    /// Handles generation of numeric IDs for users and teams.
    /// </summary>
    public sealed class IdGenerator
    {
        private volatile int _state;
        private readonly long _pid;

        /// <summary>
        /// Creates a new ID generator.
        /// </summary>
        public IdGenerator()
        {
            this._state = 0;
            this._pid = (Process.GetCurrentProcess().Id & 0x1F) << 3; // 5 bits of PID
        }

        /// <summary>
        /// Generates a new ID.
        /// </summary>
        /// <returns></returns>
        public long Generate()
        {
            long ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long state = Interlocked.Increment(ref this._state) & 0x07; // 3 bits of state

            using var rng = new SecureRandom();
            long differentiator = (rng.GetUInt8() & 0x0F) << 8; // 4 bits of differentiator

            return (ms << 12) | differentiator | this._pid | state;
        }
    }
}
