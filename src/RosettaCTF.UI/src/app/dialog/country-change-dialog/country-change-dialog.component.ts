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

import { Component, EventEmitter, OnInit } from "@angular/core";

import { IDialogComponent, ICountryDialogDefaults } from "../../data/dialog";
import { ConfigurationProviderService } from "../../services/configuration-provider.service";
import { IApiEventConfiguration } from "../../data/api";

@Component({
    selector: "app-country-change-dialog",
    templateUrl: "./country-change-dialog.component.html",
    styleUrls: ["./country-change-dialog.component.less"]
})
export class CountryChangeDialogComponent implements IDialogComponent, OnInit {

    configuration: IApiEventConfiguration = null;

    provideCode: (id: string) => void;
    model: { code: string | null } = { code: null };

    dialogDismiss = new EventEmitter<null>();

    constructor(private configurationProvider: ConfigurationProviderService) { }

    ngOnInit(): void {
        this.initConfig();
    }

    provideDefaults(defaults: ICountryDialogDefaults): void {
        this.provideCode = defaults.provideCode;
        this.model.code = defaults.current ?? "";
    }

    submit(): void {
        this.cancel();
        this.provideCode(this.model.code === "" ? null : this.model.code);
    }

    cancel(): void {
        this.dialogDismiss.emit(null);
    }

    doNothing(): void { }

    private async initConfig(): Promise<void> {
        this.configuration = await this.configurationProvider.getCurrent();
    }
}
