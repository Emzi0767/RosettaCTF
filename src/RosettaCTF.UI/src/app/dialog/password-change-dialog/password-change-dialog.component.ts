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

import { IDialogComponent, IPasswordChangeDefaults } from "../../data/dialog";
import { IUserPasswordChange } from "../../data/api";

@Component({
    selector: "app-password-change-dialog",
    templateUrl: "./password-change-dialog.component.html",
    styleUrls: ["./password-change-dialog.component.less"]
})
export class PasswordChangeDialogComponent implements IDialogComponent {

    provideModel: (id: IUserPasswordChange) => void;
    model: IUserPasswordChange = { oldPassword: null, newPassword: null, confirmPassword: null };

    dialogDismiss = new EventEmitter<null>();

    constructor() { }

    provideDefaults(defaults: IPasswordChangeDefaults): void {
        this.provideModel = defaults.provideModel;
    }

    submit(): void {
        if (this.model.oldPassword === "") {
            this.model.oldPassword = null;
        }

        this.cancel();
        this.provideModel(this.model);
    }

    cancel(): void {
        this.dialogDismiss.emit(null);
    }

    doNothing(): void { }
}
