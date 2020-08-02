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

import { Component, OnInit, EventEmitter } from "@angular/core";

import { IDialogComponent, IErrorDialogDefaults } from "src/app/data/dialog";

@Component({
    selector: "app-error-dialog",
    templateUrl: "./error-dialog.component.html",
    styleUrls: ["./error-dialog.component.less"]
})
export class ErrorDialogComponent implements IDialogComponent {

    message: string = null;

    dialogDismiss = new EventEmitter<null>();

    provideDefaults(defaults: IErrorDialogDefaults): void {
        if (!defaults || !defaults.message) {
            this.message = "An unknown error occured.";
            return;
        }

        this.message = defaults.message;
    }

    dismiss(): void {
        this.dialogDismiss.emit(null);
    }
}
