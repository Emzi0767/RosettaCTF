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
 * Represents a user.
 */
export interface IUser {
    /**
     * Gets the ID of the user.
     */
    id: string;

    /**
     * Gets the name of the user.
     */
    username: string;

    /**
     * Gets the URL of the user's avatar. Usually a Piroxy URL.
     */
    avatarUrl: string | null;

    /**
     * Gets the team this user belongs to.
     */
    team: ITeam | null;
}

/**
 * Represents a team, which is a collection of users.
 */
export interface ITeam {
    /**
     * Gets the ID of the team.
     */
    id: string;

    /**
     * Gets the name of the team.
     */
    name: string;

    /**
     * Gets the URL of the team's avatar. Usually a Piroxy URL.
     */
    avatarUrl: string | null;

    /**
     * Gets the users who belong to this team.
     */
    members: IUser[];
}

/**
 * Represents an invite to a team.
 */
export interface ITeamInvite {
    /**
     * Gets the ID of the team.
     */
    id: string;

    /**
     * Gets the name of the team.
     */
    name: string;

    /**
     * Gets the URL of the team's avatar. Usually a Piroxy URL.
     */
    avatarUrl: string | null;
}

/**
 * Represents current user's session.
 */
export interface ISession {
    /**
     * Gets whether the current user has successfully authenticated.
     */
    isAuthenticated: boolean;

    /**
     * Gets the currently-authenticated user.
     */
    user: IUser | null;

    /**
     * Gets the token issued to the caller.
     */
    token?: string;

    /**
     * Gets the datetime string at which the token expires.
     */
    expiresAt?: string;
}
