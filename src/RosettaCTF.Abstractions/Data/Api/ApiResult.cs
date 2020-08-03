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

namespace RosettaCTF.Data
{
    /// <summary>
    /// Represents a result of an API operation.
    /// </summary>
    /// <typeparam name="T">Type of the result, or <see cref="object"/> in case of errors.</typeparam>
    public sealed class ApiResult<T>
    {
        /// <summary>
        /// Gets whether the operation was a success.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the error that occured.
        /// </summary>
        public ApiError Error { get; }

        /// <summary>
        /// Gets the result of the request.
        /// </summary>
        public T Result { get; }

        internal ApiResult(bool success, ApiError error, T result)
        {
            this.IsSuccess = success;
            this.Error = error;
            this.Result = result;
        }

        /// <summary>
        /// Encapsulates a result.
        /// </summary>
        /// <param name="result">Result to encapsulate.</param>
        public static implicit operator ApiResult<T>(T result)
            => ApiResult.FromResult(result);
    }

    /// <summary>
    /// Creates instances of <see cref="ApiResult{T}"/>.
    /// </summary>
    public static class ApiResult
    {
        /// <summary>
        /// Creates a new error result instance.
        /// </summary>
        /// <param name="error">Error to encapsulate.</param>
        /// <returns>The created result.</returns>
        public static ApiResult<T> FromError<T>(ApiError error)
            => new ApiResult<T>(false, error, default);

        /// <summary>
        /// Creates a new success result instance.
        /// </summary>
        /// <typeparam name="T">Type of the result.</typeparam>
        /// <param name="result">Result to encapsulate.</param>
        /// <returns>The created result.</returns>
        public static ApiResult<T> FromResult<T>(T result)
            => new ApiResult<T>(true, null, result);
    }
}
