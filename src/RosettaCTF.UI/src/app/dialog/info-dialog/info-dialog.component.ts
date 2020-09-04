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

import { IInfoDialogDefaults } from "src/app/data/dialog";

@Component({
    selector: "app-info-dialog",
    templateUrl: "./info-dialog.component.html",
    styleUrls: ["./info-dialog.component.less"]
})
export class InfoDialogComponent {

    message: string = null;

    dialogDismiss = new EventEmitter<null>();

    provideDefaults(defaults: IInfoDialogDefaults): void {
        if (!defaults || !defaults.message) {
            this.message = "This should say something.";
            return;
        }

        this.message = defaults.message;
    }

    dismiss(): void {
        this.dialogDismiss.emit(null);
    }
}
