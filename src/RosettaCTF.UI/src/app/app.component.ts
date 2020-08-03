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

import { Component, OnInit, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";

import { ErrorDialogComponent } from "./dialog/error-dialog/error-dialog.component";
import { IErrorData } from "./data/error";
import { EventDispatcherService, EventHandler, IEventTriple } from "./services/event-dispatcher.service";
import { RosettaApiService } from "./services/rosetta-api.service";
import { ConfigurationProviderService } from "./services/configuration-provider.service";
import { SessionProviderService } from "./services/session-provider.service";

@Component({
    selector: "app-root",
    templateUrl: "./app.component.html",
    styleUrls: ["./app.component.less"]
})
export class AppComponent implements OnInit, OnDestroy {

    private eventHandlerQueue: Array<IEventTriple<any>>;

    constructor(private eventDispatcher: EventDispatcherService,
                private router: Router,
                private api: RosettaApiService,
                private configurationProvider: ConfigurationProviderService,
                private sessionProvider: SessionProviderService) {
        if (!!this.eventHandlerQueue) {
            for (const handler of this.eventHandlerQueue) {
                this.eventDispatcher.register(handler.name, handler.handler.bind(this), handler.tag);
            }
        }
    }

    test(): void {
        this.router.navigate(["konami"]);
    }

    ngOnInit(): void {
        this.api.testApi().then(x => {
            this.sessionProvider.init().then(() => {
                if (!x.isSuccess) {
                    this.eventDispatcher.emit("dialog", { componentType: ErrorDialogComponent, defaults: { message: "Could not establish a connection with the API." } });
                    return;
                }

                this.configurationProvider.updateConfiguration(x.result);
            });
        });
    }

    ngOnDestroy(): void {
        this.eventDispatcher.unregister("error", `error:${this.constructor.name}`);
    }

    @EventHandler("error")
    handleError(e: IErrorData): void {
        if (!!e.message) {
            console.log(`Error occured: ${e.message}`);
        }

        if (!!e.reason) {
            console.log(e.reason);
        }

        let message = e.message || "Error occured while communicating with the API.";
        if (!!e.reason && !!e.reason.message) {
            message += ` ${e.reason.message}`;
        }

        this.eventDispatcher.emit("dialog", { componentType: ErrorDialogComponent, defaults: { message } });
    }

    registerEventHandler<T>(name: string, handler: (e: T) => void): void {
        if (!this.eventHandlerQueue) {
            this.eventHandlerQueue = [];
        }

        this.eventHandlerQueue.push({ name, handler, tag: `${name}:${this.constructor.name}` });
    }

}
