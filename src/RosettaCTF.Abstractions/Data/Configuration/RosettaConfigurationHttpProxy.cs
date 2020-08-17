﻿// This file is part of RosettaCTF project.
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
    public sealed class RosettaConfigurationHttpProxy
    {
        /// <summary>
        /// Gets or sets whether forward header reading is enabled;
        /// </summary>
        [Required]
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets the forward limit for the headers.
        /// </summary>
        [Required]
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or sets the networks the headers are trusted from.
        /// </summary>
        public string[] Networks { get; set; }
    }
}
