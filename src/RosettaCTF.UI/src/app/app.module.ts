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

import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { HttpClientModule, HttpClientXsrfModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { MarkdownModule } from "ngx-markdown";

import { QRCodeModule } from "angular2-qrcode";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";

import { ConfigurationProviderService } from "./services/configuration-provider.service";
import { SessionProviderService } from "./services/session-provider.service";
import { TimerService } from "./services/timer.service";
import { RosettaHttpInterceptor } from "./services/rosetta-http-interceptor.service";
import { SessionRefreshManagerService } from "./services/session-refresh-manager.service";
import { CompositeRouteGuardService } from "./services/composite-route-guard.service";
import { AuthenticationGuardService } from "./services/authentication-guard.service";
import { EventStartGuardService } from "./services/event-start-guard.service";

import { NavbarComponent } from "./navbar/navbar.component";
import { FooterComponent } from "./footer/footer.component";
import { LabelledButtonComponent } from "./labelled-button/labelled-button.component";
import { DialogComponent } from "./dialog/dialog.component";
import { ErrorDialogComponent } from "./dialog/error-dialog/error-dialog.component";
import { NotFoundComponent } from "./not-found/not-found.component";
import { LandingComponent } from "./landing/landing.component";
import { LoginComponent } from "./session/login/login.component";
import { LogoutComponent } from "./session/logout/logout.component";
import { CallbackComponent } from "./session/callback/callback.component";
import { TeamComponent } from "./team/team.component";
import { TeamCreateComponent } from "./team/team-create/team-create.component";
import { NotLoggedInComponent } from "./session/not-logged-in/not-logged-in.component";
import { UserComponent } from "./user/user.component";
import { TeamManageComponent } from "./team/team-manage/team-manage.component";
import { ScoreboardComponent } from "./scoreboard/scoreboard.component";
import { ChallengesComponent } from "./challenges/challenges.component";
import { SubmitFlagDialogComponent } from "./dialog/submit-flag-dialog/submit-flag-dialog.component";
import { ChallengeDetailComponent } from "./challenges/challenge-detail/challenge-detail.component";
import { TeamDetailComponent } from "./team/team-detail/team-detail.component";
import { NotYetComponent } from "./not-yet/not-yet.component";
import { LoginScreenComponent } from "./session/login-screen/login-screen.component";
import { RegisterComponent } from "./session/register/register.component";
import { FieldValueMatchValidatorDirective } from "./services/field-value-match-validator.directive";
import { InfoDialogComponent } from "./dialog/info-dialog/info-dialog.component";
import { InviteDialogComponent } from "./dialog/invite-dialog/invite-dialog.component";
import { CountryChangeDialogComponent } from "./dialog/country-change-dialog/country-change-dialog.component";
import { PasswordRemoveDialogComponent } from "./dialog/password-remove-dialog/password-remove-dialog.component";
import { PasswordChangeDialogComponent } from "./dialog/password-change-dialog/password-change-dialog.component";
import { SudoDialogComponent } from "./dialog/sudo-dialog/sudo-dialog.component";
import { MfaEnableDialogComponent } from "./dialog/mfa-enable-dialog/mfa-enable-dialog.component";
import { MfaDisableDialogComponent } from "./dialog/mfa-disable-dialog/mfa-disable-dialog.component";
import { MfaDialogComponent } from "./dialog/mfa-dialog/mfa-dialog.component";
import { WaitDialogComponent } from "./dialog/wait-dialog/wait-dialog.component";

@NgModule({
    declarations: [
        AppComponent,
        NavbarComponent,
        FooterComponent,
        LabelledButtonComponent,
        DialogComponent,
        ErrorDialogComponent,
        NotFoundComponent,
        LandingComponent,
        LoginComponent,
        LogoutComponent,
        CallbackComponent,
        TeamComponent,
        TeamCreateComponent,
        NotLoggedInComponent,
        UserComponent,
        TeamManageComponent,
        ScoreboardComponent,
        ChallengesComponent,
        SubmitFlagDialogComponent,
        ChallengeDetailComponent,
        TeamDetailComponent,
        NotYetComponent,
        LoginScreenComponent,
        RegisterComponent,
        FieldValueMatchValidatorDirective,
        InfoDialogComponent,
        InviteDialogComponent,
        CountryChangeDialogComponent,
        PasswordRemoveDialogComponent,
        PasswordChangeDialogComponent,
        SudoDialogComponent,
        MfaEnableDialogComponent,
        MfaDisableDialogComponent,
        MfaDialogComponent,
        WaitDialogComponent
    ],
    imports: [
        BrowserModule,
        FormsModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        HttpClientModule,
        HttpClientXsrfModule.withOptions({
            cookieName: "Rosetta-XSRF",
            headerName: "X-Rosetta-XSRF"
        }),
        MarkdownModule.forRoot(),
        QRCodeModule
    ],
    providers: [
        ConfigurationProviderService,
        SessionProviderService,
        TimerService,
        SessionRefreshManagerService,
        { provide: HTTP_INTERCEPTORS, useClass: RosettaHttpInterceptor, multi: true },
        CompositeRouteGuardService,
        AuthenticationGuardService,
        EventStartGuardService
    ],
    bootstrap: [AppComponent],
    entryComponents: [
        ErrorDialogComponent,
        SubmitFlagDialogComponent
    ]
})
export class AppModule { }
