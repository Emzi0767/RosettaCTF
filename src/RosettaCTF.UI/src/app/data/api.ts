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

    /**
     * Requested action requires the user to be authorized and logged in, but the user is not logged in.
     */
    NotLoggedIn = 1000,

    /**
     * Current user is not authorized to perform the requested action.
     */
    Unauthorized = 1001,

    /**
     * Current user is missing permissions required to access a resource.
     */
    MissingPermissions = 1002,

    /**
     * External authentication source responded with an error.
     */
    ExternalAuthenticationError = 1003,

    /**
     * User is already logged in.
     */
    AlreadyLoggedIn = 1004,

    // --------------------------------------------------------------------------------------------
    // 2xxx - retrieval errors

    /**
     * Requested challenge is not available. It might require separate activation.
     */
    ChallengeUnavailable = 2000,

    /**
     * Requested challenge does not exist.
     */
    ChallengeNotFound = 2001,

    /**
     * Requested team does not exist.
     */
    TeamNotFound = 2002,

    // --------------------------------------------------------------------------------------------
    // 3xxx - creation errors

    /**
     * Challenge has already been solved by the current user's team.
     */
    AlreadySolved = 3000,

    /**
     * The submitted flag is not the expected flag. Try again!
     */
    InvalidFlag = 3001,

    /**
     * Specified username is taken.
     */
    DuplicateUsername = 3002,

    /**
     * Specified team name is taken.
     */
    DuplicateTeamName = 3003,

    /**
     * Specified user already exists.
     */
    UserExists = 3004,

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
    code: ApiErrorCode;

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
 * Represents the event-configured scoring type.
 */
export enum ApiEventScoringMode {
    /**
     * Static jeopardy-style scoring mode (i.e. all challenges have static amount of points).
     */
    Static = 0,

    /**
     * Dynamic jeopardy-style scoring mode, where the number of points for each challenge will decay for all
     * participants as more teams solve the challenge.
     */
    Jeopardy = 1,

    /**
     * Defines dynamic jeopardy-style scoring mode, where the number of points for each challenge will decay, much
     * like Jeopardy, however, once a team completes a challenge, their points will no longer decay.
     */
    FirstComeFirstServe = 2
}

/**
 * Represents an API connection test response.
 */
export interface IApiEventConfiguration {
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

    /**
     * Gets the organizers of the event.
     */
    organizers: string[];

    /**
     * Gets the scoring mode for this event.
     */
    scoring: ApiEventScoringMode;
}
