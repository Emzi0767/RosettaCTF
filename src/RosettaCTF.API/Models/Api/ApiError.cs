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

namespace RosettaCTF.Models
{
    /// <summary>
    /// Represents an error as returned from the API.
    /// </summary>
    public sealed class ApiError
    {
        /// <summary>
        /// Gets the code of the result.
        /// </summary>
        public ApiErrorCode Code { get; }

        /// <summary>
        /// Gets the result message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Creates new error information wrapper.
        /// </summary>
        /// <param name="code">Code of the error that occured.</param>
        /// <param name="message">Optional message of the error.</param>
        public ApiError(ApiErrorCode code, string message)
        {
            this.Code = code;
            this.Message = message;
        }
    }
}
