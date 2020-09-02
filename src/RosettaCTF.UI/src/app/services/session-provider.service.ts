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

import { Injectable, OnDestroy } from "@angular/core";
import { ReplaySubject } from "rxjs";

import { ISession } from "../data/session";

@Injectable({
    providedIn: "root"
})
export class SessionProviderService implements OnDestroy {
    sessionChange: ReplaySubject<ISession> = new ReplaySubject<ISession>(1);
    private currentSession: ISession;

    private init$: Promise<void>;
    private initResolve;

    constructor() {
        this.init$ = new Promise<void>((resolve, reject) => { this.initResolve = resolve; });
        this.currentSession = {
            isAuthenticated: false,
            token: this.getStoredToken(),
            user: null
        };
        this.sessionChange.next(this.currentSession);
    }

    async isAuthenticated(): Promise<boolean> {
        await this.init$;
        return this.currentSession.isAuthenticated;
    }

    updateSession(data: ISession): void {
        this.currentSession = data;
        this.sessionChange.next(data);
        this.updateStoredToken(data.token);

        if (!!this.initResolve) {
            this.initResolve();
            this.initResolve = null;
        }
    }

    getToken(): string | null {
        return this.currentSession?.token;
    }

    shouldInitialize(): boolean {
        return !!this.currentSession?.token;
    }

    manuallyFinishInit(): void {
        if (!!this.initResolve) {
            this.initResolve();
            this.initResolve = null;
        }
    }

    ngOnDestroy(): void {
        this.sessionChange.complete();
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
