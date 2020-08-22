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

using System.ComponentModel.DataAnnotations;

namespace RosettaCTF.Data
{
    /// <summary>
    /// Represents configuration for the key deriver.
    /// </summary>
    public sealed class ConfigurationSecurity
    {
        /// <summary>
        /// Gets or sets the degree of parallelism when computing the hash.
        /// </summary>
        [Required]
        public int Parallelism { get; set; }

        /// <summary>
        /// Gets or sets how much memory should be used when computing the hash.
        /// </summary>
        [Required]
        public int Memory { get; set; }

        /// <summary>
        /// Gets or sets the number of iterations to use when computing the hash.
        /// </summary>
        [Required]
        public int Iterations { get; set; }
    }
}
