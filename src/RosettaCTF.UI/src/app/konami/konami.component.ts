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

import { RosettaApiService } from "../services/rosetta-api.service";
import { EventDispatcherService } from "../services/event-dispatcher.service";
import { ErrorDialogComponent } from "../dialog/error-dialog/error-dialog.component";

@Component({
    selector: "app-konami",
    templateUrl: "./konami.component.html",
    styleUrls: ["./konami.component.less"]
})
export class KonamiComponent implements OnInit {
    enabling = true;

    constructor(private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService) { }

    ngOnInit(): void {
        this.api.enableHidden().then(x => {
            this.enabling = false;
            if (x.isSuccess) {
                return;
            }

            this.eventDispatcher.emit("dialog",
                {
                    componentType: ErrorDialogComponent,
                    defaults:
                    {
                        message: !!x.error?.message
                            ? `Could not activate hidden challenges.\n\nIf the problem persists, contact the organizers with the following error message: ${x.error.message}`
                            : "Could not activate hidden challenges.\n\nIf the problem persists, contact the organizers."
                    }
                });
        });
    }
}
