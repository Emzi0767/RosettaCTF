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

import { ApiErrorCode, ApiStatus, IApiResult, IApiError, IApiEventConfiguration, IApiFlag } from "../data/api";
import { ISession } from "../data/session";

@Injectable({
    providedIn: "root"
})
export class RosettaApiService {

    constructor(private http: HttpClient) { }

    async testApi(): Promise<IApiResult<IApiEventConfiguration>> {
        try {
            const response = await this.http.get<IApiResult<IApiEventConfiguration>>("/api/test", { responseType: "json" }).toPromise();
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }

    async getSession(token: string): Promise<IApiResult<ISession>> {
        try {
            const headers = !!token
                ? { Authorization: token }
                : null;

            const response = await this.http.get<IApiResult<ISession>>("/api/session", { headers, responseType: "json" }).toPromise();
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        }
    }
}
