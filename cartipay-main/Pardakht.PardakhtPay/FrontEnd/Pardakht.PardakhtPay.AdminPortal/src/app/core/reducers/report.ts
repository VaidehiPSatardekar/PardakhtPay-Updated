import * as report from '../actions/report';
import { DashBoardChartWidget, DepositBreakDownReport } from 'app/models/dashboard';
import { UserSegmentReport, TenantBalance } from 'app/models/report';
import { ListSearchResponse } from '../../models/types';

export interface State {
    userSegmentReportItems: UserSegmentReport[];
    tenantBalances: TenantBalance[];
    depositWithdrawalWidget: DashBoardChartWidget;
    withdrawalPaymentWidget: DashBoardChartWidget;
    bankStatementsLoading: boolean;
    userSegmentReportLoading: boolean;
    tenantBalanceLoading: boolean;
    depositWithdrawalLoading: boolean;
    withdrawalPaymentLoading: boolean;
    depositByAccountNumberWidget: DashBoardChartWidget;
    depositByAccountNumberLoading: boolean;
    depositBreakDownList: ListSearchResponse<DepositBreakDownReport[]>;
    depositBreakDownListLoading: boolean;
    error?: {
        userSegmentReport: string,
        tenantBalance: string,
        depositWithdrawalWidget: string,
        withdrawalPayment: string,
        depositByAccountNumberWidget: string,
        depositBreakDownList:string
    };
}

const initialState: State = {
    userSegmentReportItems: undefined,
    tenantBalances: undefined,
    depositWithdrawalWidget: undefined,
    withdrawalPaymentWidget: undefined,
    bankStatementsLoading: false,
    userSegmentReportLoading: false,
    tenantBalanceLoading: false,
    depositWithdrawalLoading: false,
    withdrawalPaymentLoading: false,
    error: undefined,
    depositByAccountNumberWidget: undefined,
    depositByAccountNumberLoading: false,
    depositBreakDownList: undefined,
    depositBreakDownListLoading:false
}

export function reducer(state: State = initialState, action: report.Actions): State {
    switch (action.type) {
        case report.GET_USER_SEGMENT_REPORT:
            return {
                ...state,
                userSegmentReportItems: undefined,
                userSegmentReportLoading: false,
                error: undefined
            }

        case report.GET_USER_SEGMENT_REPORT_COMPLETE:
            return {
                ...state,
                userSegmentReportItems: action.payload,
                userSegmentReportLoading: false,
                error: undefined
            }

        case report.GET_USER_SEGMENT_REPORT_ERROR:
            return {
                ...state,
                userSegmentReportItems: undefined,
                userSegmentReportLoading: false,
                error: {
                    ...state.error,
                    userSegmentReport: action.payload
                }
            }

        case report.GET_TENANT_BALANCE: {
            return {
                ...state,
                tenantBalances: undefined,
                tenantBalanceLoading: true,
                error: undefined
            }
        }

        case report.GET_TENANT_BALANCET_COMPLETE: {
            return {
                ...state,
                tenantBalances: action.payload,
                tenantBalanceLoading: false,
                error: undefined
            }
        }

        case report.GET_TENANT_BALANCE_ERROR: {
            return {
                ...state,
                tenantBalances: undefined,
                tenantBalanceLoading: false,
                error: {
                    ...state.error,
                    tenantBalance: action.payload
                }
            }
        }

        case report.GET_DEPOSIT_WITHDRAWAL_WIDGET:

            return {
                ...state,
                depositWithdrawalWidget: undefined,
                depositWithdrawalLoading: true,
                error: undefined
            }

        case report.GET_DEPOSIT_WITHDRAWAL_WIDGET_COMPLETE:
            return {
                ...state,
                depositWithdrawalWidget: action.payload,
                depositWithdrawalLoading: false,
                error: undefined
            }

        case report.GET_DEPOSIT_WITHDRAWAL_WIDGET_ERROR:
            return {
                ...state,
                depositWithdrawalWidget: undefined,
                depositWithdrawalLoading: false,
                error: {
                    ...state.error,
                    depositWithdrawalWidget: action.payload
                }
            }
        case report.GET_WITHDRAWAL_PAYMENT_WIDGET:

            return {
                ...state,
                withdrawalPaymentWidget: undefined,
                withdrawalPaymentLoading: true,
                error: undefined
            }

        case report.GET_WITHDRAWAL_PAYMENT_WIDGET_COMPLETE:
            return {
                ...state,
                withdrawalPaymentWidget: action.payload,
                withdrawalPaymentLoading: false,
                error: undefined
            }

        case report.GET_WITHDRAWAL_PAYMENT_WIDGET_ERROR:
            return {
                ...state,
                withdrawalPaymentWidget: undefined,
                withdrawalPaymentLoading: false,
                error: {
                    ...state.error,
                    withdrawalPayment: action.payload
                }
            }
        case report.GET_DEPOSIT_BY_ACCOUNTNUMBER_WIDGET:

            return {
                ...state,
                depositByAccountNumberWidget: undefined,
                depositByAccountNumberLoading: true,
                error: undefined
            }

        case report.GET_DEPOSIT_BY_ACCOUNTNUMBER_WIDGET_COMPLETE:
            return {
                ...state,
                depositByAccountNumberWidget: action.payload,
                depositByAccountNumberLoading: false,
                error: undefined
            }

        case report.GET_DEPOSIT_BY_ACCOUNTNUMBER_WIDGET_ERROR:
            return {
                ...state,
                depositByAccountNumberWidget: undefined,
                depositByAccountNumberLoading: false,
                error: {
                    ...state.error,
                    depositByAccountNumberWidget: action.payload
                }
            }

        case report.GET_DEPOSIT_BREAKDOWN_LIST:

            return {
                ...state,
                depositBreakDownList: undefined,
                depositBreakDownListLoading: true,
                error: undefined
            }

        case report.GET_DEPOSIT_BREAKDOWN_LIST_COMPLETE:
            return {
                ...state,
                depositBreakDownList: action.payload,
                depositBreakDownListLoading: false,
                error: undefined
            }

        case report.GET_DEPOSIT_BREAKDOWN_LIST_ERROR:
            return {
                ...state,
                depositBreakDownList: undefined,
                depositBreakDownListLoading: false,
                error: {
                    ...state.error,
                    depositBreakDownList: action.payload
                }
            }
        case report.CLEAR_ALL:
            return initialState;
        default:
            return state;
    }
}