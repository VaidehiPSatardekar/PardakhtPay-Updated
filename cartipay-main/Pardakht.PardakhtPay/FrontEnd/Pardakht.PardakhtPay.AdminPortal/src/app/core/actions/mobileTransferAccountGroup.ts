import { Action } from '@ngrx/store';
import { MobileTransferCardAccountGroup } from '../../models/mobile-transfer';

export const GET_ALL = '[MobileTransferCardAccountGroup] Get All';
export const GET_ALL_COMPLETE = '[MobileTransferCardAccountGroup] Get All Complete';
export const GET_ALL_ERROR = '[MobileTransferCardAccountGroup] Get All Error';

export const GET_DETAILS = '[MobileTransferCardAccountGroup] Get Details';
export const GET_DETAILS_COMPLETE = '[MobileTransferCardAccountGroup] Get Details Complete';
export const GET_DETAILS_ERROR = '[MobileTransferCardAccountGroup] Get Details Error';

export const CREATE = '[MobileTransferCardAccountGroup] Create';
export const CREATE_COMPLETE = '[MobileTransferCardAccountGroup] Create Complete';
export const CREATE_ERROR = '[MobileTransferCardAccountGroup] Create Error';

export const EDIT = '[MobileTransferCardAccountGroup] Edit';
export const EDIT_COMPLETE = '[MobileTransferCardAccountGroup] Edit Complete';
export const EDIT_ERROR = '[MobileTransferCardAccountGroup] Edit Error';

export const DELETE = '[MobileTransferCardAccountGroup] Delete';
export const DELETE_COMPLETE = '[MobileTransferCardAccountGroup] Delete Complete';
export const DELETE_ERROR = '[MobileTransferCardAccountGroup] Delete Error';

export const CLEAR = '[MobileTransferCardAccountGroup] Clear';
export const CLEAR_ERRORS = '[MobileTransferCardAccountGroup] Clear Errors';



export class GetAll implements Action {
    readonly type = GET_ALL;

    constructor() {

    }
}

export class GetAllComplete implements Action {
    readonly type = GET_ALL_COMPLETE;

    constructor(public payload: MobileTransferCardAccountGroup[]) {

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

    constructor(public payload: MobileTransferCardAccountGroup) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: MobileTransferCardAccountGroup) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: MobileTransferCardAccountGroup) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: MobileTransferCardAccountGroup) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: MobileTransferCardAccountGroup) {

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
