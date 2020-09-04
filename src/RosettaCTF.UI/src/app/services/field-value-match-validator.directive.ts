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

import { AbstractControl, Validator, NG_VALIDATORS, ValidationErrors } from "@angular/forms";
import { Input, Directive } from "@angular/core";

@Directive({
    // tslint:disable-next-line: directive-selector
    selector: "[matchFieldValue]",
    providers: [{
        provide: NG_VALIDATORS,
        useExisting: FieldValueMatchValidatorDirective,
        multi: true
    }]
})
export class FieldValueMatchValidatorDirective implements Validator {

    @Input("matchFieldValue")
    matchFields: AbstractControl[];

    validate(control: AbstractControl): ValidationErrors | null {
        if (this.matchFields.length < 2) {
            return null;
        }

        const vals = this.matchFields.map(x => x.value);
        return !vals.every(x => x === vals[0])
            ? { valueMismatch: true }
            : null;
    }
}
