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

import { Component, OnInit, OnDestroy, } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";

import { IApiEventConfiguration } from "../data/api";
import { ISession } from "../data/session";
import { SessionProviderService } from "../services/session-provider.service";
import { ConfigurationProviderService } from "../services/configuration-provider.service";

@Component({
    selector: "app-footer",
    templateUrl: "./footer.component.html",
    styleUrls: ["./footer.component.less"]
})
export class FooterComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();

    configuration$: Observable<IApiEventConfiguration>;
    configuration: IApiEventConfiguration;

    session$: Observable<ISession>;
    session: ISession;

    constructor(private configurationProvider: ConfigurationProviderService,
                private sessionProvider: SessionProviderService) {
        this.configuration$ = this.configurationProvider.configurationChange;
        this.session$ = this.sessionProvider.sessionChange;
    }

    ngOnInit(): void {
        this.configuration$
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => { this.configuration = x; });

        this.session$
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => { this.session = x; });
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
}
