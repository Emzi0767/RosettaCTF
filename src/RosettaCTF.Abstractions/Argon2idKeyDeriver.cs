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

namespace RosettaCTF
{
    public sealed class Argon2idKeyDeriver
    {
        public async Task<byte[]> DeriveKeyAsync(byte[] value, byte[] salt, int byteCount)
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
                DegreeOfParallelism = Environment.ProcessorCount * 2,
                MemorySize = 16384, /* 16 MiB */
                Iterations = 8,
                Salt = s
            };

            return await argon2.GetBytesAsync(byteCount);
        }
    }
}
