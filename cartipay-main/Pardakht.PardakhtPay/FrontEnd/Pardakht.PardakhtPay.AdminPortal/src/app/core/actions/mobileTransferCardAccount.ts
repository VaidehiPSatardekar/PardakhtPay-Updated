import { Action } from '@ngrx/store';
import { MobileTransferCardAccount } from '../../models/mobile-transfer';

export const GET_ALL = '[MobileTransferCardAccount] Get All';
export const GET_ALL_COMPLETE = '[MobileTransferCardAccount] Get All Complete';
export const GET_ALL_ERROR = '[MobileTransferCardAccount] Get All Error';

export const GET_DETAILS = '[MobileTransferCardAccount] Get Details';
export const GET_DETAILS_COMPLETE = '[MobileTransferCardAccount] Get Details Complete';
export const GET_DETAILS_ERROR = '[MobileTransferCardAccount] Get Details Error';

export const CREATE = '[MobileTransferCardAccount] Create';
export const CREATE_COMPLETE = '[MobileTransferCardAccount] Create Complete';
export const CREATE_ERROR = '[MobileTransferCardAccount] Create Error';

export const EDIT = '[MobileTransferCardAccount] Edit';
export const EDIT_COMPLETE = '[MobileTransferCardAccount] Edit Complete';
export const EDIT_ERROR = '[MobileTransferCardAccount] Edit Error';

export const CLEAR = '[MobileTransferCardAccount] Clear';
export const CLEAR_ERRORS = '[MobileTransferCardAccount] Clear Errors';

export class GetAll implements Action {
    readonly type = GET_ALL;

    constructor() {

    }
}

export class GetAllComplete implements Action {
    readonly type = GET_ALL_COMPLETE;

    constructor(public payload: MobileTransferCardAccount[]) {

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

    constructor(public payload: MobileTransferCardAccount) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: MobileTransferCardAccount) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: MobileTransferCardAccount) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: MobileTransferCardAccount) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: MobileTransferCardAccount) {

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
    GetAll | GetAllComplete | GetAllError
    | GetDetails | GetDetailsComplete | GetDetailsError
    | Create | CreateComplete | CreateError
    | Edit | EditComplete | EditError
    | Clear | ClearErrors;
