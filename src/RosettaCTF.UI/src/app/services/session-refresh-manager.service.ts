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
import { Subject, interval } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { parseZone, utc } from "moment";

import { SessionProviderService } from "./session-provider.service";
import { RosettaApiService } from "./rosetta-api.service";
import { ISession } from "../data/session";

@Injectable({
    providedIn: "root"
})
export class SessionRefreshManagerService implements OnDestroy {

    private ngUnsubscribe = new Subject();
    private stopTimer: Subject<void> | null = null;

    constructor(private sessionProvider: SessionProviderService,
                private api: RosettaApiService) { }

    start(): void {
        this.sessionProvider.sessionChange
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => this.updateTimer(x));
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    private updateTimer(session: ISession): void {
        if (!!this.stopTimer) {
            this.stopTimer.next();
            this.stopTimer.complete();
            this.stopTimer = null;
        }

        if (!session.isAuthenticated || !session.expiresAt) {
            return;
        }

        this.stopTimer = new Subject();
        const expiry = parseZone(session.expiresAt)
            .subtract(30, "seconds")
            .diff(utc(), "milliseconds");

        interval(expiry).pipe(takeUntil(this.stopTimer))
            .subscribe(x => { this.doRefresh(); });
    }

    private async doRefresh(): Promise<void> {
        const session = await this.api.refreshToken();
        this.sessionProvider.updateSession(session.isSuccess ? session.result : { isAuthenticated: false, token: null, user: null });
    }
}
