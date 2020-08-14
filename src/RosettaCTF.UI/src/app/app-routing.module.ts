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

import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { AuthenticationGuardService } from "./services/authentication-guard.service";
import { KonamiGuardService } from "./services/konami-guard.service";

import { NotFoundComponent } from "./not-found/not-found.component";
import { LandingComponent } from "./landing/landing.component";
import { KonamiComponent } from "./konami/konami.component";
import { LoginComponent } from "./session/login/login.component";
import { LogoutComponent } from "./session/logout/logout.component";
import { CallbackComponent } from "./session/callback/callback.component";
import { NotLoggedInComponent } from "./session/not-logged-in/not-logged-in.component";
import { TeamComponent } from "./team/team.component";
import { UserComponent } from "./user/user.component";
import { ScoreboardComponent } from "./scoreboard/scoreboard.component";
import { ChallengesComponent } from "./challenges/challenges.component";
import { ChallengeDetailComponent } from "./challenges/challenge-detail/challenge-detail.component";

const routes: Routes = [
    {
        path: "konami",
        component: KonamiComponent,
        canActivate: [ AuthenticationGuardService, KonamiGuardService ]
    },
    {
        path: "session/login",
        component: LoginComponent
    },
    {
        path: "session/logout",
        component: LogoutComponent
    },
    {
        path: "session/unauthorized",
        component: NotLoggedInComponent
    },
    {
        path: "session/callback",
        component: CallbackComponent
    },
    {
        path: "team",
        component: TeamComponent,
        canActivate: [ AuthenticationGuardService ]
    },
    {
        path: "team/:id",
        component: TeamComponent,
        canActivate: [ AuthenticationGuardService ]
    },
    {
        path: "profile",
        component: UserComponent,
        canActivate: [ AuthenticationGuardService ]
    },
    {
        path: "scoreboard",
        component: ScoreboardComponent
    },
    {
        path: "challenges",
        component: ChallengesComponent,
        canActivate: [ AuthenticationGuardService ]
    },
    {
        path: "challenges/:id",
        component: ChallengeDetailComponent,
        canActivate: [ AuthenticationGuardService ]
    },
    {
        path: "",
        component: LandingComponent
    },
    {
        path: "**",
        component: NotFoundComponent
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
