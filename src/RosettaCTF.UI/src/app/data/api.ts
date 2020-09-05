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

import { IUser, ITeam } from "./session";

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

    /**
     * Specified credentials were invalid.
     */
    InvalidCredentials = 1005,

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

    /**
     * Requested user does not exist.
     */
    UserNotFound = 2003,

    /**
     * Requested authentication provider does not exist.
     */
    InvalidProvider = 2004,

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

    /**
     * Event has started. Team creation is disabled.
     */
    EventStarted = 3005,

    /**
     * Specified user already has a team.
     */
    UserAlreadyOnTeam = 3006,

    /**
     * Specified name contained invalid characters.
     */
    InvalidName = 3007,

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
     * Gets the ID of the challenge.
     */
    id: string;

    /**
     * Flag text
     */
    flag: string;
}

/**
 * Contains information necessary to create a team.
 */
export interface ICreateTeam {

    /**
     * Gets or sets the name for the new team.
     */
    name: string;
}

/**
 * Contains information necessary to invite a team member.
 */
export interface ICreateTeamInvite {

    /**
     * Gets or sets the ID of the user to invite.
     */
    id: string;
}

/**
 * Contains information necessary to register a user.
 */
export interface IUserRegister {

    /**
     * Gets or sets the username for the new user.
     */
    username: string;

    /**
     * Gets or sets the password for the new user.
     */
    password: string;

    /**
     * Gets or sets the confirm password for the new user.
     */
    confirmPassword: string;
}

/**
 * Contains information necessary to login a user.
 */
export interface IUserLogin {

    /**
     * Gets or sets the username to log in as.
     */
    username: string;

    /**
     * Gets or sets the password to authenticate with.
     */
    password: string;
}

/**
 * Contains information necessary to complete a login with MFA.
 */
export interface IMfa {

    /**
     * Gets or sets the MFA code to authenticate with.
     */
    mfaCode: number;
}

/**
 * Contains information necessary to change a user's password.
 */
export interface IUserPasswordChange {

    /**
     * Gets or sets the old password.
     */
    oldPassword: string;

    /**
     * Gets or sets the new password.
     */
    newPassword: string;

    /**
     * Gets or sets the confirm new password.
     */
    confirmPassword: string;

    /**
     * Gets or sets the MFA code if one is required.
     */
    mfaCode?: number;
}

/**
 * Contains information necessary to remove a user's password.
 */
export interface IUserPasswordRemove {

    /**
     * Gets or sets the password to validate.
     */
    password: string;

    /**
     * Gets or sets the MFA code if one is required.
     */
    mfaCode?: number;
}

/**
 * Contains information about login settings.
 */
export interface ILoginSettings {

    /**
     * Gets or sets whether local login is enabled.
     */
    localLoginEnabled: boolean;

    /**
     * Gets or sets whether external login is enabled.
     */
    externalLoginEnabled: boolean;

    /**
     * Gets or sets the list of external login providers.
     */
    externalAccountProviders: ILoginProvider[];
}

/**
 * Cotains information about a login provider.
 */
export interface ILoginProvider {

    /**
     * Gets or sets the ID of the provider.
     */
    id: string;

    /**
     * Gets or sets the display name of the provider.
     */
    name: string;

    /**
     * Gets or sets the colour of the login button for the provider.
     */
    colour: string | null;
}

/**
 * Contains information about an external account.
 */
export interface IExternalAccount {

    /**
     * Gets or sets the ID of the account.
     */
    id: string;

    /**
     * Gets or sets the username of the account.
     */
    username: string;

    /**
     * Gets or sets the provider of the account.
     */
    provider: ILoginProvider;
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
 * Represents basic country data.
 */
export interface ICountry
{

    /**
     * Gets or sets the ISO-3166-alpha-2 country code.
     */
    code: string;

    /**
     * Gets or sets the name of the country.
     */
    name: string;
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

    /**
     * Gets additional settings for this event.
     */
    flags: number;

    /**
     * Gets the countries defined for the event.
     */
    countries: ICountry[];
}

/**
 * Represents a challenge category.
 */
export interface IChallengeCategory {

    /**
     * Gets the ID of the category.
     */
    id: string;

    /**
     * Gets the name of the category.
     */
    name: string;

    /**
     * Gets the challenges in this category.
     */
    challenges: IChallenge[];
}

/**
 * Represents a challenge.
 */
export interface IChallenge {

    /**
     * Gets the ID of the challenge.
     */
    id: string;

    /**
     * Gets the title of the challenge.
     */
    title: string;

    /**
     * Gets the difficulty of the challenge.
     */
    difficulty: string;

    /**
     * Gets the description of the challenge.
     */
    description: string;

    /**
     * Gets the available hints, if any.
     */
    hints?: string[];

    /**
     * Gets the available attachments, if any.
     */
    attachments?: IChallengeAttachment[];

    /**
     * Gets the endpoint of the challenge, if any.
     */
    endpoint?: string;

    /**
     * Gets the point value of this challenge.
     */
    score: number;

    /**
     * Gets the solve status in current context, if applicable.
     */
    isSolved: boolean | null;
}

/**
 * Represents a file attached to a challenge.
 */
export interface IChallengeAttachment {

    /**
     * Gets the name of the attached file.
     */
    name: string;

    /**
     * Gets the type of the attached file.
     */
    type: string;

    /**
     * Gets the size of the attached file.
     */
    length: string;

    /**
     * Gets the SHA256 checksum of the attached file.
     */
    sha256: string;

    /**
     * Gets the SHA1 checksum of the attached file.
     */
    sha1: string;

    /**
     * Gets the download URI of the attached file.
     */
    uri?: string;
}

/**
 * Represents a submitted solution.
 */
export interface ISolve {

    /**
     * Gets the challenge the submission was for.
     */
    challenge: IChallenge;

    /**
     * Gets the category the submission was for.
     */
    category: IChallengeCategory;

    /**
     * Gets the author of the submission.
     */
    user: IUser;

    /**
     * Gets the team the submission was for.
     */
    team: ITeam;

    /**
     * Gets the points received for the submission.
     */
    score: number;

    /**
     * Gets the ordinal of this submission. This determines placing.
     */
    ordinal: number;

    /**
     * Gets the amount of time taken to solve.
     */
    timeTaken: number;
}

/**
 * Represents an entry on the scoreboard.
 */
export interface IScoreboardEntry {

    /**
     * Gets the team for this entry.
     */
    team: ITeam;

    /**
     * Gets the score this team has accumulated.
     */
    score: number;

    /**
     * Gets the position of the team on the leaderboard.
     */
    ordinal: number;
}
