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
using System.Threading.Tasks;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;
using RosettaCTF.Data;

namespace RosettaCTF
{
    /// <summary>
    /// Derives password hashes from values using Argon2id algorithm.
    /// </summary>
    public sealed class Argon2idHashDeriver : IPasswordHashDeriver
    {
        private int? Parallelism { get; }
        private int? MemorySize { get; }
        private int? Iterations { get; }

        public Argon2idHashDeriver(IOptions<ConfigurationSecurity> opts)
        {
            var ov = opts.Value;
            this.Parallelism = ov.Parallelism <= 0 ? null : ov.Parallelism as int?;
            this.MemorySize = ov.Memory <= 0 ? null : ov.Memory as int?;
            this.Iterations = ov.Iterations <= 0 ? null : ov.Iterations as int?;
        }

        public async Task<byte[]> DeriveHashAsync(byte[] value, byte[] salt, int byteCount)
        {
            // just to be safe
            var k = new byte[value.Length];
            value.AsSpan().CopyTo(k);

            var s = salt != null 
                ? new byte[salt.Length] 
                : null;
            if (s != null)
                salt.AsSpan().CopyTo(s);

            var argon2 = new Argon2id(k)
            {
                DegreeOfParallelism = this.Parallelism ?? (Environment.ProcessorCount * 2),
                MemorySize = this.MemorySize ?? 131072, /* 128 MiB */
                Iterations = this.Iterations ?? ComputeIterations(Environment.ProcessorCount),
                Salt = s
            };

            return await argon2.GetBytesAsync(byteCount);
        }

        private static int ComputeIterations(int processors)
            => (int)Math.Ceiling(0.6667 * processors + 3.3333);
    }
}
