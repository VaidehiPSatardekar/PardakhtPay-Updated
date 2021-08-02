import { Action } from '@ngrx/store';
import { CardToCardAccountGroup } from '../../models/card-to-card-account-group';

export const GET_ALL = '[CardToCardAccountGroup] Get All';
export const GET_ALL_COMPLETE = '[CardToCardAccountGroup] Get All Complete';
export const GET_ALL_ERROR = '[CardToCardAccountGroup] Get All Error';

export const GET_DETAILS = '[CardToCardAccountGroup] Get Details';
export const GET_DETAILS_COMPLETE = '[CardToCardAccountGroup] Get Details Complete';
export const GET_DETAILS_ERROR = '[CardToCardAccountGroup] Get Details Error';

export const CREATE = '[CardToCardAccountGroup] Create';
export const CREATE_COMPLETE = '[CardToCardAccountGroup] Create Complete';
export const CREATE_ERROR = '[CardToCardAccountGroup] Create Error';

export const EDIT = '[CardToCardAccountGroup] Edit';
export const EDIT_COMPLETE = '[CardToCardAccountGroup] Edit Complete';
export const EDIT_ERROR = '[CardToCardAccountGroup] Edit Error';

export const DELETE = '[CardToCardAccountGroup] Delete';
export const DELETE_COMPLETE = '[CardToCardAccountGroup] Delete Complete';
export const DELETE_ERROR = '[CardToCardAccountGroup] Delete Error';

export const CLEAR = '[CardToCardAccountGroup] Clear';
export const CLEAR_ERRORS = '[CardToCardAccountGroup] Clear Errors';



export class GetAll implements Action {
    readonly type = GET_ALL;

    constructor() {

    }
}

export class GetAllComplete implements Action {
    readonly type = GET_ALL_COMPLETE;

    constructor(public payload: CardToCardAccountGroup[]) {

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

    constructor(public payload: CardToCardAccountGroup) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: CardToCardAccountGroup) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: CardToCardAccountGroup) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: CardToCardAccountGroup) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: CardToCardAccountGroup) {

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
