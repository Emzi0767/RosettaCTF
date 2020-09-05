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

import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subject, Observable, zip } from "rxjs";
import { takeUntil, take } from "rxjs/operators";
import { parseZone, utc } from "moment";

import { SessionProviderService } from "../services/session-provider.service";
import { ISession, IUser } from "../data/session";
import { ISolve, IApiEventConfiguration, ICountry } from "../data/api";
import { RosettaApiService } from "../services/rosetta-api.service";
import { EventDispatcherService } from "../services/event-dispatcher.service";
import { ErrorDialogComponent } from "../dialog/error-dialog/error-dialog.component";
import { ConfigurationProviderService } from "../services/configuration-provider.service";
import { CountryChangeDialogComponent } from "../dialog/country-change-dialog/country-change-dialog.component";

@Component({
    selector: "app-user",
    templateUrl: "./user.component.html",
    styleUrls: ["./user.component.less"]
})
export class UserComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();

    session$: Observable<ISession>;
    session: ISession = null;
    user: IUser;
    country: ICountry;

    configuration$: Observable<IApiEventConfiguration>;
    configuration: IApiEventConfiguration = null;

    solves: ISolve[] | null = null;
    showSolves = false;
    changingCountry = false;

    constructor(private sessionProvider: SessionProviderService,
                private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService,
                private configurationProvider: ConfigurationProviderService) {
        this.session$ = this.sessionProvider.sessionChange;
        this.configuration$ = this.configurationProvider.configurationChange;
    }

    ngOnInit(): void {
        this.session$
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => {
                this.session = x;
                this.user = x.user;

                if (this.configuration !== null) {
                    this.country = this.configuration.countries.find(c => c.code === this.user.country);
                }
            });

        this.configuration$
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => { this.configuration = x; });

        zip(
            this.session$.pipe(take(1)),
            this.configuration$.pipe(take(1))
        )
            .pipe(take(1))
            .subscribe(() => {
                this.recomputeSolveVisibility();
                this.loadSolves();
                this.country = this.configuration.countries.find(c => c.code === this.user.country);
            });
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    openCountryChange(): void {
        this.eventDispatcher.emit("dialog",
            {
                componentType: CountryChangeDialogComponent,
                defaults:
                {
                    provideCode: (code: string) => this.doCountryChange(code),
                    current: this.country?.code
                }
            });
    }

    private async doCountryChange(code: string): Promise<void> {
        const session = await this.api.updateCountry(code);
        if (!session.result) {
            this.eventDispatcher.emit("dialog",
                {
                    componentType: ErrorDialogComponent,
                    defaults:
                    {
                        message: !!session.error?.message
                            ? `Updating country failed.\n\nIf the problem persists, contact the organizers, with the following error message: ${session.error.message}`
                            : "Updating country failed.\n\nIf the problem persists, contact the organizers."
                    }
                });

            return;
        }

        this.sessionProvider.updateSession(session.result);
    }

    private async loadSolves(): Promise<void> {
        if (!this.showSolves) {
            return;
        }

        const solves = await this.api.getTeamSolves(this.session.user.team.id);
        if (!solves.isSuccess) {
            this.eventDispatcher.emit("dialog",
                {
                    componentType: ErrorDialogComponent,
                    defaults:
                    {
                        message: !!solves.error?.message
                            ? `Fetching solves failed.\n\nIf the problem persists, contact the organizers, with the following error message: ${solves.error.message}`
                            : "Fetching solves failed.\n\nIf the problem persists, contact the organizers."
                    }
                });
            this.solves = [];
            return;
        }

        this.solves = solves.result.filter(x => x.user.id === this.session.user.id);
    }

    private recomputeSolveVisibility(): void {
        const start = parseZone(this.configuration.startTime);
        const now = utc();

        this.showSolves = now.isAfter(start) && this.session.user.team !== null;
    }
}
