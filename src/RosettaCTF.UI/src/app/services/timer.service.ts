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

import { Injectable, OnDestroy } from "@angular/core";
import { interval, Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { utc, Moment } from "moment";

@Injectable({
    providedIn: "root"
})
export class TimerService implements OnDestroy {

    private ngUnsubscribe = new Subject();

    timer$ = new Subject<Moment>();

    constructor() {
        interval(100)
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => this.timer$.next(utc()));
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
        this.timer$.complete();
    }
}
