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

import { ApiErrorCode, ApiStatus, IApiResult, IApiError, IApiTestResponse, IApiFlag } from "../data/api-data";

@Injectable({
    providedIn: "root"
})
export class RosettaApiService {

    constructor(private http: HttpClient) { }

    async testApi(): Promise<IApiResult<IApiTestResponse>> {
        try {
            const response = await this.http.get<IApiResult<IApiTestResponse>>("/api/test", { responseType: "json" }).toPromise();
            return response;
        } catch (ex) { }

        return {
            isSuccess: false
        };
    }
}
