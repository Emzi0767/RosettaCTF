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
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { Router } from "@angular/router";

import { EventDispatcherService } from "../../services/event-dispatcher.service";
import { RosettaApiService } from "../../services/rosetta-api.service";
import { SessionProviderService } from "../../services/session-provider.service";
import { ISession } from "../../data/session";
import { waitOpen, waitClose } from "../../common/waits";

@Component({
    selector: "app-logout",
    templateUrl: "./logout.component.html",
    styleUrls: ["./logout.component.less"]
})
export class LogoutComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();

    constructor(private eventDispatcher: EventDispatcherService,
                private api: RosettaApiService,
                private router: Router,
                private sessionProvider: SessionProviderService) { }

    ngOnInit(): void {
        this.sessionProvider.sessionChange
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => this.beginLogout(x));
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    private async beginLogout(session: ISession): Promise<void> {
        if (!session.isAuthenticated) {
            this.router.navigate(["/"]);
            return;
        }

        waitOpen(this.eventDispatcher);
        this.api.logout().then(x => {
            if (x.isSuccess) {
                this.sessionProvider.updateSession(x.result);
            } else {
                this.eventDispatcher.emit("error", { message: "Failed to log out.", reason: x.error });
            }

            this.api.refreshXsrf().then(_ => {
                waitClose(this.eventDispatcher);
                this.router.navigate(["/"]);
            });
        });
    }
}
