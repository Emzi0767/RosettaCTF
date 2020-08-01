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
using System.ComponentModel.DataAnnotations;

namespace RosettaCTF.Data
{
    /// <summary>
    /// Represents configuration for a temporary cache, like Redis.
    /// </summary>
    public sealed class RosettaConfigurationCache
    {
        /// <summary>
        /// Gets the type of the datastore.
        /// </summary>
        [Required, MinLength(1)]
        public string Type { get; set; }

        /// <summary>
        /// Gets the hostname to connect to.
        /// </summary>
        [Required, MinLength(1)]
        public string Hostname { get; set; }

        /// <summary>
        /// Gets the port to connect to.
        /// </summary>
        [Required, Range(1, 65535)]
        public int Port { get; set; }

        /// <summary>
        /// Gets the index of the datastore to use.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public int Index { get; set; }

        /// <summary>
        /// Gets the password used to authenticate with the datastore.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets whether to use encryption for datastore connection.
        /// </summary>
        public bool UseSsl { get; set; }
    }
}
