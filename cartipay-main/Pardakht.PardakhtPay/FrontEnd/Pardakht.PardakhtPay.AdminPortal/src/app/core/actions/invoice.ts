import { Action } from '@ngrx/store';
import { Invoice, InvoiceSearchArgs } from '../../models/invoice';
import { ListSearchResponse } from '../../models/types';


export const SEARCH = '[Invoice] Search';
export const SEARCH_COMPLETE = '[Invoice] Search Complete';
export const SEARCH_ERROR = '[Invoice] Search Error';

export const GET_DETAILS = '[Invoice] Get Details';
export const GET_DETAILS_COMPLETE = '[Invoice] Get Details Complete';
export const GET_DETAILS_ERROR = '[Invoice] Get Details Error';

export const CLEAR = '[Invoice] Clear';
export const CLEAR_ERRORS = '[Invoice] Clear Errors';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: InvoiceSearchArgs) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: ListSearchResponse<Invoice[]>) {

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

    constructor(public payload: Invoice) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

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
    | Clear | ClearErrors;
