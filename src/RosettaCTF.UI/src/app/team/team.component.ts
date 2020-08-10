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

import { RosettaApiService } from "../services/rosetta-api.service";
import { ITeam } from "../data/session";

@Component({
    selector: "app-team",
    templateUrl: "./team.component.html",
    styleUrls: ["./team.component.less"]
})
export class TeamComponent implements OnInit {

    hasTeam: boolean | null = null;
    team: ITeam | null = null;

    constructor(private api: RosettaApiService) { }

    ngOnInit(): void {
        this.doInit();
    }

    private async doInit(): Promise<void> {
        const team = await this.api.getTeam();
        this.hasTeam = team.isSuccess && !!team.result;
        this.team = team.result;
    }
}
