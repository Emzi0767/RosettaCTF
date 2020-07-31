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
using System.Globalization;
using System.IO;

namespace RosettaCTF
{
    /// <summary>
    /// Specifies that specified path has to be an existent file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidFilePathAttribute : ValidationAttribute
    {
        /// <inheritdoc />
        public override bool RequiresValidationContext
            => true;

        /// <summary>
        /// Specifies that specified path has to be an existent file.
        /// </summary>
        public ValidFilePathAttribute()
            : base("File specified in property '{0}' does not exist.")
        { }

        /// <summary>
        /// Specifies that specified path has to be an existent file, with a given error message accessor.
        /// </summary>
        /// <param name="errorMessageAccessor">Error message accessor.</param>
        public ValidFilePathAttribute(Func<string> errorMessageAccessor) 
            : base(errorMessageAccessor)
        { }

        /// <summary>
        /// Specifies that specified path has to be an existent file, with a given error message.
        /// </summary>
        /// <param name="errorMessage">Error message to use for validation errors.</param>
        public ValidFilePathAttribute(string errorMessage) 
            : base(errorMessage)
        { }

        /// <inheritdoc />
        public override string FormatErrorMessage(string name)
            => string.Format(CultureInfo.InvariantCulture, this.ErrorMessage, name);

        /// <inheritdoc />
        public override bool IsValid(object value)
            => value is string path && File.Exists(path);

        /// <inheritdoc />
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            => this.IsValid(value)
            ? ValidationResult.Success
            : new ValidationResult(this.FormatErrorMessage(validationContext?.DisplayName ?? validationContext?.MemberName ?? "(unknown)"));
    }
}
