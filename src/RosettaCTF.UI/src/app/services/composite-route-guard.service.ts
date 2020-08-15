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

import { Injectable, Injector, Type } from "@angular/core";
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from "@angular/router";
import { Observable } from "rxjs";

import { ICompositeGuardRouteData } from "../data/route";

@Injectable({
    providedIn: "root"
})
export class CompositeRouteGuardService implements CanActivate {
    constructor(private injector: Injector) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Promise<boolean | UrlTree> {
        const data = route.data as ICompositeGuardRouteData;
        if (!data.routeGuards || data.routeGuards.length === 0) {
            return true;
        }

        return this.testCanActivate(data.routeGuards, route, state);
    }

    // tslint:disable-next-line: max-line-length
    private async testCanActivate(guards: Type<any>[], route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean | UrlTree> {
        const guard = this.injector.get(guards[0]);
        const next = guards.slice(1);
        let result = guard.canActivate(route, state);

        if (result instanceof Observable) {
            result = result.toPromise();
        }

        if (result instanceof Promise) {
            result = await result;
        }

        if (result !== true || next.length === 0) {
            return result;
        }

        return await this.testCanActivate(next, route, state);
    }
}
