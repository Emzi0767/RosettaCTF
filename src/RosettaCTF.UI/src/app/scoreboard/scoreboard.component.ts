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

import { RosettaApiService } from "../services/rosetta-api.service";
import { EventDispatcherService } from "../services/event-dispatcher.service";
import { IScoreboardEntry } from "../data/api";
import { waitOpen, waitClose } from "../common/waits";

@Component({
    selector: "app-scoreboard",
    templateUrl: "./scoreboard.component.html",
    styleUrls: ["./scoreboard.component.less"]
})
export class ScoreboardComponent implements OnInit {

    scoreboard: IScoreboardEntry[] | null = null;

    constructor(private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService,
                private router: Router) { }

    ngOnInit(): void {
        waitOpen(this.eventDispatcher);
        this.api.getScoreboard().then(x => {
            if (!x.isSuccess) {
                this.eventDispatcher.emit("error", { message: "Fetching scoreboard failed.", reason: x.error });
                this.router.navigate(["/"]);
                return;
            }

            this.scoreboard = x.result;
            waitClose(this.eventDispatcher);
        });
    }
}
