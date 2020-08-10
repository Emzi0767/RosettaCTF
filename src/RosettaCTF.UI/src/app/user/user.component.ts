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
import { Subject, Observable } from "rxjs";
import { takeUntil } from "rxjs/operators";

import { SessionProviderService } from "../services/session-provider.service";
import { ISession } from "../data/session";

@Component({
    selector: "app-user",
    templateUrl: "./user.component.html",
    styleUrls: ["./user.component.less"]
})
export class UserComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();

    session$: Observable<ISession>;
    session: ISession;

    constructor(private sessionProvider: SessionProviderService) {
        this.session$ = this.sessionProvider.sessionChange;
    }

    ngOnInit(): void {
        this.session$
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => { this.session = x; });
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
}
