import * as withdrawal from '../actions/withdrawal';
import { Withdrawal, WithdrawalSearch, WithdrawalCreate, WithdrawalReceipt, WithdrawalTransferHistory } from '../../models/withdrawal';
import { ListSearchResponse } from '../../models/types';

export interface State {
    withdrawals: ListSearchResponse<WithdrawalSearch[]>;
    histories: WithdrawalTransferHistory[];
    withdrawalDetail: Withdrawal;    
    loading: boolean;
    error?: {
        searchError: string,
        createWithdrawal: string,
        editWithdrawal: string,
        getDetails: string,
        receipt: string,
        cancel: string,
        retry: string,
        setAsCompleted: string,
        changeProcessType: string,
        withdrawalCallbackToMerchantError: string;
        history
    };
    query: string;
    withdrawalCreated: Withdrawal;
    withdrawalUpdateSuccess: boolean;
    withdrawalCancelSuccess: boolean;
    withdrawalRetrySuccess: boolean;
    withdrawalSetAsCompletedSuccess: boolean;
    withdrawalChangeProcessTypeSuccess: boolean;
    withdrawalChangeAllProcessTypeSuccess: boolean;
    receipt: WithdrawalReceipt;
    callbackToMerchant: string;
    
}

const initialState: State = {
    withdrawals: undefined,
    histories: undefined,
    withdrawalDetail: undefined,
    loading: false,
    error: undefined,
    query: '',
    withdrawalCreated: undefined,
    withdrawalUpdateSuccess: false,
    withdrawalCancelSuccess: false,
    withdrawalRetrySuccess: false,
    withdrawalSetAsCompletedSuccess: false,
    withdrawalChangeProcessTypeSuccess: false,
    withdrawalChangeAllProcessTypeSuccess: false,    
    callbackToMerchant: undefined,
    receipt: undefined
};

export function reducer(state: State = initialState, action: withdrawal.Actions): State {
    switch (action.type) {
        case withdrawal.SEARCH:

            return {
                ...state,
                withdrawals: undefined,
                withdrawalDetail: undefined,
                loading: true,
                withdrawalCreated: undefined,
                error: undefined
            }

        case withdrawal.SEARCH_COMPLETE:
            return {
                ...state,
                withdrawals: action.payload,
                withdrawalDetail: undefined,
                loading: false,
                error: undefined,
                query: state.query,
                withdrawalCreated: undefined
            }

        case withdrawal.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    searchError: action.payload
                }
            }

        case withdrawal.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                withdrawalCreated: undefined,
            };

        case withdrawal.CREATE_COMPLETE:

            return {
                ...state,
                withdrawals: undefined,
                loading: false,
                withdrawalCreated: action.payload,
            };

        case withdrawal.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createWithdrawal: action.payload
                },
                withdrawalCreated: undefined,
            };

        case withdrawal.GET_DETAILS:
            return {
                ...state,
                loading: true,
                error: undefined,
                withdrawalDetail: undefined,
            };

        case withdrawal.GET_DETAILS_COMPLETE:
            return {
                ...state,
                loading: false,
                withdrawalDetail: action.payload,
            };

        case withdrawal.GET_DETAILS_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                withdrawalCreated: undefined,
            };
        case withdrawal.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                withdrawalCreated: undefined,
                withdrawalUpdateSuccess: false
            };

        case withdrawal.EDIT_COMPLETE:
            return {
                ...state,
                withdrawals: undefined,
                loading: false,
                withdrawalCreated: undefined,
                withdrawalUpdateSuccess: true
            };
        case withdrawal.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editWithdrawal: action.payload
                },
                withdrawalCreated: undefined,
            };
        case withdrawal.GET_RECEIPT:
            return {
                ...state,
                receipt: undefined,
                loading: false,
                error: undefined
            };
        case withdrawal.GET_RECEIPT_COMPLETE:
            return {
                ...state,
                receipt: action.payload,
                error: undefined
            };
        case withdrawal.GET_RECEIPT_ERROR:
            return {
                ...state,
                receipt: undefined,
                error: {
                    ...state.error,
                    receipt: action.payload
                }
            }
        case withdrawal.CANCEL:
            return {
                ...state,
                loading: false,
                withdrawalCancelSuccess: false,
                error: undefined
            };
        case withdrawal.CANCEL_COMPLETE:
            return {
                ...state,
                withdrawalCancelSuccess: true,
                error: undefined
            };
        case withdrawal.CANCEL_ERROR:
            return {
                ...state,
                withdrawalCancelSuccess: false,
                error: {
                    ...state.error,
                    cancel: action.payload
                }
            }
        case withdrawal.RETRY:
            return {
                ...state,
                loading: true,
                withdrawalRetrySuccess: false,
                error: undefined
            };
        case withdrawal.RETRY_COMPLETE:
            return {
                ...state,
                loading: false,
                withdrawalRetrySuccess: true,
                error: undefined
            };
        case withdrawal.RETRY_ERROR:
            return {
                ...state,
                loading: false,
                withdrawalRetrySuccess: false,
                error: {
                    ...state.error,
                    retry: action.payload
                }
            }
        case withdrawal.SET_AS_COMPLETED:
            return {
                ...state,
                loading: true,
                withdrawalSetAsCompletedSuccess: false,
                error: undefined
            };
        case withdrawal.SET_AS_COMPLETED_COMPLETE:
            return {
                ...state,
                loading: false,
                withdrawalSetAsCompletedSuccess: true,
                error: undefined
            };
        case withdrawal.SET_AS_COMPLETED_ERROR:
            return {
                ...state,
                loading: false,
                withdrawalSetAsCompletedSuccess: false,
                error: {
                    ...state.error,
                    setAsCompleted: action.payload
                }
            }
        case withdrawal.CHANGE_PROCESS_TYPE:
            return {
                ...state,
                loading: true,
                withdrawalChangeProcessTypeSuccess: false,
                error: undefined
            };
        case withdrawal.CHANGE_PROCESS_TYPE_COMPLETED:
            return {
                ...state,
                loading: false,
                withdrawalChangeProcessTypeSuccess: true,
                error: undefined
            };
        case withdrawal.CHANGE_PROCESS_TYPE_ERROR:
            return {
                ...state,
                loading: false,
                withdrawalChangeProcessTypeSuccess: false,
                error: {
                    ...state.error,
                    changeProcessType: action.payload
                }
            }
        case withdrawal.CHANGE_ALL_PROCESS_TYPE:
            return {
                ...state,
                loading: true,
                withdrawalChangeAllProcessTypeSuccess: false,
                error: undefined
            };
        case withdrawal.CHANGE_ALL_PROCESS_TYPE_COMPLETED:
            return {
                ...state,
                loading: false,
                withdrawalChangeAllProcessTypeSuccess: true,
                error: undefined
            };
        case withdrawal.CHANGE_ALL_PROCESS_TYPE_ERROR:
            return {
                ...state,
                loading: false,
                withdrawalChangeAllProcessTypeSuccess: false,
                error: {
                    ...state.error,
                    changeProcessType: action.payload
                }
            }

        case withdrawal.GET_HISTORY:
            return {
                ...state,
                error: undefined,
                histories: undefined
            };

        case withdrawal.GET_HISTORY_COMPLETE:
            return {
                ...state,
                histories: action.payload,
            };

        case withdrawal.GET_HISTORY_ERROR:
            return {
                ...state,
                error: {
                    ...state.error,
                    history: action.payload
                },
                histories: undefined
            };
        case withdrawal.CALLBACKTOMERCHANT:
            return {
                ...state,
                loading: true,
                callbackToMerchant: undefined,
                error: {
                    ...state.error,
                    withdrawalCallbackToMerchantError: undefined
                }
            };
        case withdrawal.CALLBACKTOMERCHANT_COMPLETE:
            return {
                ...state,
                loading: false,
                callbackToMerchant: action.payload,
                error: {
                    ...state.error,
                    withdrawalCallbackToMerchantError: undefined
                }
            };
        case withdrawal.CALLBACKTOMERCHANT_ERROR:
            return {
                ...state,
                loading: false,
                callbackToMerchant: undefined,
                error: {
                    ...state.error,
                    withdrawalCallbackToMerchantError: action.payload
                }
            }
        case withdrawal.CLEAR_ERRORS:
            return {
                ...state,
                error: undefined
            };
        case withdrawal.CLEAR:
            return initialState;
        default:
            return state;
    }
}