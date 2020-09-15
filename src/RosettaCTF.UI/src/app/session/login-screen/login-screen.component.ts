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

import { RosettaApiService } from "../../services/rosetta-api.service";
import { ILoginSettings, IUserLogin, IMfa } from "../../data/api";
import { EventDispatcherService } from "../../services/event-dispatcher.service";
import { SessionProviderService } from "../../services/session-provider.service";
import { MfaDialogComponent } from "../../dialog/mfa-dialog/mfa-dialog.component";

@Component({
    selector: "app-login-screen",
    templateUrl: "./login-screen.component.html",
    styleUrls: ["./login-screen.component.less"]
})
export class LoginScreenComponent implements OnInit {

    loginModel: IUserLogin = { username: null, password: null };

    lockControls = false;
    loginSettings: ILoginSettings = null;

    constructor(private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService,
                private router: Router,
                private sessionProvider: SessionProviderService) { }

    ngOnInit(): void {
        this.doInit();
    }

    async loginSubmit(): Promise<void> {
        this.lockControls = true;
        const loginResult = await this.api.login(this.loginModel);
        if (!loginResult.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Could not log in. Please try again.", reason: loginResult.error });

            this.lockControls = false;
            return;
        }

        if (loginResult.result.mfaContinuation !== undefined && loginResult.result.mfaContinuation !== null) {
            this.eventDispatcher.emit("dialog", {
                    componentType: MfaDialogComponent,
                    defaults: {
                        provideModel: (model: IMfa) => this.do2fa(model),
                        continuation: loginResult.result.mfaContinuation
                    }
                });
        } else {
            this.sessionProvider.updateSession(loginResult.result);
            await this.api.refreshXsrf();
            this.router.navigate(["/"]);
        }
    }

    private async doInit(): Promise<void> {
        const data = await this.api.getLoginSettings();
        if (!data.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Could not retrieve login settings.", reason: data.error });

            this.router.navigate(["/"]);
            return;
        }

        this.loginSettings = data.result;
    }

    private async do2fa(model: IMfa): Promise<void> {
        const result = await this.api.completeMfaLogin(model);
        if (!result.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Could not log in. Please try again.", reason: result.error });

            this.lockControls = false;
            return;
        }

        this.sessionProvider.updateSession(result.result);
        await this.api.refreshXsrf();
        this.router.navigate(["/"]);
    }
}
