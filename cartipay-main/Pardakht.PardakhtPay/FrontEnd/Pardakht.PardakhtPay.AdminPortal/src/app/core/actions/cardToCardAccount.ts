import { Action } from '@ngrx/store';
import { CardToCardAccount } from '../../models/card-to-card-account';


export const SEARCH = '[CardToCardAccount] Search';
export const SEARCH_COMPLETE = '[CardToCardAccount] Search Complete';
export const SEARCH_ERROR = '[CardToCardAccount] Search Error';

export const GET_ALL = '[CardToCardAccount] Get All';
export const GET_ALL_COMPLETE = '[CardToCardAccount] Get All Complete';
export const GET_ALL_ERROR = '[CardToCardAccount] Get All Error';

export const GET_DETAILS = '[CardToCardAccount] Get Details';
export const GET_DETAILS_COMPLETE = '[CardToCardAccount] Get Details Complete';
export const GET_DETAILS_ERROR = '[CardToCardAccount] Get Details Error';

export const CREATE = '[CardToCardAccount] Create';
export const CREATE_COMPLETE = '[CardToCardAccount] Create Complete';
export const CREATE_ERROR = '[CardToCardAccount] Create Error';

export const EDIT = '[CardToCardAccount] Edit';
export const EDIT_COMPLETE = '[CardToCardAccount] Edit Complete';
export const EDIT_ERROR = '[CardToCardAccount] Edit Error';

export const CLEAR = '[CardToCardAccount] Clear';
export const CLEAR_ERRORS = '[CardToCardAccount] Clear Errors';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: string) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: CardToCardAccount[]) {

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

    constructor(public payload: CardToCardAccount[]) {

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

    constructor(public payload: CardToCardAccount) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: CardToCardAccount) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: CardToCardAccount) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: CardToCardAccount) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: CardToCardAccount) {

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
    Search | SearchComplete | SearchError
    | GetAll | GetAllComplete | GetAllError
    | GetDetails | GetDetailsComplete | GetDetailsError
    | Create | CreateComplete | CreateError
    | Edit | EditComplete | EditError
    | Clear | ClearErrors;
