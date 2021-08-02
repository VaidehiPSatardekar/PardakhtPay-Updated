import { Action } from '@ngrx/store';
import { InvoicePayment, InvoicePaymentSearchArgs } from '../../models/invoice';
import { ListSearchArgs, ListSearchResponse } from 'app/models/types';

export const SEARCH = '[InvoicePayment] Search';
export const SEARCH_COMPLETE = '[InvoicePayment] Search Complete';
export const SEARCH_ERROR = '[InvoicePayment] Search Error';

export const GET_DETAILS = '[InvoicePayment] Get Details';
export const GET_DETAILS_COMPLETE = '[InvoicePayment] Get Details Complete';
export const GET_DETAILS_ERROR = '[InvoicePayment] Get Details Error';

export const CREATE = '[InvoicePayment] Create';
export const CREATE_COMPLETE = '[InvoicePayment] Create Complete';
export const CREATE_ERROR = '[InvoicePayment] Create Error';

export const EDIT = '[InvoicePayment] Edit';
export const EDIT_COMPLETE = '[InvoicePayment] Edit Complete';
export const EDIT_ERROR = '[InvoicePayment] Edit Error';

export const CLEAR = '[InvoicePayment] Clear';
export const CLEAR_ERRORS = '[InvoicePayment] Clear Errors';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: InvoicePaymentSearchArgs) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: ListSearchResponse<InvoicePayment[]>) {
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

    constructor(public payload: InvoicePayment) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: InvoicePayment) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: InvoicePayment) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: InvoicePayment) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: InvoicePayment) {

    }
}

export class EditError implements Action {
    readonly type = EDIT_ERROR;

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
    | Search | SearchComplete | SearchError
    | GetDetails | GetDetailsComplete | GetDetailsError
    | Create | CreateComplete | CreateError
    | Edit | EditComplete | EditError
    | Clear | ClearErrors;
