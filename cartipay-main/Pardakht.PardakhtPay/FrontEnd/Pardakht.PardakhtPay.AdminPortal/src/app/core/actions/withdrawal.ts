import { Action } from '@ngrx/store';
import { Withdrawal, WithdrawalSearchArgs, WithdrawalSearch, WithdrawalCreate, WithdrawalReceipt, WithdrawalTransferHistory } from '../../models/withdrawal';
import { ListSearchResponse } from '../../models/types';


export const SEARCH = '[Withdrawal] Search';
export const SEARCH_COMPLETE = '[Withdrawal] Search Complete';
export const SEARCH_ERROR = '[Withdrawal] Search Error';

export const GET_DETAILS = '[Withdrawal] Get Details';
export const GET_DETAILS_COMPLETE = '[Withdrawal] Get Details Complete';
export const GET_DETAILS_ERROR = '[Withdrawal] Get Details Error';

export const CREATE = '[Withdrawal] Create';
export const CREATE_COMPLETE = '[Withdrawal] Create Complete';
export const CREATE_ERROR = '[Withdrawal] Create Error';

export const EDIT = '[Withdrawal] Edit';
export const EDIT_COMPLETE = '[Withdrawal] Edit Complete';
export const EDIT_ERROR = '[Withdrawal] Edit Error';

export const GET_RECEIPT = '[Withdrawal] Get Receipt';
export const GET_RECEIPT_COMPLETE = '[Withdrawal] Get Receipt Complete';
export const GET_RECEIPT_ERROR = '[Withdrawal] Get Receipt Error';

export const CANCEL = '[Withdrawal] Cancel Withdrawal';
export const CANCEL_COMPLETE = '[Withdrawal] Cancel Withdrawal Complete';
export const CANCEL_ERROR = '[Withdrawal] Cancel Withdrawal Error';

export const RETRY = '[Withdrawal] Retry Withdrawal';
export const RETRY_COMPLETE = '[Withdrawal] Retry Withdrawal Complete';
export const RETRY_ERROR = '[Withdrawal] Retry Withdrawal Error';

export const SET_AS_COMPLETED = '[Withdrawal] Set as Completed Withdrawal';
export const SET_AS_COMPLETED_COMPLETE = '[Withdrawal] Set As Completed Withdrawal Complete';
export const SET_AS_COMPLETED_ERROR = '[Withdrawal] Set As Completed Withdrawal Error';

export const CHANGE_PROCESS_TYPE = '[Withdrawal] Change Process Type';
export const CHANGE_PROCESS_TYPE_COMPLETED = '[Withdrawal] Change Process Type Completed';
export const CHANGE_PROCESS_TYPE_ERROR = '[Withdrawal] Change Process Type Error';

export const CHANGE_ALL_PROCESS_TYPE = '[Withdrawal] Change All Process Type';
export const CHANGE_ALL_PROCESS_TYPE_COMPLETED = '[Withdrawal] Change All Process Type Completed';
export const CHANGE_ALL_PROCESS_TYPE_ERROR = '[Withdrawal] Change All Process Type Error';

export const GET_HISTORY = '[Withdrawal] Get History';
export const GET_HISTORY_COMPLETE = '[Withdrawal] Get History Complete';
export const GET_HISTORY_ERROR = '[Withdrawal] Get History Error';

export const CLEAR = '[Withdrawal] Clear';
export const CLEAR_ERRORS = '[Withdrawal] Clear Errors';

export const CALLBACKTOMERCHANT = '[Withdrawal] Callback Withdrawal To Merchant';
export const CALLBACKTOMERCHANT_COMPLETE = '[Withdrawal] Callback Withdrawal To Merchant Complete';
export const CALLBACKTOMERCHANT_ERROR = '[Withdrawal] Callback Withdrawal  To Merchant Error';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: WithdrawalSearchArgs) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: ListSearchResponse<WithdrawalSearch[]>) {

    }
}

export class SearchError implements Action {
    readonly type = SEARCH_ERROR;

    constructor(public payload: string) {

    }
}

export class GetDetails implements Action {
    readonly type = GET_DETAILS;

    constructor(public payload: number) {

    }
}

export class GetDetailsComplete implements Action {
    readonly type = GET_DETAILS_COMPLETE;

    constructor(public payload: Withdrawal) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: WithdrawalCreate) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: Withdrawal) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    } 
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: Withdrawal) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: Withdrawal) {

    }
}

export class EditError implements Action {
    readonly type = EDIT_ERROR;

    constructor(public payload: string) {

    }
}

export class GetReceipt implements Action {
    readonly type = GET_RECEIPT;

    constructor(public id: number) {

    }
}

export class GetReceiptComplete implements Action {
    readonly type = GET_RECEIPT_COMPLETE;

    constructor(public payload: WithdrawalReceipt) {

    }
}

export class GetReceiptError implements Action {
    readonly type = GET_RECEIPT_ERROR;

    constructor(public payload: string) {

    }
}

export class Cancel implements Action {
    readonly type = CANCEL;

    constructor(public id: number) {

    }
}

export class CancelComplete implements Action {
    readonly type = CANCEL_COMPLETE;

    constructor(public payload: Withdrawal) {

    }
}

export class CancelError implements Action {
    readonly type = CANCEL_ERROR;

    constructor(public payload: string) {

    }
}

export class Retry implements Action {
    readonly type = RETRY;

    constructor(public id: number) {

    }
}

export class RetryComplete implements Action {
    readonly type = RETRY_COMPLETE;

    constructor(public payload: Withdrawal) {

    }
}

export class RetryError implements Action {
    readonly type = RETRY_ERROR;

    constructor(public payload: string) {

    }
}

export class SetAsCompleted implements Action {
    readonly type = SET_AS_COMPLETED;

    constructor(public id: number) {

    }
}

export class SetAsCompletedComplete implements Action {
    readonly type = SET_AS_COMPLETED_COMPLETE;

    constructor(public payload: Withdrawal) {

    }
}

export class SetAsCompletedError implements Action {
    readonly type = SET_AS_COMPLETED_ERROR;

    constructor(public payload: string) {

    }
}

export class ChangeProcessType implements Action {
    readonly type = CHANGE_PROCESS_TYPE;

    constructor(public id: number, public processType: number) {

    }
}

export class ChangeProcessTypeComplete implements Action {
    readonly type = CHANGE_PROCESS_TYPE_COMPLETED;

    constructor(public payload: Withdrawal) {

    }
}

export class ChangeProcessTypeError implements Action {
    readonly type = CHANGE_PROCESS_TYPE_ERROR;

    constructor(public payload: string) {

    }
}

export class ChangeAllProcessType implements Action {
    readonly type = CHANGE_ALL_PROCESS_TYPE;

    constructor(public args: WithdrawalSearchArgs, public processType: number) {

    }
}

export class ChangeAllProcessTypeComplete implements Action {
    readonly type = CHANGE_ALL_PROCESS_TYPE_COMPLETED;

    constructor() {

    }
}

export class ChangeAllProcessTypeError implements Action {
    readonly type = CHANGE_ALL_PROCESS_TYPE_ERROR;

    constructor(public payload: string) {

    }
}

export class GetHistory implements Action {
    readonly type = GET_HISTORY;

    constructor(public payload: number) {

    }
}

export class GetHistoryComplete implements Action {
    readonly type = GET_HISTORY_COMPLETE;

    constructor(public payload: WithdrawalTransferHistory[]) {

    }
}

export class GetHistoryError implements Action {
    readonly type = GET_HISTORY_ERROR;

    constructor(public payload: string) {

    }
}

export class CallbackToMerchant implements Action {
    readonly type = CALLBACKTOMERCHANT;

    constructor(public id: number) {

    }
}

export class CallbackToMerchantComplete implements Action {
    readonly type = CALLBACKTOMERCHANT_COMPLETE;

    constructor(public payload: string) {
        console.log(payload);
    }
}

export class CallbackToMerchantError implements Action {
    readonly type = CALLBACKTOMERCHANT_ERROR;

    constructor(public payload: string) {

    }
}

export class Clear implements Action {
    readonly type = CLEAR;

    constructor() {

    }
}

export class ClearErrors implements Action {
    readonly type = CLEAR_ERRORS;

    constructor() {

    }
}

export type Actions =
    Search | SearchComplete | SearchError
    | GetDetails | GetDetailsComplete | GetDetailsError
    | Create | CreateComplete | CreateError
    | Edit | EditComplete | EditError
    | GetReceipt | GetReceiptComplete | GetReceiptError
    | Cancel | CancelComplete | CancelError
    | Retry | RetryComplete | RetryError
    | SetAsCompleted | SetAsCompletedComplete | SetAsCompletedError
    | ChangeProcessType | ChangeProcessTypeComplete | ChangeProcessTypeError
    | ChangeAllProcessType | ChangeAllProcessTypeComplete | ChangeAllProcessTypeError
    | GetHistory | GetHistoryComplete | GetHistoryError
    | CallbackToMerchant | CallbackToMerchantComplete | CallbackToMerchantError
    | Clear | ClearErrors;
