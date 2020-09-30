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
import { parseZone, utc } from "moment";

import { ITeam } from "../../data/session";
import { ISolve } from "../../data/api";
import { RosettaApiService } from "../../services/rosetta-api.service";
import { EventDispatcherService } from "../../services/event-dispatcher.service";
import { SessionProviderService } from "../../services/session-provider.service";
import { ConfigurationProviderService } from "../../services/configuration-provider.service";
import { waitOpen, waitClose } from "../../common/waits";

@Component({
    selector: "app-team-detail",
    templateUrl: "./team-detail.component.html",
    styleUrls: ["./team-detail.component.less"]
})
export class TeamDetailComponent implements OnInit {

    @Input()
    team: ITeam;

    showSolves = false;
    solves: ISolve[] | null = null;
    points: number | null = null;

    constructor(private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService,
                private sessionProvider: SessionProviderService,
                private configurationProvider: ConfigurationProviderService) { }

    ngOnInit(): void {
        this.initView();
    }

    private async initView(): Promise<void> {
        const config = await this.configurationProvider.getCurrent();
        const start = parseZone(config.startTime);

        if (start.isAfter(utc())) {
            waitClose(this.eventDispatcher);
            return;
        }

        if (!await this.sessionProvider.isAuthenticated()) {
            waitClose(this.eventDispatcher);
            return;
        }

        this.showSolves = true;
        await this.loadSolves();
    }

    private async loadSolves(): Promise<void> {
        const solves = await this.api.getTeamSolves(this.team.id);
        if (!solves.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Fetching solves failed.", reason: solves.error });
            this.solves = [];
            return;
        }

        this.points = solves.result.map(x => x.score)
            .reduce((acc: number, current: number) => acc + current, 0);

        this.solves = solves.result;
        waitClose(this.eventDispatcher);
    }
}
