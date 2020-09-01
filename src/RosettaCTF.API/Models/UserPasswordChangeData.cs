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
    public sealed class UserPasswordChangeData
    {
        /// <summary>
        /// Gets or sets the old password used to confirm identity.
        /// </summary>
        [MinLength(6), MaxLength(512)]
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password to use for the user.
        /// </summary>
        [Required, MinLength(6), MaxLength(512)]
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        [Required, MinLength(6), MaxLength(512), Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the MFA code for the request.
        /// </summary>
        public int? MfaCode { get; set; }
    }
}
