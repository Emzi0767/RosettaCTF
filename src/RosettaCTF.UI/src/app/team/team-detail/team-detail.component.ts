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

import { Component, OnInit, Input } from "@angular/core";

import { ITeam } from "../../data/session";
import { ISolve, IChallenge } from "../../data/api";
import { RosettaApiService } from "../../services/rosetta-api.service";
import { EventDispatcherService } from "../../services/event-dispatcher.service";
import { ErrorDialogComponent } from "../../dialog/error-dialog/error-dialog.component";

@Component({
    selector: "app-team-detail",
    templateUrl: "./team-detail.component.html",
    styleUrls: ["./team-detail.component.less"]
})
export class TeamDetailComponent implements OnInit {

    @Input()
    team: ITeam;

    solves: ISolve[] | null = null;
    points: number | null = null;

    constructor(private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService) { }

    ngOnInit(): void {
        this.loadSolves();
    }

    private async loadSolves(): Promise<void> {
        const solves = await this.api.getTeamSolves(this.team.id);
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

        this.points = solves.result.map(x => x.score)
            .reduce((acc: number, current: number) => acc + current);

        this.solves = solves.result;
    }
}
