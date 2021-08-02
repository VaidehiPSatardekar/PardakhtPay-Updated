import { Action } from '@ngrx/store';
import { ListSearchResponse } from '../../models/types';
import { AutoTransfer, AutoTransferSearchArgs } from '../../models/autoTransfer';

export const SEARCH = '[AutoTransfer] Search';
export const SEARCH_COMPLETE = '[AutoTransfer] Search Complete';
export const SEARCH_ERROR = '[AutoTransfer] Search Error';

export const CLEAR_ALL = '[AutoTransfer] Clear All';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: AutoTransferSearchArgs) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: ListSearchResponse<AutoTransfer[]>) {
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