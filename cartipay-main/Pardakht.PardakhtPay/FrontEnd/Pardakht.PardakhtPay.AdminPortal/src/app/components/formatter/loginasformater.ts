import { Component } from "@angular/core";
import { ICellRendererAngularComp } from "ag-grid-angular";
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import * as userActions from '../../core/actions/user';
import { LoginAsStaffUserRequest } from '../../core/models/user-management.model';

@Component({
    selector: 'app-download-receipt-formatter-cell',
    template: `<button style="height:unset;line-height:unset" mat-icon-button class="mat-accent" title="Login As"  [ngClass]="'systemActionButtonColor'"
    (click)="loginAs(params.data.username)">
                                        <mat-icon >how_to_reg</mat-icon>
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
export class LoginAsFormatterComponent implements ICellRendererAngularComp {
    public params: any;

    constructor(private store: Store<coreState.State>) {

    }
    agInit(params: any): void {
        this.params = params;
    }

    refresh(): boolean {
        return false;
    }

    loginAs(username: string) {
        var model = new LoginAsStaffUserRequest();
        model.userName = username;
        this.store.dispatch(new userActions.LoginAsStaffUser(model));
    }
}