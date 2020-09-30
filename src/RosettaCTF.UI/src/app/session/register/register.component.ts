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

import { Component } from "@angular/core";
import { Router } from "@angular/router";

import { IUserRegister } from "../../data/api";
import { RosettaApiService } from "../../services/rosetta-api.service";
import { EventDispatcherService } from "../../services/event-dispatcher.service";
import { InfoDialogComponent } from "../../dialog/info-dialog/info-dialog.component";
import { waitOpen } from "../../common/waits";

@Component({
    selector: "app-register",
    templateUrl: "./register.component.html",
    styleUrls: ["./register.component.less"]
})
export class RegisterComponent {

    lockControls = false;
    registerModel: IUserRegister = { username: null, password: null, confirmPassword: null };

    constructor(private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService,
                private router: Router) { }

    async registerSubmit(): Promise<void> {
        waitOpen(this.eventDispatcher);
        this.lockControls = true;
        const registerResult = await this.api.register(this.registerModel);
        if (!registerResult.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Could not register. Please try again.", reason: registerResult.error });

            this.lockControls = false;
            return;
        }

        this.eventDispatcher.emit("dialog", {
            componentType: InfoDialogComponent,
            defaults: { message: "Registration complete. You may now log in." }
        });
        this.router.navigate(["/session", "login"]);
    }
}
