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
import { Observable, Subject, merge } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { Moment, parseZone, utc } from "moment";
import { HumanizeDuration, HumanizeDurationLanguage } from "humanize-duration-ts";

import { RosettaApiService } from "../services/rosetta-api.service";
import { EventDispatcherService } from "../services/event-dispatcher.service";
import { IChallengeCategory, IChallenge, IApiFlag, IApiEventConfiguration } from "../data/api";
import { ErrorDialogComponent } from "../dialog/error-dialog/error-dialog.component";
import { SubmitFlagDialogComponent } from "../dialog/submit-flag-dialog/submit-flag-dialog.component";
import { ConfigurationProviderService } from "../services/configuration-provider.service";
import { TimerService } from "../services/timer.service";
import { waitOpen, waitClose } from "../common/waits";

@Component({
    selector: "app-challenges",
    templateUrl: "./challenges.component.html",
    styleUrls: ["./challenges.component.less"]
})
export class ChallengesComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();
    private timerStop = new Subject();

    private humanizer = new HumanizeDuration(new HumanizeDurationLanguage());

    configuration$: Observable<IApiEventConfiguration>;
    configuration: IApiEventConfiguration = null;
    eventEnd: Moment = null;

    endCountdown: string | boolean | null = null;
    categories: IChallengeCategory[] | null = null;
    disableButtons = false;
    hideButtons = false;

    constructor(private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService,
                private router: Router,
                private configurationProvider: ConfigurationProviderService,
                private timer: TimerService) {
        this.configuration$ = this.configurationProvider.configurationChange;
    }

    ngOnInit(): void {
        waitOpen(this.eventDispatcher);
        this.configuration$
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => { this.configuration = x; this.eventEnd = parseZone(x.endTime); this.recomputeButtonVisibility(); });

        this.timer.timer$
            .pipe(takeUntil(merge(this.ngUnsubscribe, this.timerStop)))
            .subscribe(x => this.processCountdown(x));

        this.api.getCategories().then(x => {
            if (!x.isSuccess) {
                this.eventDispatcher.emit("error", { message: "Fetching challenges failed.", reason: x.error });
                this.router.navigate(["/"]);
                return;
            }

            this.categories = x.result;
            waitClose(this.eventDispatcher);
        });
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();

        if (!!this.timerStop) {
            this.stopTimer();
        }
    }

    openSolveDialog(challenge: IChallenge): void {
        this.eventDispatcher.emit("dialog", {
            componentType: SubmitFlagDialogComponent,
            defaults: {
                challenge,
                provideFlag: (flag: IApiFlag) => this.submitFlag(flag)
            }
        });
    }

    private async submitFlag(flag: IApiFlag): Promise<void> {
        waitOpen(this.eventDispatcher);
        this.disableButtons = true;

        const response = await this.api.submitSolve(flag);
        if (!response.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Submitting flag failed.", reason: response.error });
            this.disableButtons = false;
            return;
        } else if (!response.result) {
            // tslint:disable-next-line: max-line-length
            this.eventDispatcher.emit("dialog", { componentType: ErrorDialogComponent, defaults: { message: "Your response was incorrect." } });
            this.disableButtons = false;
            return;
        }

        const categories = await this.api.getCategories();
        if (!categories.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Fetching challenges failed.", reason: categories.error });
            this.router.navigate(["/"]);
            return;
        }

        this.categories = categories.result;
        this.disableButtons = false;
        waitClose(this.eventDispatcher);
    }

    private recomputeButtonVisibility(): void {
        const start = parseZone(this.configuration.endTime);
        const now = utc();

        this.hideButtons = now.isAfter(start);
    }

    private stopTimer(): void {
        this.timerStop.next();
        this.timerStop.complete();
        this.timerStop = null;
    }

    private processCountdown(x: Moment): void {
        if (this.eventEnd == null) {
            return;
        }

        if (x.isAfter(this.eventEnd)) {
            this.stopTimer();
            this.endCountdown = true;
        } else {
            this.endCountdown = this.humanizer.humanize(this.eventEnd.diff(x) * -1, {
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
