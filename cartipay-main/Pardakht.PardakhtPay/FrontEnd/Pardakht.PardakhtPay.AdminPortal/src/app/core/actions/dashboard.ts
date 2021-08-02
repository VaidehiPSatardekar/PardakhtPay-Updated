import { Action } from '@ngrx/store';
import { DashBoardWidget, DashBoardChartWidget, DashboardQuery, DashboardAccountStatusDTO, DashboardMerchantTransactionReportDTO } from '../../models/dashboard';

export const CLEAR_ERRORS = '[Dashboard] Clear Errors';

export const GET_TRANSACTION_WIDGET = '[Dashboard] Get transaction widget';
export const GET_TRANSACTION_WIDGET_COMPLETE = '[Dashboard] Get transaction widget Complete';
export const GET_TRANSACTION_WIDGET_ERROR = '[Dashboard] Get transaction widget Error';

export const GET_MERCHANT_TRANSACTION_WIDGET = '[Dashboard] Get merchant transaction widget';
export const GET_MERCHANT_TRANSACTION_WIDGET_COMPLETE = '[Dashboard] Get merchant transaction widget complete';
export const GET_MERCHANT_TRANSACTION_WIDGET_ERROR = '[Dashboard] Get merchant transaction widget error';

export const GET_TRANSACTION_GRAPH_WIDGET = '[Dashboard] Get transaction graph widget';
export const GET_TRANSACTION_GRAPH_WIDGET_COMPLETE = '[Dashboard] Get transaction graph widget complete';
export const GET_TRANSACTION_GRAPH_WIDGET_ERROR = '[Dashboard] Get transaction graph widget error';

export const GET_ACCOUNTING_GRAPH_WIDGET = '[Dashboard] Get accounting graph widget';
export const GET_ACCOUNTING_GRAPH_WIDGET_COMPLETE = '[Dashboard] Get accounting graph widget complete';
export const GET_ACCOUNTING_GRAPH_WIDGET_ERROR = '[Dashboard] Get accounting graph widget error';

export const GET_ACCOUNT_STATUS_WIDGET = '[Dashboard] Get account status widget';
export const GET_ACCOUNT_STATUS_WIDGET_COMPLETE = '[Dashboard] Get account status widget complete';
export const GET_ACCOUNT_STATUS_WIDGET_ERROR = '[Dashboard] Get account status widget error';

export const GET_TRANSACTION_DEPOSIT_BREAKDOWN_GRAPH_WIDGET = '[Dashboard] Get transaction deposit break down graph widget';
export const GET_TRANSACTION_DEPOSIT_BREAKDOWN_GRAPH_WIDGET_COMPLETE = '[Dashboard] Get transaction deposit break down graph widget complete';
export const GET_TRANSACTION_DEPOSIT_BREAKDOWN_GRAPH_WIDGET_ERROR = '[Dashboard] Get transaction deposit break down graph widget error';

export const GET_TRANSACTION_WITHDRAWAL_WIDGET = '[Dashboard] Get transaction withdrawal widget';
export const GET_TRANSACTION_WITHDRAWAL_WIDGET_COMPLETE = '[Dashboard] Get transaction withdrawal widget Complete';
export const GET_TRANSACTION_WITHDRAWAL_WIDGET_ERROR = '[Dashboard] Get transaction withdrawal widget Error';

export const GET_TRANSACTION_BY_PAYMENTTYPE_WIDGET = '[Dashboard] Get transaction by payment type widget';
export const GET_TRANSACTION_BY_PAYMENTTYPE_WIDGET_COMPLETE = '[Dashboard] Get transaction by payment type widget complete';
export const GET_TRANSACTION_BY_PAYMENTTYPE_WIDGET_ERROR = '[Dashboard] Get transaction by payment type widget error';


export class ClearErrors implements Action {
    readonly type = CLEAR_ERRORS;
}

export class GetTransactionWidget implements Action {

    readonly type = GET_TRANSACTION_WIDGET;

    constructor(public payload: DashboardQuery) { }
}

export class GetTransactionWidgetComplete implements Action {
    readonly type = GET_TRANSACTION_WIDGET_COMPLETE;

    constructor(public payload: DashBoardWidget) { }
}

export class GetTransactionWidgetError implements Action {
    readonly type = GET_TRANSACTION_WIDGET_ERROR;

    constructor(public payload: string) { }
}

export class GetMerchantTransactionWidget implements Action {

    readonly type = GET_MERCHANT_TRANSACTION_WIDGET;

    constructor(public payload: DashboardQuery) { }
}

export class GetMerchantTransactionWidgetComplete implements Action {
    readonly type = GET_MERCHANT_TRANSACTION_WIDGET_COMPLETE;

    constructor(public payload: DashboardMerchantTransactionReportDTO[]) { }
}

export class GetMerchantTransactionWidgetError implements Action {
    readonly type = GET_MERCHANT_TRANSACTION_WIDGET_ERROR;

    constructor(public payload: string) { }
}

export class GetTransactionGraphWidget implements Action {

    readonly type = GET_TRANSACTION_GRAPH_WIDGET;

    constructor(public payload: DashboardQuery) { }
}

export class GetTransactionGraphWidgetComplete implements Action {
    readonly type = GET_TRANSACTION_GRAPH_WIDGET_COMPLETE;

    constructor(public payload: DashBoardChartWidget) { }
}

export class GetTransactionGraphWidgetError implements Action {
    readonly type = GET_TRANSACTION_GRAPH_WIDGET_ERROR;

    constructor(public payload: string) { }
}

export class GetAccountingGraphWidget implements Action {

    readonly type = GET_ACCOUNTING_GRAPH_WIDGET;

    constructor(public payload: DashboardQuery) { }
}

export class GetAccountingGraphWidgetComplete implements Action {
    readonly type = GET_ACCOUNTING_GRAPH_WIDGET_COMPLETE;

    constructor(public payload: DashBoardChartWidget) { }
}

export class GetAccountingGraphWidgetError implements Action {
    readonly type = GET_ACCOUNTING_GRAPH_WIDGET_ERROR;

    constructor(public payload: string) { }
}

export class GetAccountStatusWidget implements Action {

    readonly type = GET_ACCOUNT_STATUS_WIDGET;

    constructor(public payload: DashboardQuery) { }
}

export class GetAccountStatusWidgetComplete implements Action {
    readonly type = GET_ACCOUNT_STATUS_WIDGET_COMPLETE;

    constructor(public payload: DashboardAccountStatusDTO[]) { }
}

export class GetAccountStatusWidgetError implements Action {
    readonly type = GET_ACCOUNT_STATUS_WIDGET_ERROR;

    constructor(public payload: string) { }
}

export class GetTransactionDepositBreakDownGraphWidget implements Action {

    readonly type = GET_TRANSACTION_DEPOSIT_BREAKDOWN_GRAPH_WIDGET;

    constructor(public payload: DashboardQuery) { }
}

export class GetTransactionDepositBreakDownGraphWidgetComplete implements Action {
    readonly type = GET_TRANSACTION_DEPOSIT_BREAKDOWN_GRAPH_WIDGET_COMPLETE;
    
    constructor(public payload: DashBoardChartWidget) {
    }
}
export class GetTransactionDepositBreakDownGraphWidgetError implements Action {
    readonly type = GET_TRANSACTION_DEPOSIT_BREAKDOWN_GRAPH_WIDGET_ERROR;

    constructor(public payload: string) { }
}

export class GetTransactionWithdrawalWidget implements Action {

    readonly type = GET_TRANSACTION_WITHDRAWAL_WIDGET;

    constructor(public payload: DashboardQuery) { }
}

export class GetTransactionWithdrawalWidgetComplete implements Action {
    readonly type = GET_TRANSACTION_WITHDRAWAL_WIDGET_COMPLETE;

    constructor(public payload: DashBoardWidget) { }
}

export class GetTransactionWithdrawalWidgetError implements Action {
    readonly type = GET_TRANSACTION_WITHDRAWAL_WIDGET_ERROR;

    constructor(public payload: string) { }
}

export class GetTransactionByPaymentTypeWidget implements Action {

    readonly type = GET_TRANSACTION_BY_PAYMENTTYPE_WIDGET;

    constructor(public payload: DashboardQuery) { }
}

export class GetTransactionByPaymentTypeWidgetComplete implements Action {
    readonly type = GET_TRANSACTION_BY_PAYMENTTYPE_WIDGET_COMPLETE;

    constructor(public payload: DashboardMerchantTransactionReportDTO[]) { }
}

export class GeTransactionByPaymentTypeWidgetError implements Action {
    readonly type = GET_TRANSACTION_BY_PAYMENTTYPE_WIDGET_ERROR;

    constructor(public payload: string) { }
}


export type Actions = ClearErrors
    | GetTransactionWidget | GetTransactionWidgetComplete | GetTransactionWidgetError
    | GetMerchantTransactionWidget | GetMerchantTransactionWidgetComplete | GetMerchantTransactionWidgetError
    | GetTransactionGraphWidget | GetTransactionGraphWidgetComplete | GetTransactionGraphWidgetError
    | GetAccountingGraphWidget | GetAccountingGraphWidgetComplete | GetAccountingGraphWidgetError
    | GetAccountStatusWidget | GetAccountStatusWidgetComplete | GetAccountStatusWidgetError
    | GetTransactionDepositBreakDownGraphWidget | GetTransactionDepositBreakDownGraphWidgetComplete | GetTransactionDepositBreakDownGraphWidgetError
    | GetTransactionWithdrawalWidget | GetTransactionWithdrawalWidgetComplete | GetTransactionWithdrawalWidgetError
    | GetTransactionByPaymentTypeWidget | GetTransactionByPaymentTypeWidgetComplete | GeTransactionByPaymentTypeWidgetError;