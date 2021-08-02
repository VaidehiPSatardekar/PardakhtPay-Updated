import { Action } from '@ngrx/store';
import { MerchantBankAccount } from '../../models/merchant-model';

export const GET_ACCOUNTS = '[MerchantBankAccount] Get accounts';
export const GET_ACCOUNTS_COMPLETE = '[MerchantBankAccount] Get accounts complete';
export const GET_ACCOUNTS_ERROR = '[MerchantBankAccount] Get accounts error';
export const CLEAR_ALL = '[MerchantBankAccount] Clear all';

export class GetAccounts implements Action {
    readonly type = GET_ACCOUNTS;

    constructor(public payload: number) {

    }
}

export class GetAccountsComplete implements Action {
    readonly type = GET_ACCOUNTS_COMPLETE;

    constructor(public payload: MerchantBankAccount[]) {
    }
}

export class GetAccountsError implements Action {
    readonly type = GET_ACCOUNTS_ERROR;

    constructor(public payload: string) {

    }
}

export class ClearAll implements Action {
    readonly type = CLEAR_ALL;

    constructor() {

    }
}

export type Actions =
    GetAccounts | GetAccountsComplete | GetAccountsError | ClearAll;