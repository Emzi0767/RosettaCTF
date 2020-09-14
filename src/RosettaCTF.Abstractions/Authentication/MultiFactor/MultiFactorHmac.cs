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

namespace RosettaCTF.Authentication
{
    /// <summary>
    /// Determines the type of HMAC algorithm used for the MFA code generator.
    /// </summary>
    public enum MultiFactorHmac : int
    {
        /// <summary>
        /// Specifies unknown algorithm type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Specifies MD5 should be used as the HMAC algorithm.
        /// </summary>
        Md5 = 1,

        /// <summary>
        /// Specifies SHA1 should be used as the HMAC algorithm.
        /// </summary>
        Sha1 = 2,

        /// <summary>
        /// Specifies SHA256 should be used as the HMAC algorithm.
        /// </summary>
        Sha256 = 3,

        /// <summary>
        /// Specifies SHA384 should be used as the HMAC algorithm.
        /// </summary>
        Sha384 = 4,

        /// <summary>
        /// Specifies SHA512 should be used as the HMAC algorithm.
        /// </summary>
        Sha512 = 5
    }
}
