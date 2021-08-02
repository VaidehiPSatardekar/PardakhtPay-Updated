import { Action } from '@ngrx/store';
import { UserSegmentGroup } from '../../models/user-segment-group';

export const GET_ALL = '[UserSegmentGroup] Get All';
export const GET_ALL_COMPLETE = '[UserSegmentGroup] Get All Complete';
export const GET_ALL_ERROR = '[UserSegmentGroup] Get All Error';

export const GET_DETAILS = '[UserSegmentGroup] Get Details';
export const GET_DETAILS_COMPLETE = '[UserSegmentGroup] Get Details Complete';
export const GET_DETAILS_ERROR = '[UserSegmentGroup] Get Details Error';

export const CREATE = '[UserSegmentGroup] Create';
export const CREATE_COMPLETE = '[UserSegmentGroup] Create Complete';
export const CREATE_ERROR = '[UserSegmentGroup] Create Error';

export const EDIT = '[UserSegmentGroup] Edit';
export const EDIT_COMPLETE = '[UserSegmentGroup] Edit Complete';
export const EDIT_ERROR = '[UserSegmentGroup] Edit Error';

export const DELETE = '[UserSegmentGroup] Delete';
export const DELETE_COMPLETE = '[UserSegmentGroup] Delete Complete';
export const DELETE_ERROR = '[UserSegmentGroup] Delete Error';

export const CLEAR = '[UserSegmentGroup] Clear';
export const CLEAR_ERRORS = '[UserSegmentGroup] Clear Errors';



export class GetAll implements Action {
    readonly type = GET_ALL;

    constructor() {

    }
}

export class GetAllComplete implements Action {
    readonly type = GET_ALL_COMPLETE;

    constructor(public payload: UserSegmentGroup[]) {

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

    constructor(public payload: UserSegmentGroup) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: UserSegmentGroup) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: UserSegmentGroup) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: UserSegmentGroup) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: UserSegmentGroup) {

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
