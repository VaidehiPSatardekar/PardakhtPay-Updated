import { Action } from '@ngrx/store';
import { TenantUrlConfig } from '../../models/tenant-url-config';

export const GET_ALL = '[TenantUrlConfig] Get All';
export const GET_ALL_COMPLETE = '[TenantUrlConfig] Get All Complete';
export const GET_ALL_ERROR = '[TenantUrlConfig] Get All Error';

export const GET_DETAILS = '[TenantUrlConfig] Get Details';
export const GET_DETAILS_COMPLETE = '[TenantUrlConfig] Get Details Complete';
export const GET_DETAILS_ERROR = '[TenantUrlConfig] Get Details Error';

export const CREATE = '[TenantUrlConfig] Create';
export const CREATE_COMPLETE = '[TenantUrlConfig] Create Complete';
export const CREATE_ERROR = '[TenantUrlConfig] Create Error';

export const EDIT = '[TenantUrlConfig] Edit';
export const EDIT_COMPLETE = '[TenantUrlConfig] Edit Complete';
export const EDIT_ERROR = '[TenantUrlConfig] Edit Error';

export const CLEAR = '[TenantUrlConfig] Clear';
export const CLEAR_ERRORS = '[TenantUrlConfig] Clear Errors';

export class GetAll implements Action {
    readonly type = GET_ALL;

    constructor() {

    }
}

export class GetAllComplete implements Action {
    readonly type = GET_ALL_COMPLETE;

    constructor(public payload: TenantUrlConfig[]) {

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

    constructor(public payload: TenantUrlConfig) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: TenantUrlConfig) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: TenantUrlConfig) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: TenantUrlConfig) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: TenantUrlConfig) {

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
