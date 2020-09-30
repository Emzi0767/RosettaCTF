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

import { Component, EventEmitter } from "@angular/core";

import { IMfa } from "../../data/api";
import { IMfaEnableDefaults, IDialogComponent } from "../../data/dialog";

@Component({
    selector: "app-mfa-enable-dialog",
    templateUrl: "./mfa-enable-dialog.component.html",
    styleUrls: ["./mfa-enable-dialog.component.less"]
})
export class MfaEnableDialogComponent implements IDialogComponent {

    authenticatorUri: string = null;
    backups: string[] = null;
    secret: string = null;
    accountLabel: string = null;
    displayCode = true;

    provideModel: (id: IMfa, backups: string[]) => void;
    model: IMfa = { mfaCode: null, actionToken: null };

    dialogDismiss = new EventEmitter<null>();

    constructor() { }

    provideDefaults(defaults: IMfaEnableDefaults): void {
        this.provideModel = defaults.provideModel;
        this.model.actionToken = defaults.continuation;
        this.authenticatorUri = defaults.authenticatorUri;
        this.backups = defaults.backups;

        const uri = new URL(this.authenticatorUri);
        this.secret = uri.searchParams.get("secret");
        this.accountLabel = `${uri.searchParams.get("issuer")}`;

        const secretIndices = [];
        for (let i = 0; i < this.secret.length; i += 4) {
            secretIndices.push(this.secret.substr(i, 4));
        }
        this.secret = secretIndices.join(" ");
    }

    submit(): void {
        this.model.mfaCode = this.model.mfaCode;
        this.cancel();
        this.provideModel(this.model, this.backups);
    }

    cancel(): void {
        this.dialogDismiss.emit(null);
    }

    doNothing(): void { }

    toggleCodeDisplay(): void {
        this.displayCode = !this.displayCode;
    }
}
