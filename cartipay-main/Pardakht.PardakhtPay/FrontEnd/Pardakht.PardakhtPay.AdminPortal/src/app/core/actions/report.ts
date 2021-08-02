import { Action } from '@ngrx/store';
import { DashBoardChartWidget, DepositBreakDownReport } from 'app/models/dashboard';
import { BankStatementReportSearchArgs, UserSegmentReportSearchArgs, UserSegmentReport, TenantBalance, TransactionReportSearchArgs, WithdrawalPaymentReportArgs, TenantBalanceSearchArgs } from 'app/models/report';
import { ListSearchResponse } from '../../models/types';

export const GET_USER_SEGMENT_REPORT = '[Report] Get User Segment Report';
export const GET_USER_SEGMENT_REPORT_COMPLETE = '[Report] Get User Segment Report Complete';
export const GET_USER_SEGMENT_REPORT_ERROR = '[Report] Get User Segment Report Error';

export const GET_TENANT_BALANCE = '[Report] Get Tenant Balance Report';
export const GET_TENANT_BALANCET_COMPLETE = '[Report] Get Tenant Balance Report Complete';
export const GET_TENANT_BALANCE_ERROR = '[Report] Get Tenant Balance Report Error';

export const GET_DEPOSIT_WITHDRAWAL_WIDGET = '[Report] Get Deposit Withdrawal Widget';
export const GET_DEPOSIT_WITHDRAWAL_WIDGET_COMPLETE = '[Report] Get Deposit Withdrawal Widget Complete';
export const GET_DEPOSIT_WITHDRAWAL_WIDGET_ERROR = '[Report] Get Deposit Withdrawal Widget Error';

export const GET_WITHDRAWAL_PAYMENT_WIDGET = '[Report] Get Withdrawal Payment Widget';
export const GET_WITHDRAWAL_PAYMENT_WIDGET_COMPLETE = '[Report] Get Withdrawal Payment Widget Complete';
export const GET_WITHDRAWAL_PAYMENT_WIDGET_ERROR = '[Report] Get Withdrawal Payment Widget Error';

export const GET_DEPOSIT_BY_ACCOUNTNUMBER_WIDGET = '[Report] Get Deposit By Account Number Widget';
export const GET_DEPOSIT_BY_ACCOUNTNUMBER_WIDGET_COMPLETE = '[Report] Get Deposit By Account Number Widget Complete';
export const GET_DEPOSIT_BY_ACCOUNTNUMBER_WIDGET_ERROR = '[Report] Get Deposit By Account Number Widget Error';

export const GET_DEPOSIT_BREAKDOWN_LIST = '[Report] Get Deposit Breakdown List';
export const GET_DEPOSIT_BREAKDOWN_LIST_COMPLETE = '[Report] Get Deposit Breakdown List Complete';
export const GET_DEPOSIT_BREAKDOWN_LIST_ERROR = '[Report] Get Deposit Breakdown List Widget Error';






export const CLEAR_ALL = '[Report] Clear All';


export class GetUserSegmentReport implements Action {
    readonly type = GET_USER_SEGMENT_REPORT;

    constructor(public payload: UserSegmentReportSearchArgs) {

    }
}

export class GetUserSegmentReportCompleted implements Action {
    readonly type = GET_USER_SEGMENT_REPORT_COMPLETE;

    constructor(public payload: UserSegmentReport[]) {

    }
}

export class GetUserSegmentReportError implements Action {
    readonly type = GET_USER_SEGMENT_REPORT_ERROR;

    constructor(public payload: string) {

    }
}

export class GetTenantBalance implements Action {
    readonly type = GET_TENANT_BALANCE;

    constructor(public payload: TenantBalanceSearchArgs) {

    }
}

export class GetTenantBalanceCompleted implements Action {
    readonly type = GET_TENANT_BALANCET_COMPLETE;

    constructor(public payload: TenantBalance[]) {

    }
}

export class GetTenantBalanceError implements Action {
    readonly type = GET_TENANT_BALANCE_ERROR;

    constructor(public payload: string) {

    }
}

export class GetDepositWithdrawalWidget implements Action {
    readonly type = GET_DEPOSIT_WITHDRAWAL_WIDGET;

    constructor(public payload: TransactionReportSearchArgs) {

    }
}

export class GetDepositWithdrawalWidgetComplete implements Action {
    readonly type = GET_DEPOSIT_WITHDRAWAL_WIDGET_COMPLETE;

    constructor(public payload: DashBoardChartWidget) {
    }
}

export class GetDepositWithdrawalWidgetError implements Action {
    readonly type = GET_DEPOSIT_WITHDRAWAL_WIDGET_ERROR;

    constructor(public payload: string) {

    }
}

export class GetWithdrawalPaymentWidget implements Action {
    readonly type = GET_WITHDRAWAL_PAYMENT_WIDGET;

    constructor(public payload: WithdrawalPaymentReportArgs) {

    }
}

export class GetWithdrawalPaymentWidgetComplete implements Action {
    readonly type = GET_WITHDRAWAL_PAYMENT_WIDGET_COMPLETE;

    constructor(public payload: DashBoardChartWidget) {
    }
}

export class GetWithdrawalPaymentWidgetError implements Action {
    readonly type = GET_WITHDRAWAL_PAYMENT_WIDGET_ERROR;

    constructor(public payload: string) {

    }
}

export class GetDepositByAccountNumberWidgetComplete implements Action {
    readonly type = GET_DEPOSIT_BY_ACCOUNTNUMBER_WIDGET_COMPLETE;

    constructor(public payload: DashBoardChartWidget) {
    }
}

export class GetDepositByAccountNumberWidgetError implements Action {
    readonly type = GET_DEPOSIT_BY_ACCOUNTNUMBER_WIDGET_ERROR;

    constructor(public payload: string) {

    }
}

export class GetDepositByAccountNumberWidget implements Action {
    readonly type = GET_DEPOSIT_BY_ACCOUNTNUMBER_WIDGET;

    constructor(public payload: TransactionReportSearchArgs) {

    }
}

export class GetDepositBreakDownListComplete implements Action {
    readonly type = GET_DEPOSIT_BREAKDOWN_LIST_COMPLETE;

    constructor(public payload: ListSearchResponse<DepositBreakDownReport[]>) {
    }
}

export class GetDepositBreakDownListError implements Action {
    readonly type = GET_DEPOSIT_BREAKDOWN_LIST_ERROR;

    constructor(public payload: string) {

    }
}

export class GetDepositBreakDownList implements Action {
    readonly type = GET_DEPOSIT_BREAKDOWN_LIST;

    constructor(public payload: TransactionReportSearchArgs) {

    }
}




export class ClearAll implements Action {
    readonly type = CLEAR_ALL;

    constructor() {

    }
}

export type Actions =
    GetUserSegmentReport | GetUserSegmentReportCompleted | GetUserSegmentReportError
    | GetTenantBalance | GetTenantBalanceCompleted | GetTenantBalanceError
    | GetDepositWithdrawalWidget | GetDepositWithdrawalWidgetComplete | GetDepositWithdrawalWidgetError
    | GetWithdrawalPaymentWidget | GetWithdrawalPaymentWidgetComplete | GetWithdrawalPaymentWidgetError
    | GetDepositByAccountNumberWidget | GetDepositByAccountNumberWidgetComplete | GetDepositByAccountNumberWidgetError
    | GetDepositBreakDownList | GetDepositBreakDownListComplete | GetDepositBreakDownListError
    | ClearAll;