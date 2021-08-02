import {Component} from "@angular/core";
import {ICellRendererAngularComp} from "ag-grid-angular";

@Component({
    selector: 'app-boolean-inverse-cell',
    template: `<mat-icon class="danger" *ngIf="params.value === true || params.value === 'true'">close</mat-icon> <mat-icon class="success" *ngIf="params.value === false || params.value === 'false'">check</mat-icon>`,
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
export class BooleanInverseFormatterComponent implements ICellRendererAngularComp {
    public params: any;

    agInit(params: any): void {
        this.params = params;
    }

    refresh(): boolean {
        return false;
    }
}