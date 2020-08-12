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
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpXsrfTokenExtractor, HttpErrorResponse, HttpResponse } from "@angular/common/http";
import { Observable, of, throwError } from "rxjs";
import { catchError } from "rxjs/operators";

@Injectable({
    providedIn: "root"
})
export class RosettaHttpInterceptor implements HttpInterceptor {
    constructor(private tokenExtractor: HttpXsrfTokenExtractor) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const header = "X-Rosetta-XSRF";

        const token = this.tokenExtractor.getToken();
        if (!!token && !req.headers.has(header)) {
            req = req.clone({ headers: req.headers.set(header, token) });
        }

        return next.handle(req)
            .pipe(catchError(this.catchHttpError));
    }

    private catchHttpError(ev: HttpEvent<any>): Observable<HttpEvent<any>> {
        if (ev instanceof HttpErrorResponse) {
            return of(new HttpResponse({ body: ev.error, ...ev }));
        }

        throwError(ev);
    }
}
