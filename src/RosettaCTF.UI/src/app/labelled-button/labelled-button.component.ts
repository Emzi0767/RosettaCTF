import { Component, Input, EventEmitter, Output } from "@angular/core";
import { trigger, state, style, transition, animate } from "@angular/animations";

@Component({
    selector: "labelled-button",
    templateUrl: "./labelled-button.component.html",
    styleUrls: ["./labelled-button.component.less"],
    animations: [
        trigger("toggleLabelVisibility", [
            state("*", style({ display: "none", opacity: 0 })),
            state("showing", style({ display: "inline-block", opacity: 0 })),
            state("hiding", style({ display: "inline-block", opacity: 0 })),
            state("visible", style({ display: "inline-block", opacity: 1 })),
            transition("collapsed => showing", animate("0s")),
            transition("showing => visible", animate("0.1s")),
            transition("visible => hiding", animate("0.1s")),
            transition("hiding => collapsed", animate("0s"))
        ])
    ]
})
export class LabelledButtonComponent {

    @Input()
    actionId: string;

    @Input()
    actionDescription: string;

    @Input()
    actionRoute?: Array<any>;

    @Input()
    actionText: string;

    @Output()
    actionClickHandler = new EventEmitter<string>();

    labelState: "collapsed" | "showing" | "visible" | "hiding" = "collapsed";

    constructor() { }

    click(): void {
        this.actionClickHandler.emit(this.actionId);
    }

    show(): void {
        this.labelState = "showing";
    }

    hide(): void {
        this.labelState = "hiding";
    }

    animated(): void {
        window.setTimeout(() => {
            if (this.labelState === "showing") {
                this.labelState = "visible";
            } else if (this.labelState === "hiding") {
                this.labelState = "collapsed";
            }
        }, 1);
    }

}
