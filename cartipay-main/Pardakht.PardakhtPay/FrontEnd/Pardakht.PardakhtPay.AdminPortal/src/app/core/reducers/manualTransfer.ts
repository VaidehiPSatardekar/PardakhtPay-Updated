import { ManualTransfer } from "../../models/manual-transfer";
import * as manualTransfer from '../actions/manualTransfer';
import { ListSearchResponse } from "../../models/types";
import { WithdrawalReceipt } from "../../models/withdrawal";

export interface State {
    manualTransfers: ListSearchResponse<ManualTransfer[]>;
    manualTransferDetails: ManualTransfer;
    loading: boolean;
    getDetailLoading: boolean;
    error?: {
        searchError: string,
        createManualTransfer: string,
        editManualTransfer: string,
        getDetails: string,
        deleteError: string,
        cancelError: string,
        receipt: string,
        cancelDetailError: string,
        retryDetailError: string,
        setAsCompletedError: string
    };
    manualTransferCreated: ManualTransfer;
    manualTransferCreateSuccess: boolean;
    manualTransferUpdateSuccess: boolean;
    manualTransferDeleteSuccess: boolean;
    manualTransferCancelSuccess: boolean;
    manualTransferDetailCancelSuccess: boolean;
    manualTransferDetailRetrySuccess: boolean;
    manualTransferDetailSetAsCompletedSuccess: boolean;
    receipt: WithdrawalReceipt;
}

const initialState: State = {
    manualTransfers: undefined,
    manualTransferDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    manualTransferCreated: undefined,
    manualTransferCreateSuccess: false,
    manualTransferUpdateSuccess: false,
    manualTransferDeleteSuccess: false,
    manualTransferCancelSuccess: false,
    manualTransferDetailCancelSuccess: false,
    manualTransferDetailRetrySuccess: false,
    manualTransferDetailSetAsCompletedSuccess: false,
    receipt: undefined
};

export function reducer(state: State = initialState, action: manualTransfer.Actions): State {
    switch (action.type) {

        case manualTransfer.SEARCH:
            return {
                ...state,
                manualTransfers: undefined,
                manualTransferDetails: undefined,
                loading: true,
                manualTransferCreated: undefined,
                error: undefined
            }

        case manualTransfer.SEARCH_COMPLETE:
            return {
                ...state,
                manualTransfers: action.payload,
                manualTransferDetails: undefined,
                loading: false,
                error: undefined,
                manualTransferCreated: undefined
            }

        case manualTransfer.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    searchError: action.payload
                }
            }

        case manualTransfer.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                manualTransferCreated: undefined,
                manualTransferCreateSuccess: false
            };

        case manualTransfer.CREATE_COMPLETE:
            return {
                ...state,
                loading: false,
                manualTransferCreated: action.payload,
                manualTransferCreateSuccess: true
            };

        case manualTransfer.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createManualTransfer: action.payload
                },
                manualTransferCreated: undefined,
                manualTransferCreateSuccess: false
            };

        case manualTransfer.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                manualTransferDetails: undefined,
            };

        case manualTransfer.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                manualTransferDetails: action.payload,
            };

        case manualTransfer.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                manualTransferCreated: undefined,
            };

        case manualTransfer.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                manualTransferCreated: undefined,
                manualTransferUpdateSuccess: false
            };

        case manualTransfer.EDIT_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    manualTransferCreated: undefined,
                    manualTransferUpdateSuccess: true
                };
            }

        case manualTransfer.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editManualTransfer: action.payload
                },
                manualTransferCreated: undefined,
            };

        case manualTransfer.CANCEL:
            return {
                ...state,
                loading: true,
                error: undefined,
                manualTransferCancelSuccess: false
            };

        case manualTransfer.CANCEL_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    manualTransferCancelSuccess: true
                };
            }

        case manualTransfer.CANCEL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    cancelError: action.payload
                },
                manualTransferCancelSuccess: false
            };

        case manualTransfer.CANCEL_DETAIL:
            return {
                ...state,
                loading: true,
                error: undefined,
                manualTransferDetailCancelSuccess: false
            };

        case manualTransfer.CANCEL_DETAIL_COMPLETE:
            {
                var item = state.manualTransferDetails;

                if (item != null) {
                    var index = item.details.findIndex(t => t.id == action.payload.id);

                    if (index != -1) {
                        item.details[index] = action.payload
                    }
                }

                return {
                    ...state,
                    loading: false,
                    manualTransferDetailCancelSuccess: true,
                    manualTransferDetails: {
                        ...item
                    }
                };
            }

        case manualTransfer.CANCEL_DETAIL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    cancelDetailError: action.payload
                },
                manualTransferDetailCancelSuccess: false
            };

        case manualTransfer.RETRY_DETAIL:
            return {
                ...state,
                loading: true,
                error: undefined,
                manualTransferDetailRetrySuccess: false
            };

        case manualTransfer.RETRY_DETAIL_COMPLETE:
            {
                var item = state.manualTransferDetails;

                if (item != null) {
                    var index = item.details.findIndex(t => t.id == action.payload.id);

                    if (index != -1) {
                        item.details[index] = action.payload
                    }
                }

                return {
                    ...state,
                    loading: false,
                    manualTransferDetailRetrySuccess: true,
                    manualTransferDetails: {
                        ...item
                    }
                };
            }

        case manualTransfer.RETRY_DETAIL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    retryDetailError: action.payload
                },
                manualTransferDetailRetrySuccess: false
            };

        case manualTransfer.SETASCOMPLETED_DETAIL:
            return {
                ...state,
                loading: true,
                error: undefined,
                manualTransferDetailSetAsCompletedSuccess: false
            };

        case manualTransfer.SETASCOMPLETED_DETAIL_COMPLETE:
            {
                var item = state.manualTransferDetails;

                if (item != null) {
                    var index = item.details.findIndex(t => t.id == action.payload.id);

                    if (index != -1) {
                        item.details[index] = action.payload
                    }
                }

                return {
                    ...state,
                    loading: false,
                    manualTransferDetailSetAsCompletedSuccess: true,
                    manualTransferDetails: {
                        ...item
                    }
                };
            }

        case manualTransfer.SETASCOMPLETED_DETAIL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    setAsCompletedError: action.payload
                },
                manualTransferDetailSetAsCompletedSuccess: false
            };

        case manualTransfer.DELETE:
            return {
                ...state,
                loading: true,
                error: undefined,
                manualTransferDeleteSuccess: false
            };

        case manualTransfer.DELETE_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    manualTransferDeleteSuccess: true
                };
            }

        case manualTransfer.DELETE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    deleteError: action.payload
                },
                manualTransferDeleteSuccess: false
            };
        case manualTransfer.GET_RECEIPT:
            return {
                ...state,
                receipt: undefined,
                loading: false,
                error: undefined
            };
        case manualTransfer.GET_RECEIPT_COMPLETE:
            return {
                ...state,
                receipt: action.payload,
                error: undefined
            };
        case manualTransfer.GET_RECEIPT_ERROR:
            return {
                ...state,
                receipt: undefined,
                error: {
                    ...state.error,
                    receipt: action.payload
                }
            }

        case manualTransfer.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case manualTransfer.CLEAR:
            return initialState;
        default:
            return state;
    }
}