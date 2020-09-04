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
using Emzi0767;

namespace RosettaCTF.Services
{
    /// <summary>
    /// Handles hashing and comparisons of passwords.
    /// </summary>
    public sealed class PasswordHandler
    {
        private const int PasswordSize = 480; // in bytes
        private const int SaltSize = 32;      // in bytes

        private IPasswordHashDeriver KeyDeriver { get; }

        /// <summary>
        /// Creates a new password handler instance.
        /// </summary>
        /// <param name="keyDeriver">Deriver implementation.</param>
        public PasswordHandler(IPasswordHashDeriver keyDeriver)
        {
            this.KeyDeriver = keyDeriver;
        }

        /// <summary>
        /// Creates a new password hash for storage.
        /// </summary>
        /// <param name="input">Input string to hash.</param>
        /// <returns>Hashed password.</returns>
        public async Task<byte[]> CreatePasswordHashAsync(string input)
        {
            var buff = new byte[SaltSize + PasswordSize];
            var salt = new byte[SaltSize];

            using (var rng = new SecureRandom())
                rng.GetBytes(salt);

            var utfinput = AbstractionUtilities.UTF8.GetBytes(input);
            var output = await this.KeyDeriver.DeriveHashAsync(utfinput, salt, PasswordSize);

            salt.AsSpan().CopyTo(buff);
            output.AsSpan().CopyTo(buff.AsSpan().Slice(SaltSize));

            return buff;
        }

        /// <summary>
        /// Validates user input against stored input.
        /// </summary>
        /// <param name="input">Input to compare.</param>
        /// <param name="reference">Reference password.</param>
        /// <returns>Whether the passwords match.</returns>
        public async Task<bool> ValidatePasswordHashAsync(string input, byte[] reference)
        {
            var salt = reference.AsSpan(0, SaltSize).ToArray();

            var utfinput = AbstractionUtilities.UTF8.GetBytes(input);
            var output = await this.KeyDeriver.DeriveHashAsync(utfinput, salt, PasswordSize);

            return AbstractionUtilities.ConstantTimeEquals(output, reference.AsSpan().Slice(SaltSize), PasswordSize);
        }
    }
}
