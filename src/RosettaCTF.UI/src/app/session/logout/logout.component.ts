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

import { EventDispatcherService } from "src/app/services/event-dispatcher.service";
import { RosettaApiService } from "src/app/services/rosetta-api.service";
import { SessionProviderService } from "src/app/services/session-provider.service";
import { ISession } from "src/app/data/session";

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
    }
}
