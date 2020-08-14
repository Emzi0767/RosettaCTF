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
import { ActivatedRoute, Router } from "@angular/router";

import { RosettaApiService } from "src/app/services/rosetta-api.service";
import { IChallenge, ISolve } from "src/app/data/api";
import { EventDispatcherService } from "src/app/services/event-dispatcher.service";
import { ErrorDialogComponent } from "src/app/dialog/error-dialog/error-dialog.component";

@Component({
    selector: "app-challenge-detail",
    templateUrl: "./challenge-detail.component.html",
    styleUrls: ["./challenge-detail.component.less"]
})
export class ChallengeDetailComponent implements OnInit {

    challenge: IChallenge | null = null;
    solves: ISolve[] | null = null;

    constructor(private api: RosettaApiService,
                private currentRoute: ActivatedRoute,
                private router: Router,
                private eventDispatcher: EventDispatcherService) { }

    ngOnInit(): void {
        this.loadChallenge();
    }

    private async loadChallenge(): Promise<void> {
        const args = this.currentRoute.snapshot.paramMap;
        const id = args.has("id") ? args.get("id") : null;
        if (id === null) {
            this.eventDispatcher.emit("dialog",
                {
                    componentType: ErrorDialogComponent,
                    defaults:
                    {
                        message: "Missing challenge ID."
                    }
                });
            this.router.navigate(["/challenges"]);
        }

        const challenge = await this.api.getChallenge(id);
        if (!challenge.isSuccess) {
            this.eventDispatcher.emit("dialog",
                {
                    componentType: ErrorDialogComponent,
                    defaults:
                    {
                        message: !!challenge.error?.message
                            ? `Fetching challenge failed.\n\nIf the problem persists, contact the organizers, with the following error message: ${challenge.error.message}`
                            : "Fetching challenge failed.\n\nIf the problem persists, contact the organizers."
                    }
                });
            this.router.navigate(["/challenges"]);
        }

        this.challenge = challenge.result;

        const solves = await this.api.getSolves(id);
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

        this.solves = solves.result;
    }
}
