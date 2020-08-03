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
import { EventDispatcherService } from "src/app/services/event-dispatcher.service";
import { ErrorDialogComponent } from "src/app/dialog/error-dialog/error-dialog.component";

@Component({
    selector: "app-login",
    templateUrl: "./login.component.html",
    styleUrls: ["./login.component.less"]
})
export class LoginComponent implements OnInit {

    constructor(private eventDispatcher: EventDispatcherService,
                private api: RosettaApiService,
                private router: Router) { }

    ngOnInit(): void {
        this.api.getLoginUrl().then(x => {
            if (!x.isSuccess) {
                this.eventDispatcher.emit("dialog",
                    {
                        componentType: ErrorDialogComponent,
                        defaults:
                        {
                            message: !!x.error?.message
                                ? `Could not retrieve login data. Please try again. If the problem persists, contact the organizers, with the following error message: ${x.error.message} (${x.error.code})`
                                : "Could not retrieve login data. Please try again. If the problem persists, contact the organizers."
                        }
                    });
                this.router.navigate(["/"]);
                return;
            }

            window.location.href = x.result;
        });
    }
}
