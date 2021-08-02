import { Action } from '@ngrx/store';
import { TransferAccount } from '../../models/transfer-account';


export const SEARCH = '[TransferAccount] Search';
export const SEARCH_COMPLETE = '[TransferAccount] Search Complete';
export const SEARCH_ERROR = '[TransferAccount] Search Error';

export const GET_ALL = '[TransferAccount] Get All';
export const GET_ALL_COMPLETE = '[TransferAccount] Get All Complete';
export const GET_ALL_ERROR = '[TransferAccount] Get All Error';

export const GET_DETAILS = '[TransferAccount] Get Details';
export const GET_DETAILS_COMPLETE = '[TransferAccount] Get Details Complete';
export const GET_DETAILS_ERROR = '[TransferAccount] Get Details Error';

export const CREATE = '[TransferAccount] Create';
export const CREATE_COMPLETE = '[TransferAccount] Create Complete';
export const CREATE_ERROR = '[TransferAccount] Create Error';

export const EDIT = '[TransferAccount] Edit';
export const EDIT_COMPLETE = '[TransferAccount] Edit Complete';
export const EDIT_ERROR = '[TransferAccount] Edit Error';

export const DELETE = '[TransferAccount] Delete';
export const DELETE_COMPLETE = '[TransferAccount] Delete Complete';
export const DELETE_ERROR = '[TransferAccount] Delete Error';

export const CLEAR = '[TransferAccount] Clear';
export const CLEAR_ERRORS = '[TransferAccount] Clear Errors';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: string) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: TransferAccount[]) {

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

    constructor(public payload: TransferAccount[]) {

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

    constructor(public payload: TransferAccount) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: TransferAccount) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: TransferAccount) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: TransferAccount) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: TransferAccount) {

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
