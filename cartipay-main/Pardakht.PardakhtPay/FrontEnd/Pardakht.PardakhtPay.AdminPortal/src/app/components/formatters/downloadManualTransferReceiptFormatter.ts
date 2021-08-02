import {Component} from "@angular/core";
import { ICellRendererAngularComp } from "ag-grid-angular";
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import * as manualTransferActions from '../../core/actions/manualTransfer';

@Component({
    selector: 'app-download-receipt-formatter-cell',
    template: `<button style="height:unset;line-height:unset" *ngIf="params.value != null && params.value != ''" mat-icon-button title="{{'WITHDRAWAL.LIST-COLUMNS.DOWNLOAD_RECEIPT' | translate}}" (click)="downloadReceipt(params.data.id)">
                                        <mat-icon class="success">get_app</mat-icon>
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
export class DownloadManualTransferReceiptFormatterComponent implements ICellRendererAngularComp {
    public params: any;

    constructor(private store: Store<coreState.State>) {

    }

    agInit(params: any): void {
        this.params = params;
    }

    refresh(): boolean {
        return false;
    }

    downloadReceipt(id: number) {
        this.store.dispatch(new manualTransferActions.GetReceipt(id));
    }
}