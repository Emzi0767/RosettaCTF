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
import { ReplaySubject } from "rxjs";

import { ISession } from "../data/session";

@Injectable({
    providedIn: "root"
})
export class SessionProviderService {
    sessionChange: ReplaySubject<ISession> = new ReplaySubject<ISession>(1);

    constructor() {
        this.updateSession({
            authenticated: false,
            token: null,
            user: null,
            team: null
        });
    }

    updateSession(data: ISession): void {
        this.sessionChange.next(data);
    }
}
