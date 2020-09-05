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

import { Type, EventEmitter } from "@angular/core";

import { IChallenge, IApiFlag, ICreateTeamInvite, IUserPasswordChange, IUserPasswordRemove } from "./api";

export interface IDialogComponent {
    dialogDismiss: EventEmitter<null>;
    provideDefaults(defaults: any): void;
}

export interface IDialogData {
    componentType: Type<IDialogComponent>;
    defaults?: object;
}

export interface IErrorDialogDefaults {
    message: string;
}

export interface IInfoDialogDefaults {
    message: string;
}

export interface ISubmitFlagDialogDefaults {
    challenge: IChallenge;
    provideFlag: (flag: IApiFlag) => void;
}

export interface IInviteDialogDefaults {
    provideId: (id: ICreateTeamInvite) => void;
}

export interface ICountryDialogDefaults {
    provideCode: (code: string) => void;
    current: string | null;
}

export interface IPasswordChangeDefaults {
    provideModel: (model: IUserPasswordChange) => void;
}

export interface IPasswordRemoveDefaults {
    provideModel: (model: IUserPasswordRemove) => void;
}
