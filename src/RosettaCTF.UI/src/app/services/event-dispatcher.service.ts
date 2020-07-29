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

interface IEventHandler<T> {
    function: (args: T) => void;
    tag: string;
}

interface IEvent<T> {
    name: string;
    handlers: Array<IEventHandler<T>>;
}

@Injectable({
    providedIn: "root"
})
export class EventDispatcherService {

    private events: { [k: string]: IEvent<any> };

    constructor() {
        this.events = {};
    }

    register<T>(name: string, handler: (args: T) => void, tag: string): void {
        if (!this.events[name]) {
            this.events[name] = {
                name,
                handlers: []
            };
        }

        const event = this.events[name];
        event.handlers.push({ function: handler, tag });
    }

    unregister<T>(name: string, tag: string): void {
        if (!this.events[name]) {
            return;
        }

        const event = this.events[name];
        const index = event.handlers.findIndex(x => x.tag === tag);
        if (index !== -1) {
            event.handlers.splice(index, 1);
        }
    }

    emit<T>(name: string, args: T): void {
        if (!this.events[name]) {
            return;
        }

        const event = this.events[name] as IEvent<T>;
        for (const handler of event.handlers) {
            handler.function(args);
        }
    }
}

export interface IEventDispatcherProvider {
    registerEventHandler<T>(name: string, handler: (e: T) => void): void;
}

export interface IEventTriple<T> {
    name: string;
    handler: (e: T) => void;
    tag: string;
}

export function EventHandler<TArgs>(name: string) {
    // tslint:disable-next-line: max-line-length
    return function<T extends (e: TArgs) => void>(target: IEventDispatcherProvider, propertyKey: string | symbol, descriptor: TypedPropertyDescriptor<T>): void {
        target.registerEventHandler<TArgs>(name, descriptor.value);
    };
}
