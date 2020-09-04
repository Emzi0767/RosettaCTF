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
    /// Describes a class which derives encryption keys from input data.
    /// </summary>
    public interface IKeyDeriver
    {
        /// <summary>
        /// Derives a key from given input.
        /// </summary>
        /// <param name="value">Value to derive key from.</param>
        /// <param name="salt">Salt for derived key.</param>
        /// <param name="byteCount">Number of bytes to derive.</param>
        /// <returns>Derived key.</returns>
        Task<byte[]> DeriveKeyAsync(byte[] value, byte[] salt, int byteCount);
    }
}
