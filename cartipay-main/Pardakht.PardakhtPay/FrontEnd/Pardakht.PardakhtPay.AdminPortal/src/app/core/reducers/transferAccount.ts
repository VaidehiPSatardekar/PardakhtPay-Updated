import { TransferAccount } from "../../models/transfer-account";
import * as transferAccount from '../actions/transferAccount';

export interface State {
    transferAccounts: TransferAccount[];
    allTransferAccounts: TransferAccount[];
    transferAccountDetails: TransferAccount;
    loading: boolean;
    getDetailLoading: boolean;
    updatePaymentGatewayDetails: TransferAccount;
    error?: {
        searchError: string,
        createTransferAccount: string,
        editTransferAccount: string,
        getDetails: string,
        getAll: string,
        deleteError: string
    };
    query: string;
    transferAccountCreated: TransferAccount;
    transferAccountUpdateSuccess: boolean;
    transferAccountDeleteSuccess: boolean;
}

const initialState: State = {
    transferAccounts: [],
    allTransferAccounts: [],
    transferAccountDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    transferAccountCreated: undefined,
    updatePaymentGatewayDetails: undefined,
    transferAccountUpdateSuccess: false,
    transferAccountDeleteSuccess: false
};

export function reducer(state: State = initialState, action: transferAccount.Actions): State {
    switch (action.type) {
        case transferAccount.SEARCH:

            return {
                ...state,
                transferAccounts: [],
                transferAccountDetails: undefined,
                loading: true,
                transferAccountCreated: undefined,
                error: undefined
            }

        case transferAccount.SEARCH_COMPLETE:
            return {
                ...state,
                transferAccounts: action.payload.map(payload => payload),
                transferAccountDetails: undefined,
                loading: false,
                error: undefined,
                query: state.query,
                transferAccountCreated: undefined
            }

        case transferAccount.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    searchError: action.payload
                }
            }
        case transferAccount.GET_ALL:

            return {
                ...state,
                allTransferAccounts : [],
                loading: true,
                error: undefined
            }

        case transferAccount.GET_ALL_COMPLETE:
            return {
                ...state,
                allTransferAccounts: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case transferAccount.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAll: action.payload
                }
            }

        case transferAccount.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                transferAccountCreated: undefined,
            };

        case transferAccount.CREATE_COMPLETE:


            const newTransferAccounts: TransferAccount[] = [
                ...state.transferAccounts
            ];
            newTransferAccounts.push(action.payload);
            return {
                ...state,
                transferAccounts: newTransferAccounts,
                loading: false,
                transferAccountCreated: action.payload,
            };

        case transferAccount.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createTransferAccount: action.payload
                },
                transferAccountCreated: undefined
            };

        case transferAccount.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                transferAccountDetails: undefined,
            };

        case transferAccount.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                transferAccountDetails: action.payload,
            };

        case transferAccount.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                transferAccountCreated: undefined,
            };



        case transferAccount.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                transferAccountCreated: undefined,
                transferAccountUpdateSuccess: false
            };

        case transferAccount.EDIT_COMPLETE:
            {

                const newTransferAccounts: TransferAccount[] = [
                    ...state.transferAccounts
                ];
                newTransferAccounts.splice(newTransferAccounts.findIndex(q => q.id === action.payload.id), 1, action.payload);
                return {
                    ...state,
                    transferAccounts: newTransferAccounts,
                    loading: false,
                    transferAccountCreated: undefined,
                    transferAccountUpdateSuccess: true
                };
            }

        case transferAccount.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editTransferAccount: action.payload
                },
                transferAccountCreated: undefined,
            };

        case transferAccount.DELETE:
            return {
                ...state,
                loading: true,
                error: undefined,
                transferAccountDeleteSuccess: false
            };

        case transferAccount.DELETE_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    transferAccountDeleteSuccess: true
                };
            }

        case transferAccount.DELETE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    deleteError: action.payload
                },
                transferAccountDeleteSuccess: false
            };
        case transferAccount.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case transferAccount.CLEAR:
            return initialState;
        default:
            return state;
    }
}