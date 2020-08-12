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

import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";

import { RosettaApiService } from "src/app/services/rosetta-api.service";
import { ITeamInvite } from "src/app/data/session";
import { ICreateTeam } from "src/app/data/api";
import { EventDispatcherService } from "src/app/services/event-dispatcher.service";
import { ErrorDialogComponent } from "../../dialog/error-dialog/error-dialog.component";
import { SessionRefreshManagerService } from "src/app/services/session-refresh-manager.service";

@Component({
    selector: "app-team-create",
    templateUrl: "./team-create.component.html",
    styleUrls: ["./team-create.component.less"]
})
export class TeamCreateComponent implements OnInit {

    invites: ITeamInvite[] | null = null;

    model: ICreateTeam = { name: null };
    hideForm = false;

    acceptingInvite = false;

    constructor(private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService,
                private router: Router,
                private sessionRefresh: SessionRefreshManagerService) { }

    ngOnInit(): void {
        this.api.getInvites().then(x => {
            this.invites = x.isSuccess
                ? x.result
                : [];
        });
    }

    async createSubmit(): Promise<void> {
        this.hideForm = true;
        const response = await this.api.createTeam(this.model.name);
        if (response.isSuccess) {
            await this.sessionRefresh.forceUpdate();
            this.router.navigate(["/"]);
            return;
        }

        this.eventDispatcher.emit("dialog",
            {
                componentType: ErrorDialogComponent,
                defaults:
                {
                    message: !!response.error?.message
                        ? `Could not create the team.\n\nIf the problem persists, contact the organizers with the following error message: ${response.error.message}`
                        : "Could not create the team.\n\nIf the problem persists, contact the organizers."
                }
            });
        this.hideForm = false;
    }

    async acceptInvite(id: string): Promise<void> {
        this.acceptingInvite = true;
        const response = await this.api.acceptInvite(id);
        if (response.isSuccess) {
            await this.sessionRefresh.forceUpdate();
            this.router.navigate(["/"]);
            return;
        }

        this.eventDispatcher.emit("dialog",
            {
                componentType: ErrorDialogComponent,
                defaults:
                {
                    message: !!response.error?.message
                        ? `Could not accept invite.\n\nIf the problem persists, contact the organizers with the following error message: ${response.error.message}`
                        : "Could not accept invite.\n\nIf the problem persists, contact the organizers."
                }
            });
        this.acceptingInvite = false;
    }
}
