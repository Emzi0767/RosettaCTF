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
import { ReplaySubject } from "rxjs";

import { IApiEventConfiguration } from "../data/api";

@Injectable({
    providedIn: "root"
})
export class ConfigurationProviderService implements OnDestroy {
    configurationChange: ReplaySubject<IApiEventConfiguration> = new ReplaySubject<IApiEventConfiguration>(1);
    private currentConfiguration: IApiEventConfiguration;

    private init$: Promise<void>;
    private initResolve;

    constructor() {
        this.init$ = new Promise<void>((resolve, reject) => { this.initResolve = resolve; });
    }

    async hasAdditionalFeatures(): Promise<boolean> {
        await this.init$;
        // tslint:disable-next-line: no-bitwise
        return (this.currentConfiguration.flags & 1) === 1;
    }

    async getCurrent(): Promise<IApiEventConfiguration> {
        await this.init$;
        return this.currentConfiguration;
    }

    updateConfiguration(data: IApiEventConfiguration): void {
        this.currentConfiguration = data;
        this.configurationChange.next(data);

        if (!!this.initResolve) {
            this.initResolve();
            this.initResolve = null;
        }
    }

    ngOnDestroy(): void {
        this.configurationChange.complete();
    }
}
