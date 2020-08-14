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
import { Subject, Observable } from "rxjs";
import { takeUntil } from "rxjs/operators";

import { SessionProviderService } from "../services/session-provider.service";
import { ISession } from "../data/session";
import { ISolve } from "../data/api";
import { RosettaApiService } from "../services/rosetta-api.service";
import { EventDispatcherService } from "../services/event-dispatcher.service";
import { ErrorDialogComponent } from "../dialog/error-dialog/error-dialog.component";

@Component({
    selector: "app-user",
    templateUrl: "./user.component.html",
    styleUrls: ["./user.component.less"]
})
export class UserComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();

    session$: Observable<ISession>;
    session: ISession;

    solves: ISolve[] | null = null;

    constructor(private sessionProvider: SessionProviderService,
                private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService) {
        this.session$ = this.sessionProvider.sessionChange;
    }

    ngOnInit(): void {
        this.session$
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => { this.session = x; this.loadSolves(); });
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    private async loadSolves(): Promise<void> {
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
}
