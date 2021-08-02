import { UserSegmentGroup } from "../../models/user-segment-group";
import * as userSegmentGroup from '../actions/userSegmentGroup';


export interface State {
    allUserSegmentGroups: UserSegmentGroup[];
    userSegmentGroupDetails: UserSegmentGroup;
    loading: boolean;
    getDetailLoading: boolean;
    updated: UserSegmentGroup;
    error?: {
        searchError: string,
        createUserSegmentGroup: string,
        editUserSegmentGroup: string,
        getDetails: string,
        getAll: string,
        deleteError: string
    };
    query: string;
    userSegmentGroupCreated: UserSegmentGroup;
    userSegmentGroupUpdateSuccess: boolean;
    deleteSuccess: boolean;
}

const initialState: State = {
    allUserSegmentGroups: undefined,
    userSegmentGroupDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    userSegmentGroupCreated: undefined,
    updated: undefined,
    userSegmentGroupUpdateSuccess: false,
    deleteSuccess: false
};

export function reducer(state: State = initialState, action: userSegmentGroup.Actions): State {
    switch (action.type) {
        case userSegmentGroup.GET_ALL:

            return {
                ...state,
                allUserSegmentGroups: undefined,
                loading: true,
                error: undefined
            }

        case userSegmentGroup.GET_ALL_COMPLETE:
            return {
                ...state,
                allUserSegmentGroups: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case userSegmentGroup.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAll: action.payload
                }
            }

        case userSegmentGroup.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                userSegmentGroupCreated: undefined
            };

        case userSegmentGroup.CREATE_COMPLETE:
            
            return {
                ...state,
                loading: false,
                userSegmentGroupCreated: action.payload,
            };

        case userSegmentGroup.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createUserSegmentGroup: action.payload
                },
                userSegmentGroupCreated: undefined,
            };

        case userSegmentGroup.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                userSegmentGroupDetails: undefined
            };

        case userSegmentGroup.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                userSegmentGroupDetails: action.payload
            };

        case userSegmentGroup.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                userSegmentGroupCreated: undefined,
            };



        case userSegmentGroup.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                userSegmentGroupCreated: undefined,
                userSegmentGroupUpdateSuccess: false
            };

        case userSegmentGroup.EDIT_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    userSegmentGroupCreated: undefined,
                    userSegmentGroupUpdateSuccess: true
                };
            }

        case userSegmentGroup.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editUserSegmentGroup: action.payload
                },
                userSegmentGroupCreated: undefined,
            };

        case userSegmentGroup.DELETE:
            return {
                ...state,
                loading: true,
                error: undefined,
                deleteSuccess: false
            };

        case userSegmentGroup.DELETE_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    deleteSuccess: true
                };
            }

        case userSegmentGroup.DELETE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    deleteError: action.payload
                },
                deleteSuccess: false,
            };

        case userSegmentGroup.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case userSegmentGroup.CLEAR:
            return initialState;
        default:
            return state;
    }
}