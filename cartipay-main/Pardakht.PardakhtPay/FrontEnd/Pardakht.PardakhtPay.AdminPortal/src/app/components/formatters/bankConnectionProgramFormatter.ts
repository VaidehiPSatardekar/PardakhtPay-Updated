import { Component, EventEmitter } from "@angular/core";
import { ICellRendererAngularComp } from "ag-grid-angular";
import { DeleteLoginDialogComponent } from "../delete-login-dialog/delete-login-dialog.component";
import { MatDialog } from "@angular/material";

@Component({
    selector: 'app-icon-button-formatter-cell',
    template: `<button style="height:unset;line-height:unset" *ngIf="params.data.bankConnectionProgram == 'MobileBanking' && params.value != '' && (params.allow == undefined || params.allow == true) && allowed()" mat-icon-button title="{{params.data.bankConnectionProgram}}" (click)="onclick(params.data.id)">
               <mat-icon class="{{params.iconClass}} mat-icon ng-tns-c42-28 material-icons">mobile_screen_share</mat-icon>
               </button>
               <button style="height:unset;line-height:unset" *ngIf="params.data.bankConnectionProgram == 'InternetBanking' && params.value != '' && (params.allow == undefined || params.allow == true) && allowed()" mat-icon-button title="InternetBanking" (click)="onclick(params.data.id)">
               <mat-icon class="{{params.iconClass}} mat-icon ng-tns-c42-28 material-icons">computer</mat-icon>
               </button>`,
    styles: [
        `.success {
            color:green;
        }
        .danger{
            color:red;
        }
        .warning{
            color:yellow !important;
        }
        `
    ]
})
export class BankConnectionProgramFormatterComponent implements ICellRendererAngularComp {
    public params: any;

    constructor(public dialog: MatDialog) {

    }

    agInit(params: any): void {
        this.params = params;
    }

    refresh(): boolean {
        return false;
    }

    onclick(id: number) {
        if (this.params.onButtonClick instanceof Function) {
            this.params.onButtonClick(id);
        }
    }

    allowed() : boolean {
        if (this.params.allowed == undefined) {
            return true;
        }

        return this.params.allowed(this.params);
    }
}