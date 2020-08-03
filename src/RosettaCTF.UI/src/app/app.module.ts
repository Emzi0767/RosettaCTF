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
import { HttpClientModule, HttpClientXsrfModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { MarkdownModule } from "ngx-markdown";

import { KonamiModule } from "ngx-konami";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";

import { ConfigurationProviderService } from "./services/configuration-provider.service";
import { SessionProviderService } from "./services/session-provider.service";
import { RosettaHttpInterceptor } from "./services/rosetta-http-interceptor.service";

import { NavbarComponent } from "./navbar/navbar.component";
import { FooterComponent } from "./footer/footer.component";
import { LabelledButtonComponent } from "./labelled-button/labelled-button.component";
import { DialogComponent } from "./dialog/dialog.component";
import { ErrorDialogComponent } from "./dialog/error-dialog/error-dialog.component";
import { NotFoundComponent } from "./not-found/not-found.component";
import { LandingComponent } from "./landing/landing.component";
import { KonamiComponent } from "./konami/konami.component";
import { LoginComponent } from "./session/login/login.component";
import { LogoutComponent } from "./session/logout/logout.component";
import { CallbackComponent } from "./session/callback/callback.component";

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
        KonamiComponent,
        LoginComponent,
        LogoutComponent,
        CallbackComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        KonamiModule,
        BrowserAnimationsModule,
        HttpClientModule,
        HttpClientXsrfModule.withOptions({
            cookieName: "Rosetta-XSRF",
            headerName: "X-Rosetta-XSRF"
        }),
        MarkdownModule.forRoot()
    ],
    providers: [
        ConfigurationProviderService,
        SessionProviderService,
        { provide: HTTP_INTERCEPTORS, useClass: RosettaHttpInterceptor, multi: true }
    ],
    bootstrap: [AppComponent],
    entryComponents: [
        ErrorDialogComponent
    ]
})
export class AppModule { }
