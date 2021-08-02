import { MobileTransferCardAccountGroup } from "../../models/mobile-transfer";
import * as mobileTransferCardAccountGroup from '../actions/mobileTransferAccountGroup';


export interface State {
    allMobileTransferCardAccountGroups: MobileTransferCardAccountGroup[];
    mobileTransferCardAccountGroupDetails: MobileTransferCardAccountGroup;
    loading: boolean;
    getDetailLoading: boolean;
    updated: MobileTransferCardAccountGroup;
    error?: {
        searchError: string,
        createMobileTransferCardAccountGroup: string,
        editMobileTransferCardAccountGroup: string,
        getDetails: string,
        getAll: string,
        deleteError: string
    };
    query: string;
    mobileTransferCardAccountGroupCreated: MobileTransferCardAccountGroup;
    mobileTransferCardAccountGroupUpdateSuccess: boolean;
    cardToCardAccountDeletedSuccess: boolean;
}

const initialState: State = {
    allMobileTransferCardAccountGroups: [],
    mobileTransferCardAccountGroupDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    mobileTransferCardAccountGroupCreated: undefined,
    updated: undefined,
    mobileTransferCardAccountGroupUpdateSuccess: false,
    cardToCardAccountDeletedSuccess: false
};

export function reducer(state: State = initialState, action: mobileTransferCardAccountGroup.Actions): State {
    switch (action.type) {
        case mobileTransferCardAccountGroup.GET_ALL:

            return {
                ...state,
                allMobileTransferCardAccountGroups: [],
                loading: true,
                error: undefined
            }

        case mobileTransferCardAccountGroup.GET_ALL_COMPLETE:
            return {
                ...state,
                allMobileTransferCardAccountGroups: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case mobileTransferCardAccountGroup.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAll: action.payload
                }
            }

        case mobileTransferCardAccountGroup.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                mobileTransferCardAccountGroupCreated: undefined,
            };

        case mobileTransferCardAccountGroup.CREATE_COMPLETE:
            
            return {
                ...state,
                loading: false,
                mobileTransferCardAccountGroupCreated: action.payload,
            };

        case mobileTransferCardAccountGroup.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createMobileTransferCardAccountGroup: action.payload
                },
                mobileTransferCardAccountGroupCreated: undefined,
            };

        case mobileTransferCardAccountGroup.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                mobileTransferCardAccountGroupDetails: undefined,
            };

        case mobileTransferCardAccountGroup.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                mobileTransferCardAccountGroupDetails: action.payload,
            };

        case mobileTransferCardAccountGroup.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                mobileTransferCardAccountGroupCreated: undefined,
            };

        case mobileTransferCardAccountGroup.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                mobileTransferCardAccountGroupCreated: undefined,
                mobileTransferCardAccountGroupUpdateSuccess: false
            };

        case mobileTransferCardAccountGroup.EDIT_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    mobileTransferCardAccountGroupCreated: undefined,
                    mobileTransferCardAccountGroupUpdateSuccess: true
                };
            }

        case mobileTransferCardAccountGroup.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editMobileTransferCardAccountGroup: action.payload
                },
                mobileTransferCardAccountGroupCreated: undefined,
            };

        case mobileTransferCardAccountGroup.DELETE:
            return {
                ...state,
                loading: true,
                error: undefined,
                cardToCardAccountDeletedSuccess: false
            };

        case mobileTransferCardAccountGroup.DELETE_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    cardToCardAccountDeletedSuccess: true
                };
            }

        case mobileTransferCardAccountGroup.DELETE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    deleteError: action.payload
                },
                cardToCardAccountDeletedSuccess: false
            };
        case mobileTransferCardAccountGroup.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case mobileTransferCardAccountGroup.CLEAR:
            return initialState;
        default:
            return state;
    }
}