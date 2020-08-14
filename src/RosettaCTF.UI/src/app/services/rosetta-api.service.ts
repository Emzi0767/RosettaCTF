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

import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { IApiResult, IApiEventConfiguration, IApiFlag, IScoreboardEntry, IChallengeCategory } from "../data/api";
import { ISession, ITeam, ITeamInvite } from "../data/session";
import { SessionProviderService } from "./session-provider.service";

interface IHttpOptions {
    headers?: {
        [header: string]: string | string[];
    };
    observe: "response";
    params?: {
        [param: string]: string | string[];
    };
    reportProgress?: boolean;
    responseType?: "json";
    withCredentials?: boolean;
}

@Injectable({
    providedIn: "root"
})
export class RosettaApiService {

    constructor(private http: HttpClient,
                private sessionProvider: SessionProviderService) { }

    async testApi(): Promise<IApiResult<IApiEventConfiguration>> {
        try {
            const response = await this.http.get<IApiResult<IApiEventConfiguration>>("/api/config", this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    // SESSION -------------------------------------------------------
    async getSession(): Promise<IApiResult<ISession>> {
        try {
            const response = await this.http.get<IApiResult<ISession>>("/api/session", this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async getLoginUrl(): Promise<IApiResult<string>> {
        try {
            const response = await this.http.get<IApiResult<string>>("/api/session/endpoint", this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async completeLogin(code: string, state: string): Promise<IApiResult<ISession>> {
        try {
            const response = await this.http.post<IApiResult<ISession>>("/api/session",
                { code, state },
                this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async logout(): Promise<IApiResult<ISession>> {
        try {
            const response = await this.http.delete<IApiResult<ISession>>("/api/session", this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    // REFRESH -------------------------------------------------------
    async refreshToken(): Promise<IApiResult<ISession>> {
        try {
            const response = await this.http.get<IApiResult<ISession>>("/api/session/refresh", this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async refreshXsrf(): Promise<IApiResult<null>> {
        try {
            const response = await this.http.get<IApiResult<null>>("/api/config/wiggle", this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    // TEAM ----------------------------------------------------------
    async getTeam(id?: string): Promise<IApiResult<ITeam>> {
        try {
            const uri = !!id ? `/api/team/${id}` : "/api/team";
            const response = await this.http.get<IApiResult<ITeam>>(uri, this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async createTeam(name: string): Promise<IApiResult<ITeam>> {
        try {
            const response = await this.http.post<IApiResult<ITeam>>("/api/team", { name }, this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async kickTeamMember(userId: string): Promise<IApiResult<ITeam>> {
        try {
            const response = await this.http.delete<IApiResult<ITeam>>(`/api/team/members/${userId}`, this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async inviteMember(userId: string): Promise<IApiResult<ITeam>> {
        try {
            const response = await this.http.post<IApiResult<ITeam>>(`/api/team/invite/${userId}`, null, this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async acceptInvite(teamId: string): Promise<IApiResult<ITeam>> {
        try {
            const response = await this.http.patch<IApiResult<ITeam>>(`/api/team/invite/${teamId}`, null, this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async getInvites(): Promise<IApiResult<ITeamInvite[]>> {
        try {
            const response = await this.http.get<IApiResult<ITeamInvite[]>>("/api/team/invite", this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    // SCOREBOARD ----------------------------------------------------
    async getScoreboard(): Promise<IApiResult<IScoreboardEntry[]>> {
        try {
            const response = await this.http.get<IApiResult<IScoreboardEntry[]>>("/api/scoreboard", this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    // CHALLENGES ----------------------------------------------------
    async getCategories(): Promise<IApiResult<IChallengeCategory[]>> {
        try {
            // tslint:disable-next-line: max-line-length
            const response = await this.http.get<IApiResult<IChallengeCategory[]>>("/api/challenge/categories", this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async submitSolve(flag: IApiFlag): Promise<IApiResult<boolean>> {
        try {
            // tslint:disable-next-line: max-line-length
            const response = await this.http.post<IApiResult<boolean>>(`/api/challenge/${flag.id}`, { flag: flag.flag }, this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    // SUPRISE -------------------------------------------------------
    async enableHidden(): Promise<IApiResult<void>> {
        try {
            const response = await this.http.post<IApiResult<void>>("/api/session/unhide", null, this.getOptions()).toPromise();
            return response.body;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    // HELPERS -------------------------------------------------------
    private getHeaders(): { [header: string]: string | string[] } {
        const token = this.sessionProvider.getToken();
        if (!!token) {
            return {
                Authorization: token
            };
        } else {
            return { };
        }
    }

    private getOptions(): IHttpOptions {
        return {
            headers: this.getHeaders(),
            observe: "response",
            responseType: "json"
        };
    }
}
