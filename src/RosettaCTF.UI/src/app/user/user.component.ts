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
import { Subject, Observable, zip } from "rxjs";
import { takeUntil, take } from "rxjs/operators";
import { parseZone, utc } from "moment";

import { SessionProviderService } from "../services/session-provider.service";
import { ISession, IUser } from "../data/session";
// tslint:disable-next-line: max-line-length
import { ISolve, IApiEventConfiguration, ICountry, IExternalAccount, ILoginSettings, IUserPasswordRemove, IUserPasswordChange, IUserSudo, IMfa, IMfaDisable } from "../data/api";
import { RosettaApiService } from "../services/rosetta-api.service";
import { EventDispatcherService } from "../services/event-dispatcher.service";
import { ConfigurationProviderService } from "../services/configuration-provider.service";
import { CountryChangeDialogComponent } from "../dialog/country-change-dialog/country-change-dialog.component";
import { PasswordRemoveDialogComponent } from "../dialog/password-remove-dialog/password-remove-dialog.component";
import { PasswordChangeDialogComponent } from "../dialog/password-change-dialog/password-change-dialog.component";
import { InfoDialogComponent } from "../dialog/info-dialog/info-dialog.component";
import { SudoDialogComponent } from "../dialog/sudo-dialog/sudo-dialog.component";
import { MfaEnableDialogComponent } from "../dialog/mfa-enable-dialog/mfa-enable-dialog.component";
import { MfaDisableDialogComponent } from "../dialog/mfa-disable-dialog/mfa-disable-dialog.component";
import { waitOpen, waitClose } from "../common/waits";

@Component({
    selector: "app-user",
    templateUrl: "./user.component.html",
    styleUrls: ["./user.component.less"]
})
export class UserComponent implements OnInit, OnDestroy {

    private ngUnsubscribe = new Subject();

    session$: Observable<ISession>;
    session: ISession = null;
    user: IUser;
    country: ICountry;

    configuration$: Observable<IApiEventConfiguration>;
    configuration: IApiEventConfiguration = null;

    solves: ISolve[] | null = null;
    showSolves = false;
    changingCountry = false;

    connections: IExternalAccount[] = null;
    removingConnection = false;

    loginSettings: ILoginSettings = null;

    changingPassword = false;

    constructor(private sessionProvider: SessionProviderService,
                private api: RosettaApiService,
                private eventDispatcher: EventDispatcherService,
                private configurationProvider: ConfigurationProviderService) {
        this.session$ = this.sessionProvider.sessionChange;
        this.configuration$ = this.configurationProvider.configurationChange;
    }

    ngOnInit(): void {
        waitOpen(this.eventDispatcher);

        this.session$
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => {
                this.session = x;
                this.user = x.user;

                if (this.configuration !== null) {
                    this.country = this.configuration.countries.find(c => c.code === this.user.country);
                }
            });

        this.configuration$
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe(x => { this.configuration = x; });

        zip(
            this.session$.pipe(take(1)),
            this.configuration$.pipe(take(1))
        )
            .pipe(take(1))
            .subscribe(() => {
                this.recomputeSolveVisibility();
                this.loadSolves();
                this.country = this.configuration.countries.find(c => c.code === this.user.country);
            });

        this.getProviders();
        this.loadConnections();
    }

    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    openCountryChange(): void {
        this.eventDispatcher.emit("dialog", {
            componentType: CountryChangeDialogComponent,
            defaults: {
                provideCode: (code: string) => this.doCountryChange(code),
                current: this.country?.code
            }
        });
    }

    openPasswordChange(): void {
        this.eventDispatcher.emit("dialog", {
            componentType: PasswordChangeDialogComponent,
            defaults: { provideModel: (model: IUserPasswordChange) => this.doChangePassword(model) }
        });
    }

    openPasswordRemove(): void {
        this.eventDispatcher.emit("dialog", {
            componentType: PasswordRemoveDialogComponent,
            defaults: { provideModel: (model: IUserPasswordRemove) => this.doRemovePassword(model) }
        });
    }

    open2faEnable(): void {
        this.eventDispatcher.emit("dialog", {
            componentType: SudoDialogComponent,
            defaults: { provideModel: (model: IUserSudo) => this.do2faEnable(model) }
        });
    }

    open2faDisable(): void {
        this.eventDispatcher.emit("dialog", {
            componentType: MfaDisableDialogComponent,
            defaults: { provideModel: (model: IMfaDisable) => this.do2faDisable(model) }
        });
    }

    async removeConnection(provider: string): Promise<void> {
        waitOpen(this.eventDispatcher);
        this.removingConnection = true;
        const result = await this.api.removeConnection(provider);
        if (!result.result) {
            this.eventDispatcher.emit("error", { message: "Removing connection failed.", reason: result.error });
            this.removingConnection = false;
            return;
        }

        await this.loadConnections();
        this.removingConnection = false;
    }

    async getProviders(): Promise<void> {
        const data = await this.api.getLoginSettings();
        if (!data.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Could not retrieve login settings.", reason: data.error });
            return;
        }

        this.loginSettings = data.result;
        if (this.connections !== null) {
            waitClose(this.eventDispatcher);
        }
    }

    private async doCountryChange(code: string): Promise<void> {
        waitOpen(this.eventDispatcher);
        const session = await this.api.updateCountry(code);
        if (!session.result) {
            this.eventDispatcher.emit("error", { message: "Updating country failed.", reason: session.error });
            return;
        }

        this.sessionProvider.updateSession(session.result);
        waitClose(this.eventDispatcher);
    }

    private async loadSolves(): Promise<void> {
        if (!this.showSolves) {
            return;
        }

        const solves = await this.api.getTeamSolves(this.session.user.team.id);
        if (!solves.isSuccess) {
            this.solves = [];
            return;
        }

        this.solves = solves.result.filter(x => x.user.id === this.session.user.id);
    }

    private async loadConnections(): Promise<void> {
        const connections = await this.api.getConnections();
        this.connections = connections.isSuccess
            ? connections.result
            : [];

        if (this.loginSettings !== null) {
            waitClose(this.eventDispatcher);
        }
    }

    private async doRemovePassword(model: IUserPasswordRemove): Promise<void> {
        waitOpen(this.eventDispatcher);
        this.changingPassword = true;

        const result = await this.api.removePassword(model);
        if (!result.isSuccess || !result.result) {
            this.eventDispatcher.emit("error", { message: "Removing password failed.", reason: result.error });
        } else {
            this.eventDispatcher.emit("dialog", {
                componentType: InfoDialogComponent,
                defaults: { message: "Password removal successful." }
            });

            this.sessionProvider.updateSession(result.result);
        }

        this.changingPassword = false;
    }

    private async doChangePassword(model: IUserPasswordChange): Promise<void> {
        waitOpen(this.eventDispatcher);
        this.changingPassword = true;

        const result = await this.api.changePassword(model);
        if (!result.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Changing password failed.", reason: result.error });
        } else {
            this.eventDispatcher.emit("dialog", {
                componentType: InfoDialogComponent,
                defaults: { message: "Password change successful." }
            });

            this.sessionProvider.updateSession(result.result);
        }

        this.changingPassword = false;
    }

    private async do2faEnable(model: IUserSudo): Promise<void> {
        waitOpen(this.eventDispatcher);
        this.changingPassword = true;

        const result = await this.api.startMfaEnable(model);
        if (!result.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Enabling 2FA failed.", reason: result.error });
        } else {
            this.eventDispatcher.emit("dialog", {
                componentType: MfaEnableDialogComponent,
                defaults: {
                    provideModel: (xmodel: IMfa, backups: string[]) => this.doComplete2faEnable(xmodel, backups),
                    continuation: result.result.continuation,
                    authenticatorUri: result.result.authenticatorUri,
                    backups: result.result.recoveryCodes
                }
            });
        }

        this.changingPassword = false;
    }

    private async doComplete2faEnable(model: IMfa, backups: string[]): Promise<void> {
        waitOpen(this.eventDispatcher);
        this.changingPassword = true;

        const result = await this.api.completeMfaEnable(model);
        if (!result.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Enabling 2FA failed.", reason: result.error });
        } else {
            this.eventDispatcher.emit("dialog", {
                componentType: InfoDialogComponent,
                defaults: {
                    message: "2FA enabled successfully.\n\n2FA recovery codes (they need to be used in that exact order): "
                        + backups.join(", ")
                }
            });

            this.sessionProvider.updateSession(result.result);
        }

        this.changingPassword = false;
    }

    private async do2faDisable(model: IMfaDisable): Promise<void> {
        waitOpen(this.eventDispatcher);
        this.changingPassword = true;

        const result = await this.api.disableMfa(model);
        if (!result.isSuccess) {
            this.eventDispatcher.emit("error", { message: "Disabling 2FA failed.", reason: result.error });
        } else {
            this.eventDispatcher.emit("dialog", {
                componentType: InfoDialogComponent,
                defaults: { message: "2FA disabled successfully." }
            });

            this.sessionProvider.updateSession(result.result);
        }

        this.changingPassword = false;
    }

    private recomputeSolveVisibility(): void {
        const start = parseZone(this.configuration.startTime);
        const now = utc();

        this.showSolves = now.isAfter(start) && this.session.user.team !== null;
    }
}
