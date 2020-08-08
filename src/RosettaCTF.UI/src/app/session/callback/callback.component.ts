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
import { Router, ActivatedRoute } from "@angular/router";

import { EventDispatcherService } from "src/app/services/event-dispatcher.service";
import { RosettaApiService } from "src/app/services/rosetta-api.service";
import { ErrorDialogComponent } from "src/app/dialog/error-dialog/error-dialog.component";
import { SessionProviderService } from "src/app/services/session-provider.service";

@Component({
    selector: "app-callback",
    templateUrl: "./callback.component.html",
    styleUrls: ["./callback.component.less"]
})
export class CallbackComponent implements OnInit {

    constructor(private eventDispatcher: EventDispatcherService,
                private api: RosettaApiService,
                private router: Router,
                private currentRoute: ActivatedRoute,
                private sessionProvider: SessionProviderService) { }

    ngOnInit(): void {
        const args = this.currentRoute.snapshot.queryParamMap;
        if (args.has("error") || !args.has("code") || !args.has("state")) {
            this.eventDispatcher.emit("dialog",
                {
                    componentType: ErrorDialogComponent,
                    defaults:
                    {
                        message: args.get("error") === "access_denied"
                            ? "You refused to authorize the application. You need to allow access for login to work."
                            : "Discord login failed. Please try again.\n\nIf the problem persists, contact the organizers."
                    }
                });
            this.router.navigate(["/"]);
            return;
        }

        this.api.completeLogin(args.get("code"), args.get("state")).then(x => {
            if (!x.isSuccess) {
                this.eventDispatcher.emit("dialog",
                    {
                        componentType: ErrorDialogComponent,
                        defaults:
                        {
                            message: !!x.error?.message
                                ? `Discord login failed. Please try again.\n\nIf the problem persists, contact the organizers, with the following error message: ${x.error.message}`
                                : "Discord login failed. Please try again.\n\nIf the problem persists, contact the organizers."
                        }
                    });
                this.router.navigate(["/"]);
                return;
            }

            this.sessionProvider.updateSession(x.result);
            this.api.refreshXsrf().then(_ => { this.router.navigate(["/"]); });
        });
    }
}
