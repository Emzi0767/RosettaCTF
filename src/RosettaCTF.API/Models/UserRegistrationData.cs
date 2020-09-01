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
    /// Contains data for user registration.
    /// </summary>
    public sealed class UserRegistrationData
    {
        /// <summary>
        /// Gets or sets the username of the registered user.
        /// </summary>
        [Required, MinLength(2), MaxLength(48), RegularExpression(AbstractionUtilities.NameRegexPattern)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password to use for the registered user.
        /// </summary>
        [Required, MinLength(6), MaxLength(512)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        [Required, MinLength(6), MaxLength(512), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
