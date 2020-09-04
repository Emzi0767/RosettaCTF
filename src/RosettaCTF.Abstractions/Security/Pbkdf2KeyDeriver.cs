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

using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RosettaCTF
{
    /// <summary>
    /// Derives encryption keys from values using PBKDF2 algorithm.
    /// </summary>
    public sealed class Pbkdf2KeyDeriver : IKeyDeriver
    {
        public Task<byte[]> DeriveKeyAsync(byte[] value, byte[] salt, int byteCount)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(value, salt, 16384))
                return Task.FromResult(pbkdf2.GetBytes(byteCount));
        }
    }
}
