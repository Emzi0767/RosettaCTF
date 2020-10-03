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

import { Component, OnInit, ComponentRef, OnDestroy } from "@angular/core";
import { animate, state, style, transition, trigger } from "@angular/animations";

import { Observable, Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { IApiEventConfiguration } from "../data/api";
import { ConfigurationProviderService } from "../services/configuration-provider.service";
import { ISession } from "../data/session";
import { SessionProviderService } from "../services/session-provider.service";

@Component({
    selector: "app-navbar",
    templateUrl: "./navbar.component.html",
    styleUrls: ["./navbar.component.less"],
    animations: [
        trigger("toggleMenuVisibility", [
            state("*", style({ height: "0" })),
            state("showing", style({ height: "0" })),
            state("hiding", style({ height: "0" })),
            state("visible", style({ height: "384px" })),
            transition("collapsed => showing", animate("0s")),
            transition("showing => visible", animate("0.1s")),
            transition("visible => hiding", animate("0.1s")),
            transition("hiding => collapsed", animate("0s"))
        ]),
        trigger("rotateMenuArrow", [
            state("*", style({ transform: "rotate(0deg)" })),
            state("showing", style({ transform: "rotate(0deg)" })),
            state("hiding", style({ transform: "rotate(360deg)" })),
            state("visible", style({ transform: "rotate(180deg)" })),
            transition("collapsed => showing", animate("0s")),
            transition("showing => visible", animate("0.1s")),
            transition("visible => hiding", animate("0.1s")),
            transition("hiding => collapsed", animate("0s"))
        ])
    ]
})
export class NavbarComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();

    private component: ComponentRef<any>;

    menuState: "collapsed" | "showing" | "visible" | "hiding" = "collapsed";

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
        this.component.destroy();

        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    toggleMenu(): void {
        if (this.menuState === "collapsed") {
            this.menuState = "showing";
        } else if (this.menuState === "visible") {
            this.menuState = "hiding";
        }
    }

    hideMenu(): void {
        this.menuState = "hiding";
    }

    animated(): void {
        window.setTimeout(() => {
            if (this.menuState === "showing") {
                this.menuState = "visible";
            } else if (this.menuState === "hiding") {
                this.menuState = "collapsed";
            }
        }, 1);
    }
}
