import { Action } from '@ngrx/store';
import { ListSearchResponse } from '../../models/types';
import { TransactionSearch, TransactionSearchArgs } from '../../models/transaction';

export const SEARCH = '[Transaction] Search';
export const SEARCH_COMPLETE = '[Transaction] Search Complete';
export const SEARCH_ERROR = '[Transaction] Search Error';

export const SET_AS_COMPLETED = '[Transaction] Set as Completed Transaction';
export const SET_AS_COMPLETED_COMPLETE = '[Transaction] Set As Completed Transaction Complete';
export const SET_AS_COMPLETED_ERROR = '[Transaction] Set As Completed Transaction Error';

export const SET_AS_EXPIRED = '[Transaction] Set as Expired Transaction';
export const SET_AS_EXPIRED_COMPLETE = '[Transaction] Set As Expired Transaction Complete';
export const SET_AS_EXPIRED_ERROR = '[Transaction] Set As Expired Transaction Error';

export const TRANSACTIONCALLBACKTOMERCHANT = '[Transaction] Transaction Callback To Merchant';
export const TRANSACTIONCALLBACKTOMERCHANT_COMPLETE = '[Transaction] Transaction Callback To Merchant Complete';
export const TRANSACTIONCALLBACKTOMERCHANT_ERROR = '[Transaction] Transaction Callback To Merchant Error';


export const CLEAR_ALL = '[Transaction] Clear All';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: TransactionSearchArgs) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: ListSearchResponse<TransactionSearch[]>) {
    }
}

export class SearchError implements Action {
    readonly type = SEARCH_ERROR;

    constructor(public payload: string) {

    }
}

export class SetAsCompleted implements Action {
    readonly type = SET_AS_COMPLETED;

    constructor(public id: number) {

    }
}

export class SetAsCompletedComplete implements Action {
    readonly type = SET_AS_COMPLETED_COMPLETE;

    constructor() {

    }
}

export class SetAsCompletedError implements Action {
    readonly type = SET_AS_COMPLETED_ERROR;

    constructor(public payload: string) {

    }
}

export class SetAsExpired implements Action {
    readonly type = SET_AS_EXPIRED;

    constructor(public id: number) {

    }
}

export class SetAsExpiredComplete implements Action {
    readonly type = SET_AS_EXPIRED_COMPLETE;

    constructor() {

    }
}

export class SetAsExpiredError implements Action {
    readonly type = SET_AS_EXPIRED_ERROR;

    constructor(public payload: string) {

    }
}

export class TransactionCallbackToMerchant implements Action {
    readonly type = TRANSACTIONCALLBACKTOMERCHANT;

    constructor(public id: number) {

    }
}

export class TransactionCallbackToMerchantComplete implements Action {
    readonly type = TRANSACTIONCALLBACKTOMERCHANT_COMPLETE;

    constructor(public payload: string) {

    }
}

export class TransactionCallbackToMerchantError implements Action {
    readonly type = TRANSACTIONCALLBACKTOMERCHANT_ERROR;

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
    | SetAsCompleted | SetAsCompletedComplete | SetAsCompletedError
    | SetAsExpired | SetAsExpiredComplete | SetAsExpiredError
    | TransactionCallbackToMerchant | TransactionCallbackToMerchantComplete | TransactionCallbackToMerchantError
    | ClearAll;