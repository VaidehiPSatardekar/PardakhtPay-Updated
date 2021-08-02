import { Action } from '@ngrx/store';
import { ManualTransfer, ManualTransferSearchArgs, ManualTransferDetail } from '../../models/manual-transfer';
import { ListSearchResponse } from '../../models/types';
import { WithdrawalReceipt } from '../../models/withdrawal';


export const SEARCH = '[ManualTransfer] Search';
export const SEARCH_COMPLETE = '[ManualTransfer] Search Complete';
export const SEARCH_ERROR = '[ManualTransfer] Search Error';

export const GET_DETAILS = '[ManualTransfer] Get Details';
export const GET_DETAILS_COMPLETE = '[ManualTransfer] Get Details Complete';
export const GET_DETAILS_ERROR = '[ManualTransfer] Get Details Error';

export const CREATE = '[ManualTransfer] Create';
export const CREATE_COMPLETE = '[ManualTransfer] Create Complete';
export const CREATE_ERROR = '[ManualTransfer] Create Error';

export const EDIT = '[ManualTransfer] Edit';
export const EDIT_COMPLETE = '[ManualTransfer] Edit Complete';
export const EDIT_ERROR = '[ManualTransfer] Edit Error';

export const CANCEL = '[ManualTransfer] Cancel';
export const CANCEL_COMPLETE = '[ManualTransfer] Cancel Complete';
export const CANCEL_ERROR = '[ManualTransfer] Cancel Error';

export const CANCEL_DETAIL = '[ManualTransfer] Cancel Detail';
export const CANCEL_DETAIL_COMPLETE = '[ManualTransfer] Cancel Detail Complete';
export const CANCEL_DETAIL_ERROR = '[ManualTransfer] Cancel Detail Error';

export const RETRY_DETAIL = '[ManualTransfer] Retry Detail';
export const RETRY_DETAIL_COMPLETE = '[ManualTransfer] Retry Detail Complete';
export const RETRY_DETAIL_ERROR = '[ManualTransfer] Retry Detail Error';

export const SETASCOMPLETED_DETAIL = '[ManualTransfer] Set As Completed Detail';
export const SETASCOMPLETED_DETAIL_COMPLETE = '[ManualTransfer] Set As Completed Detail Complete';
export const SETASCOMPLETED_DETAIL_ERROR = '[ManualTransfer] Set As Completed Detail Error';

export const DELETE = '[ManualTransfer] Delete';
export const DELETE_COMPLETE = '[ManualTransfer] Delete Complete';
export const DELETE_ERROR = '[ManualTransfer] Delete Error';

export const GET_RECEIPT = '[ManualTransfer] Get Receipt';
export const GET_RECEIPT_COMPLETE = '[ManualTransfer] Get Receipt Complete';
export const GET_RECEIPT_ERROR = '[ManualTransfer] Get Receipt Error';

export const CLEAR = '[ManualTransfer] Clear';
export const CLEAR_ERRORS = '[ManualTransfer] Clear Errors';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: ManualTransferSearchArgs) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: ListSearchResponse<ManualTransfer[]>) {

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

    constructor(public payload: ManualTransfer) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: ManualTransfer) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: ManualTransfer) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: ManualTransfer) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: ManualTransfer) {

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

    constructor(public payload: ManualTransfer) {

    }
}

export class CancelError implements Action {
    readonly type = CANCEL_ERROR;

    constructor(public payload: string) {

    }
}

export class CancelDetail implements Action {
    readonly type = CANCEL_DETAIL;

    constructor(public id: number) {

    }
}

export class CancelDetailComplete implements Action {
    readonly type = CANCEL_DETAIL_COMPLETE;

    constructor(public payload: ManualTransferDetail) {

    }
}

export class CancelDetailError implements Action {
    readonly type = CANCEL_DETAIL_ERROR;

    constructor(public payload: string) {

    }
}

export class RetryDetail implements Action {
    readonly type = RETRY_DETAIL;

    constructor(public id: number) {

    }
}

export class RetryDetailComplete implements Action {
    readonly type = RETRY_DETAIL_COMPLETE;

    constructor(public payload: ManualTransferDetail) {

    }
}

export class RetryDetailError implements Action {
    readonly type = RETRY_DETAIL_ERROR;

    constructor(public payload: string) {

    }
}

export class SetAsCompletedDetail implements Action {
    readonly type = SETASCOMPLETED_DETAIL;

    constructor(public id: number) {

    }
}

export class SetAsCompletedDetailComplete implements Action {
    readonly type = SETASCOMPLETED_DETAIL_COMPLETE;

    constructor(public payload: ManualTransferDetail) {

    }
}

export class SetAsCompletedDetailError implements Action {
    readonly type = SETASCOMPLETED_DETAIL_ERROR;

    constructor(public payload: string) {

    }
}

export class Delete implements Action {
    readonly type = DELETE;

    constructor(public id: number) {

    }
}

export class DeleteComplete implements Action {
    readonly type = DELETE_COMPLETE;

    constructor() {

    }
}

export class DeleteError implements Action {
    readonly type = DELETE_ERROR;

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
    | Cancel | CancelComplete | CancelError
    | CancelDetail | CancelDetailComplete | CancelDetailError
    | RetryDetail | RetryDetailComplete | RetryDetailError
    | SetAsCompletedDetail | SetAsCompletedDetailComplete | SetAsCompletedDetailError
    | Delete | DeleteComplete | DeleteError
    | GetReceipt | GetReceiptComplete | GetReceiptError
    | Clear | ClearErrors;
