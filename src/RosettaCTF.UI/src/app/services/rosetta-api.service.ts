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

import { IApiResult, IApiEventConfiguration, IApiFlag } from "../data/api";
import { ISession, ITeam, ITeamInvite } from "../data/session";
import { SessionProviderService } from "./session-provider.service";

interface IHttpOptions {
    headers?: {
        [header: string]: string | string[];
    };
    observe?: "body";
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
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    // SESSION -------------------------------------------------------
    async getSession(): Promise<IApiResult<ISession>> {
        try {
            const response = await this.http.get<IApiResult<ISession>>("/api/session", this.getOptions()).toPromise();
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async getLoginUrl(): Promise<IApiResult<string>> {
        try {
            const response = await this.http.get<IApiResult<string>>("/api/session/endpoint", this.getOptions()).toPromise();
            return response;
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
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async logout(): Promise<IApiResult<ISession>> {
        try {
            const response = await this.http.delete<IApiResult<ISession>>("/api/session", this.getOptions()).toPromise();
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    // REFRESH -------------------------------------------------------
    async refreshToken(): Promise<IApiResult<ISession>> {
        try {
            const response = await this.http.get<IApiResult<ISession>>("/api/session/refresh", this.getOptions()).toPromise();
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async refreshXsrf(): Promise<IApiResult<null>> {
        try {
            const response = await this.http.get<IApiResult<null>>("/api/config/wiggle", this.getOptions()).toPromise();
            return response;
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
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async createTeam(name: string): Promise<IApiResult<ITeam>> {
        try {
            const response = await this.http.post<IApiResult<ITeam>>("/api/team", { name }, this.getOptions()).toPromise();
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async kickTeamMember(userId: string): Promise<IApiResult<ITeam>> {
        try {
            const response = await this.http.delete<IApiResult<ITeam>>(`/api/team/members/${userId}`, this.getOptions()).toPromise();
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async inviteMember(userId: string): Promise<IApiResult<ITeam>> {
        try {
            const response = await this.http.post<IApiResult<ITeam>>(`/api/team/invite/${userId}`, this.getOptions()).toPromise();
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async acceptInvite(teamId: string): Promise<IApiResult<ITeam>> {
        try {
            const response = await this.http.patch<IApiResult<ITeam>>(`/api/team/invite/${teamId}`, this.getOptions()).toPromise();
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async getInvites(): Promise<IApiResult<ITeamInvite[]>> {
        try {
            const response = await this.http.get<IApiResult<ITeamInvite[]>>("/api/team/invite", this.getOptions()).toPromise();
            return response;
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
            observe: "body",
            responseType: "json"
        }
    }
}
