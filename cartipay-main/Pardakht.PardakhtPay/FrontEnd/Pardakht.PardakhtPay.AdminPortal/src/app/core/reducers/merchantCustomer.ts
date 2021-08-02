import * as merchantCustomer from '../actions/merchantCustomer';
import { ListSearchResponse } from '../../models/types';
import { MerchantCustomer, MerchantRelation, MerchantCustomerCardNumbers, CustomerPhoneNumbers } from '../../models/merchant-customer';
import { DownloadPhoneNumbers } from '../actions/merchantCustomer';
import { RegisteredPhoneNumbers } from '../../models/user-segment-group';

export interface State {
    items: ListSearchResponse<MerchantCustomer[]>;
    phoneNumberRelateds: MerchantRelation[];
    cardNumbers: MerchantCustomerCardNumbers[];
    loading: boolean;
    detailLoading: boolean;
    detail: MerchantCustomer;
    created: MerchantCustomer;
    updateSuccess: boolean;
    registeredPhoneNumbers: RegisteredPhoneNumbers[];
    registeredPhoneNumbersLoading: boolean;
    removeRegisteredPhonesLoading: boolean;
    removeRegisteredPhonesSuccess: boolean;
    downloadPhoneNumbers: CustomerPhoneNumbers;
    error?: {
        searchError: string,
        createError: string,
        detailError: string,
        phoneNumberRelated: string,
        cardNumbers: string,
        downloadPhoneNumbers: string,
        downloadPhoneNumbersError: string,
        getRegisteredPhone: string,
        removeRegisteredPhonesError: string;   
    };
}

const initialState: State = {
    items: undefined,
    phoneNumberRelateds: undefined,
    cardNumbers: undefined,
    created: undefined,
    updateSuccess: false,
    loading: false,
    error: undefined,
    detailLoading: false,
    detail: undefined,
    downloadPhoneNumbers: undefined,
    registeredPhoneNumbers: undefined,
    registeredPhoneNumbersLoading: undefined,
    removeRegisteredPhonesLoading: false,
    removeRegisteredPhonesSuccess: false
}

export function reducer(state: State = initialState, action: merchantCustomer.Actions): State {
    switch (action.type) {
        case merchantCustomer.SEARCH:

            return {
                ...state,
                items: undefined,
                loading: true,
                error: undefined
            }

        case merchantCustomer.SEARCH_COMPLETE:
            return {
                ...state,
                items: action.payload,
                loading: false,
                error: undefined
            }

        case merchantCustomer.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    searchError: action.payload
                }
            }

        case merchantCustomer.GET_DETAILS:
            return {
                ...state,
                detailLoading: true,
                error: undefined,
                detail: undefined,
            };

        case merchantCustomer.GET_DETAILS_COMPLETE:
            return {
                ...state,
                detailLoading: false,
                detail: action.payload,
            };

        case merchantCustomer.GET_DETAILS_ERROR:
            return {
                ...state,
                detailLoading: false,
                error: {
                    ...state.error,
                    detailError: action.payload
                },
                detail: undefined,
            };

        case merchantCustomer.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                created: undefined,
                updateSuccess: false
            };

        case merchantCustomer.EDIT_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    created: action.payload,
                    updateSuccess: true
                };
            }

        case merchantCustomer.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                updateSuccess: false,
                created: undefined,
                error: {
                    ...state.error,
                    createError: action.payload
                }
            };
        case merchantCustomer.GET_PHONE_NUMBER_RELATED_CUSTOMERS:

            return {
                ...state,
                phoneNumberRelateds: undefined,
                //loading: true,
                error: undefined
            }

        case merchantCustomer.GET_PHONE_NUMBER_RELATED_CUSTOMERS_COMPLETE:
            return {
                ...state,
                phoneNumberRelateds: action.payload,
                //loading: false,
                error: undefined
            }

        case merchantCustomer.GET_PHONE_NUMBER_RELATED_CUSTOMERS_ERROR:
            return {
                ...state,
                //loading: false,
                error: {
                    ...state.error,
                    phoneNumberRelated: action.payload
                }
            }
        case merchantCustomer.GET_CARD_NUMBERS:

            return {
                ...state,
                cardNumbers: undefined,
                //loading: true,
                error: undefined
            }

        case merchantCustomer.GET_CARD_NUMBERS_COMPLETE:
            return {
                ...state,
                cardNumbers: action.payload,
                //loading: false,
                error: undefined
            }

        case merchantCustomer.GET_CARD_NUMBERS_ERROR:
            return {
                ...state,
                //loading: false,
                error: {
                    ...state.error,
                    cardNumbers: action.payload
                }
            }

        case merchantCustomer.DOWNLOAD_PHONENUMBERS:

            return {
                ...state,
                downloadPhoneNumbers: undefined,
                loading: true,
                error: undefined
            }

        case merchantCustomer.DOWNLOAD_PHONENUMBERS_COMPLETE:
            return {
                ...state,
                downloadPhoneNumbers: action.payload,
                loading: false,
                error: undefined
            }

        case merchantCustomer.DOWNLOAD_PHONENUMBERS_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    downloadPhoneNumbersError: action.payload
                }
            }

        case merchantCustomer.GET_REGISTERED_PHONES:

            return {
                ...state,
                registeredPhoneNumbers: undefined,
                registeredPhoneNumbersLoading: true,
                error: undefined
            }

        case merchantCustomer.GET_REGISTERED_PHONES_COMPLETE:
            return {
                ...state,
                registeredPhoneNumbers: action.payload,
                registeredPhoneNumbersLoading: false,
                error: undefined
            }

        case merchantCustomer.GET_REGISTERED_PHONES_ERROR:
            return {
                ...state,
                registeredPhoneNumbersLoading: false,
                error: {
                    ...state.error,
                    getRegisteredPhone: action.payload
                }
            }

        case merchantCustomer.GET_REGISTERED_PHONES:
            return {
                ...state,
                registeredPhoneNumbers: undefined,
                registeredPhoneNumbersLoading: true,
                error: undefined
            }

        case merchantCustomer.REMOVE_REGISTERED_PHONES:
            return {
                ...state,
                removeRegisteredPhonesSuccess: false,
                removeRegisteredPhonesLoading: true,
                error: {
                    ...state.error,
                    removeRegisteredPhonesError: undefined
                }
            };

        case merchantCustomer.REMOVE_REGISTERED_PHONES_COMPLETE:
            return {
                ...state,
                removeRegisteredPhonesSuccess: true,
                removeRegisteredPhonesLoading: false,
                error: {
                    ...state.error,
                    removeRegisteredPhonesError: undefined
                }
            };

        case merchantCustomer.REMOVE_REGISTERED_PHONES_ERROR:
            return {
                ...state,
                removeRegisteredPhonesSuccess: false,
                removeRegisteredPhonesLoading: false,
                error: {
                    ...state.error,
                    removeRegisteredPhonesError: action.payload
                }
            }

        case merchantCustomer.CLEAR_ALL:
            return initialState;
        default:
            return state;
    }
}