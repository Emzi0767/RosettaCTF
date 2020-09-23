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

import { Component, Input, OnInit, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";
import { Observable, Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { parseZone, utc } from "moment";

import { ITeam } from "../../data/session";
import { ICreateTeamInvite, ISolve, IApiEventConfiguration } from "../../data/api";
import { RosettaApiService } from "../../services/rosetta-api.service";
import { EventDispatcherService } from "../../services/event-dispatcher.service";
import { ConfigurationProviderService } from "../../services/configuration-provider.service";
import { InviteDialogComponent } from "../../dialog/invite-dialog/invite-dialog.component";
import { SessionRefreshManagerService } from "../../services/session-refresh-manager.service";

@Component({
    selector: "app-team-manage",
    templateUrl: "./team-manage.component.html",
    styleUrls: ["./team-manage.component.less"]
})
export class TeamManageComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();

    @Input()
    team: ITeam;

    configuration$: Observable<IApiEventConfiguration>;
    configuration: IApiEventConfiguration;

    solves: ISolve[] | null = null;
    points: number | null = null;

    hideForm = false;
    kickingMember = false;
    showSolves = false;

    constructor(private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService,
                private configurationProvider: ConfigurationProviderService,
                private router: Router,
                private sessionRefresh: SessionRefreshManagerService) {
        this.configuration$ = this.configurationProvider.configurationChange;
    }

    ngOnInit(): void {
        this.configuration$
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => { this.configuration = x; this.recomputeSolveVisibility(); this.loadSolves(); });
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    openSubmit(): void {
        this.eventDispatcher.emit("dialog",
            {
                componentType: InviteDialogComponent,
                defaults: { provideId: (x: ICreateTeamInvite) => this.createSubmit(x) }
            });
    }

    async createSubmit(inv: ICreateTeamInvite): Promise<void> {
        this.hideForm = true;
        const response = await this.api.inviteMember(inv.id);
        if (response.isSuccess) {
            this.hideForm = false;
            return;
        }

        this.eventDispatcher.emit("error", { message: "Could not invite the user to the team.", reason: response.error });
        this.hideForm = false;
    }

    async kickMember(id: string): Promise<void> {
        this.kickingMember = true;
        const response = await this.api.kickTeamMember(id);
        if (response.isSuccess) {
            if (response.result === null) {
                await this.sessionRefresh.forceUpdate();
                this.router.navigate(["/profile"]);
                return;
            }

            this.team = response.result;
            this.kickingMember = false;
            return;
        }

        this.eventDispatcher.emit("error", { message: "Could not remove the member from the team.", reason: response.error });
        this.kickingMember = false;
    }

    private async loadSolves(): Promise<void> {
        if (!this.showSolves) {
            return;
        }

        const solves = await this.api.getTeamSolves(this.team.id);
        if (!solves.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Fetching solves failed.", reason: solves.error });
            this.solves = [];
            return;
        }

        this.points = solves.result.map(x => x.score)
            .reduce((acc: number, current: number) => acc + current, 0);

        this.solves = solves.result;
    }

    private recomputeSolveVisibility(): void {
        const start = parseZone(this.configuration.startTime);
        const now = utc();

        this.showSolves = now.isAfter(start);
    }
}
