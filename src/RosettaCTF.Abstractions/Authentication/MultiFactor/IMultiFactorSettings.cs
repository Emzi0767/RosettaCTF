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
    /// Represents configuration for a server component of MFA validation.
    /// </summary>
    public interface IMultiFactorSettings
    {
        /// <summary>
        /// Gets the secret used to generate MFA tokens.
        /// </summary>
        byte[] Secret { get; }

        /// <summary>
        /// Gets the number of digits the generated codes should contain.
        /// </summary>
        int Digits { get; }

        /// <summary>
        /// Gets the HMAC algorithm used to generate MFA tokens.
        /// </summary>
        MultiFactorHmac HmacAlgorithm { get; }

        /// <summary>
        /// Gets the number of seconds constituting a single MFA counter tick.
        /// </summary>
        int Period { get; }

        /// <summary>
        /// Gets the additional data used to generate MFA tokens, if applicable.
        /// </summary>
        byte[] Additional { get; }

        /// <summary>
        /// Gets the MFA generation mode.
        /// </summary>
        MultiFactorType Type { get; }

        /// <summary>
        /// Gets the counter base for MFA recovery code generation.
        /// </summary>
        long RecoveryCounterBase { get; }

        /// <summary>
        /// Gets how many times the recovery counter has been tripped.
        /// </summary>
        int RecoveryTripCount { get; }
    }
}
