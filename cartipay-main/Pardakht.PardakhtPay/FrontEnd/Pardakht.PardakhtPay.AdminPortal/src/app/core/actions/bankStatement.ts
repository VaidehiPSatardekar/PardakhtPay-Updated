import { Action } from '@ngrx/store';
import { ListSearchResponse } from '../../models/types';
import { BankStatementItem, BankStatementItemSearchArgs } from '../../models/bank-statement-item';

export const SEARCH = '[BankStatementItem] Search';
export const SEARCH_COMPLETE = '[BankStatementItem] Search Complete';
export const SEARCH_ERROR = '[BankStatementItem] Search Error';

export const CLEAR_ALL = '[BankStatementItem] Clear All';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: BankStatementItemSearchArgs) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: ListSearchResponse<BankStatementItem[]>) {
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