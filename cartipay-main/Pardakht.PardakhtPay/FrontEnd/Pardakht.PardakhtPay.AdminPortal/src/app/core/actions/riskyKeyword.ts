import { Action } from '@ngrx/store';

export const GET_DETAILS = '[RiskyKeywords] Get Details';
export const GET_DETAILS_COMPLETE = '[RiskyKeywords] Get Details Complete';
export const GET_DETAILS_ERROR = '[RiskyKeywords] Get Details Error';

export const EDIT = '[RiskyKeywords] Edit';
export const EDIT_COMPLETE = '[RiskyKeywords] Edit Complete';
export const EDIT_ERROR = '[RiskyKeywords] Edit Error';

export const CLEAR = '[RiskyKeywords] Clear';
export const CLEAR_ERRORS = '[RiskyKeywords] Clear Errors';

export class GetDetails implements Action {
    readonly type = GET_DETAILS;

    constructor() {

    }
}

export class GetDetailsComplete implements Action {
    readonly type = GET_DETAILS_COMPLETE;

    constructor(public payload: string[]) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public payload: string[]) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: string[]) {

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
    GetDetails | GetDetailsComplete | GetDetailsError
    | Edit | EditComplete | EditError
    | Clear | ClearErrors;
