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
import { ReplaySubject } from "rxjs";

import { ISession } from "../data/session";
import { RosettaApiService } from "./rosetta-api.service";

@Injectable({
    providedIn: "root"
})
export class SessionProviderService {
    sessionChange: ReplaySubject<ISession> = new ReplaySubject<ISession>(1);

    constructor(private api: RosettaApiService) {
        this.sessionChange.next({
            isAuthenticated: false,
            token: null,
            user: null
        });
    }

    async init(): Promise<void> {
        const token = this.getStoredToken();
        if (!!token) {
            const response = await this.api.getSession(token);
            if (response.isSuccess) {
                this.updateSession(response.result);
                return;
            }
        }
    }

    updateSession(data: ISession): void {
        this.sessionChange.next(data);
        this.updateStoredToken(data.token);
    }

    private updateStoredToken(token: string): void {
        if (!!token) {
            localStorage.setItem("token", token);
        } else {
            localStorage.removeItem("token");
        }
    }

    private getStoredToken(): string | null {
        return localStorage.getItem("token");
    }
}
