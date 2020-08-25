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

namespace RosettaCTF.Models
{
    /// <summary>
    /// Contains arguments for PUT /session
    /// </summary>
    public sealed class OAuthAuthenticationData
    {
        /// <summary>
        /// OAuth authentication code to exchange for a token.
        /// </summary>
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// OAuth state to validate.
        /// </summary>
        [Required]
        public string State { get; set; }

        /// <summary>
        /// OAuth referrer.
        /// </summary>
        [Required]
        public string Referrer { get; set; }
    }
}
