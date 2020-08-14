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

import { Injectable } from "@angular/core";
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";

import { ConfigurationProviderService } from "./configuration-provider.service";

@Injectable({
    providedIn: "root"
})
export class KonamiGuardService {
    constructor(private configurationProvider: ConfigurationProviderService,
                private router: Router) { }

    async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        const result = await this.configurationProvider.hasAdditionalFeatures();
        if (!result) {
            console.log("Event flags specified no special features; aborting...");
        }

        return result;
    }
}
