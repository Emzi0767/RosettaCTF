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

import { EventDispatcherService } from "../services/event-dispatcher.service";
import { IApiError } from "../data/api";
import { ErrorDialogComponent } from "../dialog/error-dialog/error-dialog.component";
import { WaitDialogComponent } from "../dialog/wait-dialog/wait-dialog.component";
import { IDialogData } from "../data/dialog";

export function showErrorDialog(eventDispatcher: EventDispatcherService, error: IApiError, message: string): void {
    eventDispatcher.emit("dialog", {
        componentType: ErrorDialogComponent,
        defaults: {
            message: !!error?.message
                ? `${message}\n\nIf the problem persists, contact the organizers, with the following error message: ${error.message} (error code: ${error.code})`
                : `${message}\n\nIf the problem persists, contact the organizers.`
        }
    } as IDialogData);
}

export function showWaitDialog(eventDispatcher: EventDispatcherService): void {
    eventDispatcher.emit("dialog", {
        componentType: WaitDialogComponent,
        inescapeable: true
    } as IDialogData);
}

export function hideWaitDialog(eventDispatcher: EventDispatcherService): void {
    eventDispatcher.emit("dialogDismiss", { });
}
