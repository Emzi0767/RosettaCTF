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

import { IDialogComponent, ISubmitFlagDialogDefaults } from "../../data/dialog";
import { IChallenge, IApiFlag } from "../../data/api";

@Component({
    selector: "app-submit-flag-dialog",
    templateUrl: "./submit-flag-dialog.component.html",
    styleUrls: ["./submit-flag-dialog.component.less"]
})
export class SubmitFlagDialogComponent implements IDialogComponent {

    challenge: IChallenge;
    provideFlag: (flag: IApiFlag) => void;
    model: IApiFlag = { id: null, flag: null };

    dialogDismiss = new EventEmitter<null>();

    constructor() { }

    provideDefaults(defaults: ISubmitFlagDialogDefaults): void {
        this.challenge = defaults.challenge;
        this.provideFlag = defaults.provideFlag;
        this.model.id = this.challenge.id;
    }

    submit(): void {
        this.cancel();
        this.provideFlag(this.model);
    }

    cancel(): void {
        this.dialogDismiss.emit(null);
    }

    doNothing(): void { }
}
