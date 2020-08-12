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

import { Component, Input } from "@angular/core";

import { ITeam } from "src/app/data/session";
import { ICreateTeamInvite } from "src/app/data/api";
import { RosettaApiService } from "src/app/services/rosetta-api.service";
import { EventDispatcherService } from "src/app/services/event-dispatcher.service";
import { ErrorDialogComponent } from "src/app/dialog/error-dialog/error-dialog.component";

@Component({
    selector: "app-team-manage",
    templateUrl: "./team-manage.component.html",
    styleUrls: ["./team-manage.component.less"]
})
export class TeamManageComponent {

    @Input()
    team: ITeam;

    model: ICreateTeamInvite = { id: null };
    hideForm = false;

    kickingMember = false;

    constructor(private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService) { }

    async createSubmit(): Promise<void> {
        this.hideForm = true;
        const response = await this.api.inviteMember(this.model.id);
        if (response.isSuccess) {
            this.clearForm();
            this.hideForm = false;
            return;
        }

        this.eventDispatcher.emit("dialog",
            {
                componentType: ErrorDialogComponent,
                defaults:
                {
                    message: !!response.error?.message
                        ? `Could not invite the user to the team.\n\nIf the problem persists, contact the organizers with the following error message: ${response.error.message}`
                        : "Could not invite the user to the team.\n\nIf the problem persists, contact the organizers."
                }
            });
        this.hideForm = false;
    }

    async kickMember(id: string): Promise<void> {
        this.kickingMember = true;
        const response = await this.api.kickTeamMember(id);
        if (response.isSuccess) {
            const team = await this.api.getTeam();
            if (team.isSuccess) {
                this.team = team.result;
            }
            
            this.kickingMember = false;
            return;
        }

        this.eventDispatcher.emit("dialog",
            {
                componentType: ErrorDialogComponent,
                defaults:
                {
                    message: !!response.error?.message
                        ? `Could not remove the member from the team.\n\nIf the problem persists, contact the organizers with the following error message: ${response.error.message}`
                        : "Could not remove the member from the team.\n\nIf the problem persists, contact the organizers."
                }
            });
        this.kickingMember = false;
    }

    private clearForm(): void {
        this.model.id = null;
    }
}
