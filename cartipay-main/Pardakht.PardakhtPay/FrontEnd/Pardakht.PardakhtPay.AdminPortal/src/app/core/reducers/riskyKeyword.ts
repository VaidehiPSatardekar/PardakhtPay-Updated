import * as riskyKeyword from '../actions/riskyKeyword';

export interface State {
    riskyKeywordDetail: string[];
    loading: boolean;
    error?: {
        updateError: string,
        getDetails: string
    };
    riskyKeywordUpdateSuccess: boolean;
}

const initialState: State = {
    riskyKeywordDetail: undefined,
    loading: false,
    error: undefined,
    riskyKeywordUpdateSuccess: false
};

export function reducer(state: State = initialState, action: riskyKeyword.Actions): State {
    switch (action.type) {

        case riskyKeyword.GET_DETAILS:
            return {
                ...state,
                loading: true,
                error: undefined,
                riskyKeywordDetail: undefined,
            };

        case riskyKeyword.GET_DETAILS_COMPLETE:
            return {
                ...state,
                loading: false,
                riskyKeywordDetail: action.payload,
            };

        case riskyKeyword.GET_DETAILS_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    updateError: '',
                    getDetails: action.payload
                }
            };
        case riskyKeyword.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                riskyKeywordUpdateSuccess: false
            };

        case riskyKeyword.EDIT_COMPLETE:
            return {
                ...state,
                loading: false,
                riskyKeywordUpdateSuccess: true
            };
        case riskyKeyword.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    updateError: action.payload,
                    getDetails: ''
                }
            };
        case riskyKeyword.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case riskyKeyword.CLEAR:
            return initialState;
        default:
            return state;
    }
}