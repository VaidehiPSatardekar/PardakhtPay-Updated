import * as dashboard from '../actions/dashboard';
import { DashBoardWidget, DashBoardChartWidget, DashboardAccountStatusDTO, DashboardMerchantTransactionReportDTO, DashboardTransactionPaymentTypeReportDTO } from '../../models/dashboard';
import { GetAccountingGraphWidget } from '../actions/dashboard';

export interface State {
    loading: boolean;
    error?: {
        getTransactionWidget: string,
        getMerchantTransactionWidget: string,
        getTransactionGraphWidget: string,
        getAccountingGraphWidget: string,
        getAccountStatusWidget: string,
        getTransactionDepositBreakDownGraphWidget: string,
        getTransactionWithdrawalWidget: string,
        getTransactionByPaymentTypeWidget: string,
    };
    transactionWidget: DashBoardWidget;
    merchantTransactionWidget: DashboardMerchantTransactionReportDTO[];
    transactionGraphWidget: DashBoardChartWidget;
    accountingGraphWidget: DashBoardChartWidget;
    accountStatusWidget: DashboardAccountStatusDTO[];
    transactionDepositBreakDownGraphWidget: DashBoardChartWidget;
    transactionWithdrawalWidget: DashBoardWidget;
    transactionByPaymentTypeWidget: DashboardTransactionPaymentTypeReportDTO[];
}

const initialState: State = {
    loading: false,
    error: {
        getTransactionWidget: undefined,
        getMerchantTransactionWidget: undefined,
        getTransactionGraphWidget: undefined,
        getAccountingGraphWidget: undefined,
        getAccountStatusWidget: undefined,
        getTransactionDepositBreakDownGraphWidget: undefined,
        getTransactionWithdrawalWidget: undefined,
        getTransactionByPaymentTypeWidget: undefined
    },
    transactionWidget: undefined,
    merchantTransactionWidget: undefined,
    transactionGraphWidget: undefined,
    accountingGraphWidget: undefined,
    accountStatusWidget: undefined,
    transactionDepositBreakDownGraphWidget: undefined,
    transactionWithdrawalWidget: undefined,
    transactionByPaymentTypeWidget: undefined
};

export function reducer(state: State = initialState, action: dashboard.Actions): State {
    switch (action.type) {

        case dashboard.CLEAR_ERRORS: {
            return {
                ...state,
                loading: false,
                transactionWidget: undefined,
                merchantTransactionWidget: undefined,
                transactionGraphWidget: undefined,
                accountingGraphWidget: undefined,
                accountStatusWidget: undefined,
                transactionDepositBreakDownGraphWidget: undefined,
                transactionByPaymentTypeWidget: undefined,
                error: {
                    getTransactionWidget: undefined,
                    getMerchantTransactionWidget: undefined,
                    getTransactionGraphWidget: undefined,
                    getAccountingGraphWidget: undefined,
                    getAccountStatusWidget: undefined,
                    getTransactionDepositBreakDownGraphWidget: undefined,
                    getTransactionWithdrawalWidget: undefined,
                    getTransactionByPaymentTypeWidget: undefined
                },
            };
        }

        case dashboard.GET_TRANSACTION_WIDGET: {
            return {
                ...state,
                loading: true,
                error: {
                    ...state.error,
                    getTransactionWidget: undefined
                },
                transactionWidget: undefined
            };
        }

        case dashboard.GET_TRANSACTION_WIDGET_COMPLETE: {
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getTransactionWidget: undefined
                },
                transactionWidget: action.payload
            };
        }

        case dashboard.GET_TRANSACTION_WIDGET_ERROR: {

            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getTransactionWidget: action.payload
                },
                transactionWidget: undefined
            };
        }

        case dashboard.GET_MERCHANT_TRANSACTION_WIDGET: {
            return {
                ...state,
                loading: true,
                error: {
                    ...state.error,
                    getMerchantTransactionWidget: undefined
                },
                merchantTransactionWidget: undefined
            };
        }

        case dashboard.GET_MERCHANT_TRANSACTION_WIDGET_COMPLETE: {
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getMerchantTransactionWidget: undefined
                },
                merchantTransactionWidget: action.payload
            };
        }

        case dashboard.GET_MERCHANT_TRANSACTION_WIDGET_ERROR: {

            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getMerchantTransactionWidget: action.payload
                },
                merchantTransactionWidget: undefined
            };
        }

        case dashboard.GET_TRANSACTION_GRAPH_WIDGET: {
            return {
                ...state,
                loading: true,
                error: {
                    ...state.error,
                    getTransactionGraphWidget: undefined
                },
                transactionGraphWidget: undefined
            };
        }

        case dashboard.GET_TRANSACTION_GRAPH_WIDGET_COMPLETE: {
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getTransactionGraphWidget: undefined
                },
                transactionGraphWidget: action.payload
            };
        }

        case dashboard.GET_TRANSACTION_GRAPH_WIDGET_ERROR: {

            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getTransactionGraphWidget: action.payload
                },
                transactionGraphWidget: undefined
            };
        }

        case dashboard.GET_ACCOUNTING_GRAPH_WIDGET: {
            return {
                ...state,
                loading: true,
                error: {
                    ...state.error,
                    getAccountingGraphWidget: undefined
                },
                accountingGraphWidget: undefined
            };
        }

        case dashboard.GET_ACCOUNTING_GRAPH_WIDGET_COMPLETE: {
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAccountingGraphWidget: undefined
                },
                accountingGraphWidget: action.payload
            };
        }

        case dashboard.GET_ACCOUNTING_GRAPH_WIDGET_ERROR: {

            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAccountingGraphWidget: action.payload
                },
                accountingGraphWidget: undefined
            };
        }

        case dashboard.GET_ACCOUNT_STATUS_WIDGET: {
            return {
                ...state,
                loading: true,
                error: {
                    ...state.error,
                    getAccountStatusWidget: undefined
                },
                accountStatusWidget: undefined
            };
        }

        case dashboard.GET_ACCOUNT_STATUS_WIDGET_COMPLETE: {
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAccountStatusWidget: undefined
                },
                accountStatusWidget: action.payload
            };
        }

        case dashboard.GET_ACCOUNT_STATUS_WIDGET_ERROR: {

            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAccountStatusWidget: action.payload
                },
                accountStatusWidget: undefined
            };
        }

        case dashboard.GET_TRANSACTION_DEPOSIT_BREAKDOWN_GRAPH_WIDGET: {
            return {
                ...state,
                loading: true,
                error: {
                    ...state.error,
                    getTransactionDepositBreakDownGraphWidget: undefined
                },
                transactionDepositBreakDownGraphWidget: undefined
            };
        }

        case dashboard.GET_TRANSACTION_DEPOSIT_BREAKDOWN_GRAPH_WIDGET_COMPLETE: {
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getTransactionDepositBreakDownGraphWidget: undefined
                },
                transactionDepositBreakDownGraphWidget: action.payload
            };
        }

        case dashboard.GET_TRANSACTION_DEPOSIT_BREAKDOWN_GRAPH_WIDGET_ERROR: {

            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getTransactionDepositBreakDownGraphWidget: action.payload
                },
                transactionDepositBreakDownGraphWidget: undefined
            };
        }

        case dashboard.GET_TRANSACTION_WITHDRAWAL_WIDGET: {
            return {
                ...state,
                loading: true,
                error: {
                    ...state.error,
                    getTransactionWithdrawalWidget: undefined
                },
                transactionWithdrawalWidget: undefined
            };
        }

        case dashboard.GET_TRANSACTION_WITHDRAWAL_WIDGET_COMPLETE: {
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getTransactionWithdrawalWidget: undefined
                },
                transactionWithdrawalWidget: action.payload
            };
        }

        case dashboard.GET_TRANSACTION_WITHDRAWAL_WIDGET_ERROR: {
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getTransactionWithdrawalWidget: action.payload
                },
                transactionWithdrawalWidget: undefined
            };
        }

        case dashboard.GET_TRANSACTION_BY_PAYMENTTYPE_WIDGET: {
            return {
                ...state,
                loading: true,
                error: {
                    ...state.error,
                    getTransactionByPaymentTypeWidget: undefined
                },
                transactionByPaymentTypeWidget: undefined
            };
        }

        case dashboard.GET_TRANSACTION_BY_PAYMENTTYPE_WIDGET_COMPLETE: {
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getTransactionByPaymentTypeWidget: undefined
                },
                transactionByPaymentTypeWidget: action.payload
            };
        }

        case dashboard.GET_TRANSACTION_BY_PAYMENTTYPE_WIDGET_ERROR: {

            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getTransactionByPaymentTypeWidget: action.payload
                },
                transactionByPaymentTypeWidget: undefined
            };
        }



        default: {
            return state;
        }
    }
}