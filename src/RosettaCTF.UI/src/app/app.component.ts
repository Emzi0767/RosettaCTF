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

import { IEmptyData, IErrorData } from "./data/events";
import { EventDispatcherService, EventHandler, IEventTriple } from "./services/event-dispatcher.service";
import { RosettaApiService } from "./services/rosetta-api.service";
import { ConfigurationProviderService } from "./services/configuration-provider.service";
import { SessionProviderService } from "./services/session-provider.service";
import { SessionRefreshManagerService } from "./services/session-refresh-manager.service";
import { hideWaitDialog, showErrorDialog, showWaitDialog } from "./common/dialog-invocation";
import { waitOpen, waitClose } from "./common/waits";

@Component({
    selector: "app-root",
    templateUrl: "./app.component.html",
    styleUrls: ["./app.component.less"]
})
export class AppComponent implements OnInit, OnDestroy {

    private eventHandlerQueue: Array<IEventTriple<any>>;

    constructor(private eventDispatcher: EventDispatcherService,
                private api: RosettaApiService,
                private configurationProvider: ConfigurationProviderService,
                private sessionProvider: SessionProviderService,
                private sessionRefresh: SessionRefreshManagerService) {
        if (!!this.eventHandlerQueue) {
            for (const handler of this.eventHandlerQueue) {
                this.eventDispatcher.register(handler.name, handler.handler.bind(this), handler.tag);
            }
        }
    }

    ngOnInit(): void {
        waitOpen(this.eventDispatcher);
        this.sessionRefresh.start();
        this.completeInit();
    }

    async completeInit(): Promise<void> {
        const x = await this.api.testApi();
        if (!x.isSuccess) {
            showErrorDialog(this.eventDispatcher, x.error, "Could not establish a connection with the API");
            return;
        }

        this.configurationProvider.updateConfiguration(x.result);
        if (!this.sessionProvider.shouldInitialize()) {
            this.sessionProvider.manuallyFinishInit();
            waitClose(this.eventDispatcher);
            return;
        }

        const y = await this.api.getSession();
        if (y.isSuccess) {
            this.sessionProvider.updateSession(y.result);
            await this.api.refreshXsrf();
        } else {
            this.sessionProvider.manuallyFinishInit();
        }

        waitClose(this.eventDispatcher);
    }

    ngOnDestroy(): void {
        this.eventDispatcher.unregister("error", `error:${this.constructor.name}`);
        this.eventDispatcher.unregister("waitOpen", `waitOpen:${this.constructor.name}`);
        this.eventDispatcher.unregister("waitClose", `waitClose:${this.constructor.name}`);
    }

    @EventHandler("error")
    handleError(e: IErrorData): void {
        const message = e.message || "Error occured while communicating with the API.";
        showErrorDialog(this.eventDispatcher, e.reason, message);
    }

    @EventHandler("waitOpen")
    handleWaitOpen(e: IEmptyData): void {
        showWaitDialog(this.eventDispatcher);
    }

    @EventHandler("waitClose")
    handleWaitClose(e: IEmptyData): void {
        hideWaitDialog(this.eventDispatcher);
    }

    registerEventHandler<T>(name: string, handler: (e: T) => void): void {
        if (!this.eventHandlerQueue) {
            this.eventHandlerQueue = [];
        }

        this.eventHandlerQueue.push({ name, handler, tag: `${name}:${this.constructor.name}` });
    }

}
