import { Action } from '@ngrx/store';
import { Tenant, TenantCreate } from '../../models/tenant';

export const SEARCH = '[Pardakhtpay Tenant] Search';
export const SEARCH_COMPLETE = '[Pardakhtpay Tenant] Search Complete';
export const SEARCH_ERROR = '[Pardakhtpay Tenant] Search Error';

export const GET_DETAILS = '[Pardakhtpay Tenant] Get Details';
export const GET_DETAILS_COMPLETE = '[Pardakhtpay Tenant] Get Details Complete';
export const GET_DETAILS_ERROR = '[Pardakhtpay Tenant] Get Details Error';

export const CREATE = '[Pardakhtpay Tenant] Create';
export const CREATE_COMPLETE = '[Pardakhtpay Tenant] Create Complete';
export const CREATE_ERROR = '[Pardakhtpay Tenant] Create Error';

export const CHANGE_SELECTED_TENANT = '[Pardakhtpay Tenant] Change Selected Tenant';

export const CLEAR = '[Pardakhtpay Tenant] Clear';
export const CLEAR_ERRORS = '[Pardakhtpay Tenant] Clear Errors';

export class Search implements Action {
    readonly type = SEARCH;

    constructor() {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: Tenant[]) {

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

    constructor(public payload: Tenant) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: TenantCreate) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: Tenant) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class ChangeSelectedTenant implements Action {
    readonly type = CHANGE_SELECTED_TENANT;

    constructor(public payload: Tenant) {

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
    | ChangeSelectedTenant
    | Clear | ClearErrors;
