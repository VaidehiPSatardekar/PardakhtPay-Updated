import { Action } from '@ngrx/store';
import { BlockedPhoneNumber } from '../../models/blocked-phone-number';

export const GET_ALL = '[BlockedPhoneNumber] Get All';
export const GET_ALL_COMPLETE = '[BlockedPhoneNumber] Get All Complete';
export const GET_ALL_ERROR = '[BlockedPhoneNumber] Get All Error';

export const GET_DETAILS = '[BlockedPhoneNumber] Get Details';
export const GET_DETAILS_COMPLETE = '[BlockedPhoneNumber] Get Details Complete';
export const GET_DETAILS_ERROR = '[BlockedPhoneNumber] Get Details Error';

export const CREATE = '[BlockedPhoneNumber] Create';
export const CREATE_COMPLETE = '[BlockedPhoneNumber] Create Complete';
export const CREATE_ERROR = '[BlockedPhoneNumber] Create Error';

export const EDIT = '[BlockedPhoneNumber] Edit';
export const EDIT_COMPLETE = '[BlockedPhoneNumber] Edit Complete';
export const EDIT_ERROR = '[BlockedPhoneNumber] Edit Error';

export const DELETE = '[BlockedPhoneNumber] Delete';
export const DELETE_COMPLETE = '[BlockedPhoneNumber] Delete Complete';
export const DELETE_ERROR = '[BlockedPhoneNumber] Delete Error';

export const CLEAR = '[BlockedPhoneNumber] Clear';
export const CLEAR_ERRORS = '[BlockedPhoneNumber] Clear Errors';



export class GetAll implements Action {
    readonly type = GET_ALL;

    constructor() {

    }
}

export class GetAllComplete implements Action {
    readonly type = GET_ALL_COMPLETE;

    constructor(public payload: BlockedPhoneNumber[]) {

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

    constructor(public payload: BlockedPhoneNumber) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: BlockedPhoneNumber) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: BlockedPhoneNumber) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: BlockedPhoneNumber) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: BlockedPhoneNumber) {

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
    | GetAll | GetAllComplete | GetAllError
    | GetDetails | GetDetailsComplete | GetDetailsError
    | Create | CreateComplete | CreateError
    | Edit | EditComplete | EditError
    | Delete | DeleteComplete | DeleteError
    | Clear | ClearErrors;
