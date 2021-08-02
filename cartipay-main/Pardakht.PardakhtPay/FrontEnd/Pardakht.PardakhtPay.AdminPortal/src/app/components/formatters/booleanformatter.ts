import {Component} from "@angular/core";
import {ICellRendererAngularComp} from "ag-grid-angular";

@Component({
    selector: 'child-cell',
    template: `<mat-icon class="success" *ngIf="params.value === true || params.value === 'true'">check</mat-icon> <mat-icon class="danger" *ngIf="params.value === false || params.value === 'false'">close</mat-icon>`,
    styles: [
        `.success {
            color:green;
        }
        .danger{
            color:red;
        }
        `
    ]
})
export class BooleanFormatterComponent implements ICellRendererAngularComp {
    public params: any;

    agInit(params: any): void {
        this.params = params;
    }

    refresh(): boolean {
        return false;
    }
}