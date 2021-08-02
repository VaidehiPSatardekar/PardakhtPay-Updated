import { Component, EventEmitter } from "@angular/core";
import { ICellRendererAngularComp } from "ag-grid-angular";
import { DeleteLoginDialogComponent } from "../delete-login-dialog/delete-login-dialog.component";
import { MatDialog } from "@angular/material";

@Component({
    selector: 'app-delete-button-formatter-cell',
    template: `<button style="height:unset;line-height:unset" *ngIf="params.value != null && params.value != ''" mat-icon-button title="{{'BANK-LOGIN.LIST-COLUMNS.DELETE' | translate}}" (click)="onclick(params.data.id)">
                                        <mat-icon class="danger">delete</mat-icon>
                                    </button>`,
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
export class DeleteButtonFormatterComponent implements ICellRendererAngularComp {
    public params: any;
    //public onButtonClick : EventEmitter<any> = new EventEmitter();

    constructor(public dialog: MatDialog) {

    }

    agInit(params: any): void {
        this.params = params;
    }

    refresh(): boolean {
        return false;
    }

    onclick(id: number) {
        const dialogRef = this.dialog.open(DeleteLoginDialogComponent, {
            width: '250px',
            data: id
        });

        dialogRef.afterClosed().subscribe(result => {
            //if (result == true) {
            //    console.log('delete button');
            //    this.onButtonClick.emit(id);
            //}
            if (result == true) {
                if (this.params.onButtonClick instanceof Function) {
                    this.params.onButtonClick(id);
                }
            }
        });
    }
}