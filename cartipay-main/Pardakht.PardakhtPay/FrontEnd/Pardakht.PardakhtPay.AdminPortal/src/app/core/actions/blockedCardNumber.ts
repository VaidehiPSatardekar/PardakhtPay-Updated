import { Action } from '@ngrx/store';
import { BlockedCardNumber } from '../../models/blocked-card-number';

export const GET_ALL = '[BlockedCardNumber] Get All';
export const GET_ALL_COMPLETE = '[BlockedCardNumber] Get All Complete';
export const GET_ALL_ERROR = '[BlockedCardNumber] Get All Error';

export const GET_DETAILS = '[BlockedCardNumber] Get Details';
export const GET_DETAILS_COMPLETE = '[BlockedCardNumber] Get Details Complete';
export const GET_DETAILS_ERROR = '[BlockedCardNumber] Get Details Error';

export const CREATE = '[BlockedCardNumber] Create';
export const CREATE_COMPLETE = '[BlockedCardNumber] Create Complete';
export const CREATE_ERROR = '[BlockedCardNumber] Create Error';

export const EDIT = '[BlockedCardNumber] Edit';
export const EDIT_COMPLETE = '[BlockedCardNumber] Edit Complete';
export const EDIT_ERROR = '[BlockedCardNumber] Edit Error';

export const DELETE = '[BlockedCardNumber] Delete';
export const DELETE_COMPLETE = '[BlockedCardNumber] Delete Complete';
export const DELETE_ERROR = '[BlockedCardNumber] Delete Error';

export const CLEAR = '[BlockedCardNumber] Clear';
export const CLEAR_ERRORS = '[BlockedCardNumber] Clear Errors';



export class GetAll implements Action {
    readonly type = GET_ALL;

    constructor() {

    }
}

export class GetAllComplete implements Action {
    readonly type = GET_ALL_COMPLETE;

    constructor(public payload: BlockedCardNumber[]) {

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

    constructor(public payload: BlockedCardNumber) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: BlockedCardNumber) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: BlockedCardNumber) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: BlockedCardNumber) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: BlockedCardNumber) {

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
