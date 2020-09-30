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

import { IDialogComponent, IMfaDefaults } from "../../data/dialog";
import { IMfa } from "../../data/api";

@Component({
    selector: "app-mfa-dialog",
    templateUrl: "./mfa-dialog.component.html",
    styleUrls: ["./mfa-dialog.component.less"]
})
export class MfaDialogComponent implements IDialogComponent {

    authenticatorUri: string;

    provideModel: (id: IMfa) => void;
    model: IMfa = { mfaCode: null, actionToken: null };

    dialogDismiss = new EventEmitter<null>();

    constructor() { }

    provideDefaults(defaults: IMfaDefaults): void {
        this.provideModel = defaults.provideModel;
        this.model.actionToken = defaults.continuation;
    }

    submit(): void {
        this.model.mfaCode = this.model.mfaCode;
        this.cancel();
        this.provideModel(this.model);
    }

    cancel(): void {
        this.dialogDismiss.emit(null);
    }

    doNothing(): void { }
}
