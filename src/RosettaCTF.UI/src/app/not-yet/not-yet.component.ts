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
import { Subject, merge } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { Moment, parseZone } from "moment";
import { Router } from "@angular/router";
import { HumanizeDuration, HumanizeDurationLanguage } from "humanize-duration-ts";

import { ConfigurationProviderService } from "../services/configuration-provider.service";
import { TimerService } from "../services/timer.service";

@Component({
    selector: "app-not-yet",
    templateUrl: "./not-yet.component.html",
    styleUrls: ["./not-yet.component.less"]
})
export class NotYetComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();
    private timerStop = new Subject();

    private humanizer = new HumanizeDuration(new HumanizeDurationLanguage());

    startCountdown: string | boolean | null = null;
    eventStart: Moment = null;

    constructor(private configurationProvider: ConfigurationProviderService,
                private timer: TimerService,
                private router: Router) { }

    ngOnInit(): void {
        this.configurationProvider.configurationChange
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => { this.eventStart = parseZone(x.startTime); });

        this.timer.timer$
            .pipe(takeUntil(merge(this.ngUnsubscribe, this.timerStop)))
            .subscribe(x => this.processCountdown(x));
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();

        if (!!this.timerStop) {
            this.stopTimer();
        }
    }

    private stopTimer(): void {
        this.timerStop.next();
        this.timerStop.complete();
        this.timerStop = null;
    }

    private processCountdown(x: Moment): void {
        if (this.eventStart == null) {
            return;
        }

        if (x.isAfter(this.eventStart)) {
            this.stopTimer();
            this.startCountdown = true;
            this.router.navigate(["/challenges"]);
        } else {
            this.startCountdown = this.humanizer.humanize(x.diff(this.eventStart) * -1, {
                units: ["d", "h", "m", "s"],
                round: true,
                largest: 3,
                unitMeasures: {
                    d: 86_400_000,
                    h:  3_600_000,
                    m:     60_000,
                    s:      1_000
                }
            });
        }
    }
}
