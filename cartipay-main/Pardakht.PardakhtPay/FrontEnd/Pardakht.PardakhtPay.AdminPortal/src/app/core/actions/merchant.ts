import { Action } from '@ngrx/store';
import { Merchant, MerchantCreate, MerchantEdit } from '../../models/merchant-model';


export const SEARCH = '[Merchant] Search';
export const SEARCH_COMPLETE = '[Merchant] Search Complete';
export const SEARCH_ERROR = '[Merchant] Search Error';

export const GET_ALL = '[Merchant] Get All';
export const GET_ALL_COMPLETE = '[Merchant] Get All Complete';
export const GET_ALL_ERROR = '[Merchant] Get All Error';

export const GET_DETAILS = '[Merchant] Get Details';
export const GET_DETAILS_COMPLETE = '[Merchant] Get Details Complete';
export const GET_DETAILS_ERROR = '[Merchant] Get Details Error';

export const CREATE = '[Merchant] Create';
export const CREATE_COMPLETE = '[Merchant] Create Complete';
export const CREATE_ERROR = '[Merchant] Create Error';

export const EDIT = '[Merchant] Edit';
export const EDIT_COMPLETE = '[Merchant] Edit Complete';
export const EDIT_ERROR = '[Merchant] Edit Error';

export const DELETE = '[Merchant] Delete';
export const DELETE_COMPLETE = '[Merchant] Delete Complete';
export const DELETE_ERROR = '[Merchant] Delete Error';

export const CLEAR = '[Merchant] Clear';
export const CLEAR_ERRORS = '[Merchant] Clear Errors';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: string) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: Merchant[]) {

    }
}

export class SearchError implements Action {
    readonly type = SEARCH_ERROR;

    constructor(public payload: string) {

    }
}

export class GetAll implements Action {
    readonly type = GET_ALL;

    constructor() {

    }
}

export class GetAllComplete implements Action {
    readonly type = GET_ALL_COMPLETE;

    constructor(public payload: Merchant[]) {

    }
}

export class GetAllError implements Action {
    readonly type = GET_ALL_ERROR;

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

    constructor(public payload: MerchantEdit) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: MerchantCreate) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: MerchantCreate) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    } 
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: MerchantEdit) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: MerchantEdit) {

    }
}

export class EditError implements Action {
    readonly type = EDIT_ERROR;

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
    | GetAll | GetAllComplete | GetAllError
    | GetDetails | GetDetailsComplete | GetDetailsError
    | Create | CreateComplete | CreateError
    | Edit | EditComplete | EditError
    | Delete | DeleteComplete | DeleteError
    | Clear | ClearErrors;
