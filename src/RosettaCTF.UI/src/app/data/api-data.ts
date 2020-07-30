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

/**
 * Error codes that can be returned from the API.
 */
export enum ApiErrorCode {
    /**
     * No error occured.
     */
    NoError = 0,

    /**
     * Another error occured. This is usually not an API error status.
     */
    Other = -1,

    // --------------------------------------------------------------------------------------------
    // 1xxx - permission errors

    

    // --------------------------------------------------------------------------------------------
    // 2xxx - retrieval errors

    

    // --------------------------------------------------------------------------------------------
    // 3xxx - creation errors

    

    // --------------------------------------------------------------------------------------------
    // 4xxx - generic errors

    /**
     * Unknown error occured.
     */
    GenericError = 4000,
}

/**
 * Contains error details as returned from the API.
 */
export interface IApiError {

    /**
     * Specific error code, which indicates the problem that occured.
     */
    errorCode: ApiErrorCode;

    /**
     * Optional error message that can be displayed to the user.
     */
    message?: string;
}

/**
 * Contains information about a flag request, as returned from or sent to the API.
 */
export interface IApiFlag {

    /**
     * Flag text
     */
    flag: string;
}


/**
 * Contains information about a result from an API request.
 */
export interface IApiResult<T> {

    /**
     * Whether the operation was successful.
     */
    isSuccess: boolean;

    /**
     * Error information returned by the API. This is used to provide feedback to the user.
     */
    error?: IApiError | null;

    /**
     * Result from the request. See information about the type parameter to this type for more information.
     */
    result?: T | null;
}

/**
 * Represents the status of the backing API.
 */
export enum ApiStatus {
    /**
     * Defines that the API connection test was successful.
     */
    OK = 0,

    /**
     * Defines that the API connection test failed.
     */
    Failed = -1
}

/**
 * Represents an API connection test response.
 */
export interface IApiTestResponse {
    /**
     * Gets the name of the event.
     */
    name: string;

    /**
     * Gets the start time of the event.
     */
    startTime: string;

    /**
     * Gets the end time of the event.
     */
    endTime: string;
}
