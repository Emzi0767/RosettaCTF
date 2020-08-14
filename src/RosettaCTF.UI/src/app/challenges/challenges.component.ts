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
import { IChallengeCategory, IChallenge, IApiFlag } from "../data/api";
import { ErrorDialogComponent } from "../dialog/error-dialog/error-dialog.component";
import { SubmitFlagDialogComponent } from "../dialog/submit-flag-dialog/submit-flag-dialog.component";

@Component({
    selector: "app-challenges",
    templateUrl: "./challenges.component.html",
    styleUrls: ["./challenges.component.less"]
})
export class ChallengesComponent implements OnInit {

    categories: IChallengeCategory[] | null = null;
    disableButtons = false;

    constructor(private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService,
                private router: Router) { }

    ngOnInit(): void {
        this.api.getCategories().then(x => {
            if (!x.isSuccess) {
                this.eventDispatcher.emit("dialog",
                    {
                        componentType: ErrorDialogComponent,
                        defaults:
                        {
                            message: !!x.error?.message
                                ? `Fetching challenges failed.\n\nIf the problem persists, contact the organizers, with the following error message: ${x.error.message}`
                                : "Fetching challenges failed.\n\nIf the problem persists, contact the organizers."
                        }
                    });
                this.router.navigate(["/"]);
                return;
            }

            this.categories = x.result;
        });
    }

    openSolveDialog(challenge: IChallenge): void {
        this.eventDispatcher.emit("dialog",
            {
                componentType: SubmitFlagDialogComponent,
                defaults: {
                    challenge,
                    provideFlag: flag => this.submitFlag(flag)
                }
            });
    }

    private async submitFlag(flag: IApiFlag): Promise<void> {

    }
}
