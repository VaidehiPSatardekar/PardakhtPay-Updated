import { Action } from '@ngrx/store';
import { CardHolder } from '../../models/card-holder';

export const SEARCH = '[CardHolder] Search';
export const SEARCH_COMPLETE = '[CardHolder] Search Complete';
export const SEARCH_ERROR = '[CardHolder] Search Error';

export const CLEAR_ALL = '[CardHolder] Clear All';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: string) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: CardHolder) {
    }
}

export class SearchError implements Action {
    readonly type = SEARCH_ERROR;

    constructor(public payload: string) {

    }
}

export class ClearAll implements Action {
    readonly type = CLEAR_ALL;

    constructor() {

    }
}

export type Actions =
    Search | SearchComplete | SearchError
    | ClearAll;