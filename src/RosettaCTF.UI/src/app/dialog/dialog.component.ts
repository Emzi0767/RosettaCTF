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

import { Component, HostBinding, OnDestroy, ViewContainerRef, ViewChild, ComponentRef, ComponentFactoryResolver, HostListener } from "@angular/core";
import { style, state, trigger, transition, animate } from "@angular/animations";

import { IDialogComponent, IDialogData } from "../data/dialog";
import { EventDispatcherService, EventHandler, IEventTriple } from "../services/event-dispatcher.service";

@Component({
    selector: "dialog",
    templateUrl: "./dialog.component.html",
    styleUrls: ["./dialog.component.less"],
    animations: [
        trigger("toggleVisibility", [
            state("*", style({ display: "none", opacity: 0 })),
            state("showing", style({ display: "block", opacity: 0 })),
            state("hiding", style({ display: "block", opacity: 0 })),
            state("visible", style({ display: "block", opacity: 1 })),
            transition("collapsed => showing", animate("0s")),
            transition("showing => visible", animate("0.1s")),
            transition("visible => hiding", animate("0.1s")),
            transition("hiding => collapsed", animate("0s"))
        ])
    ]

})
export class DialogComponent implements OnDestroy {

    @HostBinding("@toggleVisibility")
    get toggleVisibility(): "collapsed" | "showing" | "visible" | "hiding" {
        return this.displayState;
    }

    @ViewChild("componentAnchor", { read: ViewContainerRef, static: true })
    container: ViewContainerRef;
    private component: ComponentRef<IDialogComponent>;

    private displayState: "collapsed" | "showing" | "visible" | "hiding" = "collapsed";

    private eventHandlerQueue: Array<IEventTriple<any>>;

    constructor(private eventDispatcher: EventDispatcherService,
                private resolver: ComponentFactoryResolver) {
        if (!!this.eventHandlerQueue) {
            for (const handler of this.eventHandlerQueue) {
                this.eventDispatcher.register(handler.name, handler.handler.bind(this), handler.tag);
            }
        }
    }

    ngOnDestroy(): void {
        if (!!this.component) {
            this.component.instance.dialogDismiss.unsubscribe();
            this.component.destroy();
        }

        this.eventDispatcher.unregister("dialog", `dialog:${this.constructor.name}`);
    }

    @EventHandler("dialog")
    handleDialog(data: IDialogData): void {
        this.container.clear();
        if (!!this.component) {
            this.component.destroy();
            this.component = null;
        }

        if (!!data.componentType) {
            const factory = this.resolver.resolveComponentFactory(data.componentType);
            this.component = this.container.createComponent(factory);
            this.component.instance.dialogDismiss.subscribe({
                next: (e: null) => {
                    this.displayState = "hiding";
                }
            });

            if (!!data.defaults) {
                this.component.instance.provideDefaults(data.defaults);
            }
        }

        this.displayState = "showing";
    }

    @HostListener("@toggleVisibility.done")
    animationDone(): void {
        window.setTimeout(() => {
            if (this.displayState === "showing") {
                this.displayState = "visible";
            } else if (this.displayState === "hiding") {
                this.displayState = "collapsed";
            }
        }, 1);
    }

    registerEventHandler<T>(name: string, handler: (e: T) => void): void {
        if (!this.eventHandlerQueue) {
            this.eventHandlerQueue = [];
        }

        this.eventHandlerQueue.push({ name, handler, tag: `${name}:${this.constructor.name}` });
    }

    @HostListener("document:keydown", ["$event"])
    handleKeyboardEvent(e: KeyboardEvent): void {
        // tslint:disable-next-line: deprecation
        const key = (e.keyCode || e.which);
        if (key === 27) {
            e.preventDefault();
            this.displayState = "hiding";
        }
    }
}
