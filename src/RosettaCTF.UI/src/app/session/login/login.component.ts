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
import { Router, ActivatedRoute } from "@angular/router";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";

import { RosettaApiService } from "../../services/rosetta-api.service";
import { EventDispatcherService } from "../../services/event-dispatcher.service";
import { SessionProviderService } from "../../services/session-provider.service";
import { ISession } from "../../data/session";

@Component({
    selector: "app-login",
    templateUrl: "./login.component.html",
    styleUrls: ["./login.component.less"]
})
export class LoginComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();

    constructor(private eventDispatcher: EventDispatcherService,
                private api: RosettaApiService,
                private router: Router,
                private sessionProvider: SessionProviderService,
                private currentRoute: ActivatedRoute) { }

    ngOnInit(): void {
        const args = this.currentRoute.snapshot.paramMap;
        const provider = args.has("provider")
            ? args.get("provider")
            : null;

        this.sessionProvider.sessionChange
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => this.beginLogin(x, provider));
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    private async beginLogin(session: ISession, provider: string): Promise<void> {
        const endpoint = await this.api.getLoginUrl(provider);
        if (!endpoint.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Could not retrieve login data.", reason: endpoint.error });

            this.router.navigate(["/"]);
            return;
        }

        window.location.href = endpoint.result;
    }
}
