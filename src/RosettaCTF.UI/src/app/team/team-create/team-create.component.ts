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

import { RosettaApiService } from "../../services/rosetta-api.service";
import { ITeamInvite } from "../../data/session";
import { ICreateTeam } from "../../data/api";
import { EventDispatcherService } from "../../services/event-dispatcher.service";
import { SessionRefreshManagerService } from "../../services/session-refresh-manager.service";
import { waitOpen, waitClose } from "../../common/waits";

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
            waitClose(this.eventDispatcher);
        });
    }

    async createSubmit(): Promise<void> {
        waitOpen(this.eventDispatcher);
        this.hideForm = true;
        const response = await this.api.createTeam(this.model.name);
        if (response.isSuccess) {
            await this.sessionRefresh.forceUpdate();
            waitClose(this.eventDispatcher);
            this.router.navigate(["/"]);
            return;
        }

        this.eventDispatcher.emit("error", { message: "Could not create the team.", reason: response.error });
        this.hideForm = false;
    }

    async acceptInvite(id: string): Promise<void> {
        waitOpen(this.eventDispatcher);
        this.acceptingInvite = true;
        const response = await this.api.acceptInvite(id);
        if (response.isSuccess) {
            await this.sessionRefresh.forceUpdate();
            waitClose(this.eventDispatcher);
            this.router.navigate(["/"]);
            return;
        }

        this.eventDispatcher.emit("error", { message: "Could not accept invite.", reason: response.error });
        this.acceptingInvite = false;
    }
}
