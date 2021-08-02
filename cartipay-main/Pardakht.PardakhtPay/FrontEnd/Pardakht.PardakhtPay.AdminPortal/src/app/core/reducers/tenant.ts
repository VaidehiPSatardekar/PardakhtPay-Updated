import * as tenant from '../actions/tenant';
import { Tenant, TenantCreate } from '../../models/tenant';

export interface State {
    tenants: Tenant[];
    tenantDetail: Tenant;
    selectedTenant: Tenant;
    loading: boolean;
    error?: {
        searchError: string,
        createTenant: string,
        editTenant: string,
        getDetails: string
    };
    query: string;
    tenantCreated: Tenant;
    tenantUpdateSuccess: boolean;
}

const initialState: State = {
    tenants: undefined,
    selectedTenant: undefined,
    tenantDetail: undefined,
    loading: false,
    error: undefined,
    query: '',
    tenantCreated: undefined,
    tenantUpdateSuccess: false
};

export function reducer(state: State = initialState, action: tenant.Actions): State {
    switch (action.type) {
        case tenant.SEARCH:

            return {
                ...state,
                tenants: undefined,
                tenantDetail: undefined,
                loading: true,
                tenantCreated: undefined,
                error: undefined
            }

        case tenant.SEARCH_COMPLETE:
            return {
                ...state,
                tenants: action.payload,
                tenantDetail: undefined,
                loading: false,
                error: undefined,
                query: state.query,
                tenantCreated: undefined
            }

        case tenant.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: action.payload,
                    createTenant: '',
                    editTenant: '',
                    getDetails: ''
                }
            }

        case tenant.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                tenantCreated: undefined,
            };

        case tenant.CREATE_COMPLETE:

            return {
                ...state,
                tenants: undefined,
                loading: false,
                tenantCreated: action.payload,
            };

        case tenant.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: '',
                    createTenant: action.payload,
                    editTenant: '',
                    getDetails: ''
                },
                tenantCreated: undefined,
            };

        case tenant.GET_DETAILS:
            return {
                ...state,
                loading: true,
                error: undefined,
                tenantDetail: undefined,
            };

        case tenant.GET_DETAILS_COMPLETE:
            return {
                ...state,
                loading: false,
                tenantDetail: action.payload,
            };

        case tenant.GET_DETAILS_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: '',
                    createTenant: '',
                    editTenant: '',
                    getDetails: action.payload
                },
                tenantCreated: undefined,
            };

        case tenant.CHANGE_SELECTED_TENANT:
            return {
                ...state,
                selectedTenant: action.payload
            };

        case tenant.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case tenant.CLEAR:
            var newState = initialState;
            //newState.selectedTenant = state.selectedTenant;
            return newState;
        default:
            return state;
    }
}