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

import { Component } from "@angular/core";
import { Observable } from "rxjs";
import { parseZone, duration } from "moment";

import { IApiEventConfiguration, ApiEventScoringMode } from "../data/api";
import { ConfigurationProviderService } from "../services/configuration-provider.service";

@Component({
    selector: "app-landing",
    templateUrl: "./landing.component.html",
    styleUrls: ["./landing.component.less"]
})
export class LandingComponent {
    configuration$: Observable<IApiEventConfiguration>;
    ApiEventScoringMode = ApiEventScoringMode;

    constructor(private configurationProvider: ConfigurationProviderService) {
        this.configuration$ = this.configurationProvider.configurationChange;
    }

    joinOrganizers(names: string[]): string {
        if (names.length === 1) {
            return names[0];
        }

        return names.slice(0, -1).join(", ") + ", and" + names.slice(-1);
    }

    computeDuration(cfg: IApiEventConfiguration): string {
        const end = parseZone(cfg.endTime);
        const start = parseZone(cfg.startTime);

        const dur = duration(end.diff(start));
        return dur.humanize();
    }
}
