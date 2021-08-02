import { Action } from '@ngrx/store';
import { InvoiceOwnerSetting } from '../../models/invoice';

export const GET_ALL = '[InvoiceOwnerSetting] Get All';
export const GET_ALL_COMPLETE = '[InvoiceOwnerSetting] Get All Complete';
export const GET_ALL_ERROR = '[InvoiceOwnerSetting] Get All Error';

export const GET_DETAILS = '[InvoiceOwnerSetting] Get Details';
export const GET_DETAILS_COMPLETE = '[InvoiceOwnerSetting] Get Details Complete';
export const GET_DETAILS_ERROR = '[InvoiceOwnerSetting] Get Details Error';

export const CREATE = '[InvoiceOwnerSetting] Create';
export const CREATE_COMPLETE = '[InvoiceOwnerSetting] Create Complete';
export const CREATE_ERROR = '[InvoiceOwnerSetting] Create Error';

export const EDIT = '[InvoiceOwnerSetting] Edit';
export const EDIT_COMPLETE = '[InvoiceOwnerSetting] Edit Complete';
export const EDIT_ERROR = '[InvoiceOwnerSetting] Edit Error';

export const CLEAR = '[InvoiceOwnerSetting] Clear';
export const CLEAR_ERRORS = '[InvoiceOwnerSetting] Clear Errors';

export class GetAll implements Action {
    readonly type = GET_ALL;

    constructor() {

    }
}

export class GetAllComplete implements Action {
    readonly type = GET_ALL_COMPLETE;

    constructor(public payload: InvoiceOwnerSetting[]) {

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

    constructor(public payload: InvoiceOwnerSetting) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: InvoiceOwnerSetting) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: InvoiceOwnerSetting) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: InvoiceOwnerSetting) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: InvoiceOwnerSetting) {

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
    | GetAll | GetAllComplete | GetAllError
    | GetDetails | GetDetailsComplete | GetDetailsError
    | Create | CreateComplete | CreateError
    | Edit | EditComplete | EditError
    | Clear | ClearErrors;
