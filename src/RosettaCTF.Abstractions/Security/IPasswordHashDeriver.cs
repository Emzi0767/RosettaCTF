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

using System.Threading.Tasks;

namespace RosettaCTF
{
    /// <summary>
    /// Describes a class which derives secure hashes from string passwords, for purposes of user authentication.
    /// </summary>
    public interface IPasswordHashDeriver
    {
        /// <summary>
        /// Derives a hash from given password input.
        /// </summary>
        /// <param name="value">Password to derive input from.</param>
        /// <param name="salt">Salt for the derived hash.</param>
        /// <param name="byteCount">Number of bytes to derive.</param>
        /// <returns>Derived hash.</returns>
        Task<byte[]> DeriveHashAsync(byte[] value, byte[] salt, int byteCount);
    }
}
