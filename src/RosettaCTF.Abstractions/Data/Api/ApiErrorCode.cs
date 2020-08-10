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
    /// Defines various error codes returned as part of an API operation.
    /// </summary>
    public enum ApiErrorCode : int
    {
        /// <summary>
        /// No error occured.
        /// </summary>
        NoError = 0,

        /// <summary>
        /// Another error occured. This is usually not an API error status.
        /// </summary>
        Other = -1,

        // 1xxx - permission errors

        /// <summary>
        /// Requested action requires the user to be authorized and logged in, but the user is not logged in.
        /// </summary>
        NotLoggedIn = 1000,

        /// <summary>
        /// Current user is not authorized to perform the requested action.
        /// </summary>
        Unauthorized = 1001,

        /// <summary>
        /// Current user is missing permissions required to access a resource.
        /// </summary>
        MissingPermissions = 1002,

        /// <summary>
        /// External authentication source responded with an error.
        /// </summary>
        ExternalAuthenticationError = 1003,

        /// <summary>
        /// User is already logged in.
        /// </summary>
        AlreadyLoggedIn = 1004,

        // 2xxx - retrieval errors

        /// <summary>
        /// Requested challenge is not available. It might require separate activation.
        /// </summary>
        ChallengeUnavailable = 2000,

        /// <summary>
        /// Requested challenge does not exist.
        /// </summary>
        ChallengeNotFound = 2001,

        /// <summary>
        /// Requested team does not exist.
        /// </summary>
        TeamNotFound = 2002,

        /// <summary>
        /// Requested user does not exist.
        /// </summary>
        UserNotFound = 2003,

        // 3xxx - creation errors

        /// <summary>
        /// Challenge has already been solved by the current user's team.
        /// </summary>
        AlreadySolved = 3000,

        /// <summary>
        /// The submitted flag is not the expected flag. Try again!
        /// </summary>
        InvalidFlag = 3001,

        /// <summary>
        /// Specified username is taken.
        /// </summary>
        DuplicateUsername = 3002,

        /// <summary>
        /// Specified team name is taken.
        /// </summary>
        DuplicateTeamName = 3003,

        /// <summary>
        /// Specified user already exists.
        /// </summary>
        UserExists = 3004,

        /// <summary>
        /// Event has started. Team creation is disabled.
        /// </summary>
        EventStarted = 3005,

        /// <summary>
        /// Specified user already has a team.
        /// </summary>
        UserAlreadyOnTeam = 3006,

        // 4xxx - generic errors

        /// <summary>
        /// Unknown error occured.
        /// </summary>
        GenericError = 4000
    }
}
