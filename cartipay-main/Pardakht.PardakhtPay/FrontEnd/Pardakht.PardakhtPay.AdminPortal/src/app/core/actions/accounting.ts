import { Action } from '@ngrx/store';
import { ListSearchResponse } from '../../models/types';
import { DailyAccountingDTO, AccountingSearchArgs } from '../../models/accounting';

export const SEARCH = '[Accounting] Search';
export const SEARCH_COMPLETE = '[Accounting] Search Complete';
export const SEARCH_ERROR = '[Accounting] Search Error';

export const CLEAR_ALL = '[Accounting] Clear All';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: AccountingSearchArgs) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: DailyAccountingDTO[]) {
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
    Search | SearchComplete | SearchError | ClearAll;