import { Action } from '@ngrx/store';
import { ListSearchResponse } from '../../models/types';
import { MerchantCustomer, MerchantRelation, MerchantCustomerCardNumbers, CustomerPhoneNumbers } from '../../models/merchant-customer';
import { MerchantCustomerSearchArgs } from '../../models/merchant-customer';
import { RegisteredPhoneNumbers } from '../../models/user-segment-group';

export const SEARCH = '[MerchantCustomer] Search';
export const SEARCH_COMPLETE = '[MerchantCustomer] Search Complete';
export const SEARCH_ERROR = '[MerchantCustomer] Search Error';

export const GET_PHONE_NUMBER_RELATED_CUSTOMERS = '[MerchantCustomer] Get Phone Number Related Customers';
export const GET_PHONE_NUMBER_RELATED_CUSTOMERS_COMPLETE = '[MerchantCustomer] Get Phone Number Related Customers Complete';
export const GET_PHONE_NUMBER_RELATED_CUSTOMERS_ERROR = '[MerchantCustomer] Get Phone Number Related Customers Error';

export const GET_CARD_NUMBERS = '[MerchantCustomer] Get Customer Card Numbers';
export const GET_CARD_NUMBERS_COMPLETE = '[MerchantCustomer] Get Customer Card Numbers Complete';
export const GET_CARD_NUMBERS_ERROR = '[MerchantCustomer] Get Customer Card Number Error';


export const GET_DETAILS = '[MerchantCustomer] Get Details';
export const GET_DETAILS_COMPLETE = '[MerchantCustomer] Get Details Complete';
export const GET_DETAILS_ERROR = '[MerchantCustomer] Get Details Error';

export const EDIT = '[MerchantCustomer] Edit';
export const EDIT_COMPLETE = '[MerchantCustomer] Edit Complete';
export const EDIT_ERROR = '[MerchantCustomer] Edit Error';

export const DOWNLOAD_PHONENUMBERS = '[MerchantCustomer] Download Phone Numbers';
export const DOWNLOAD_PHONENUMBERS_COMPLETE = '[MerchantCustomer] Download Phone Numbers Complete';
export const DOWNLOAD_PHONENUMBERS_ERROR = '[MerchantCustomer] Download Phone Numbers Error';

export const GET_REGISTERED_PHONES = '[MerchantCustomer] Get Registered Phones';
export const GET_REGISTERED_PHONES_COMPLETE = '[MerchantCustomer] Get Registered Phones Complete';
export const GET_REGISTERED_PHONES_ERROR = '[MerchantCustomer] Get Registered Phones Error';

export const REMOVE_REGISTERED_PHONES = '[MerchantCustomer] Remove Registered Phones';
export const REMOVE_REGISTERED_PHONES_COMPLETE = '[MerchantCustomer] Remove Registered Phones Complete';
export const REMOVE_REGISTERED_PHONES_ERROR = '[MerchantCustomer] Remove Registered Phones Error';

export const CLEAR_ALL = '[MerchantCustomer] Clear All';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: MerchantCustomerSearchArgs) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: ListSearchResponse<MerchantCustomer[]>) {
    }
}

export class SearchError implements Action {
    readonly type = SEARCH_ERROR;

    constructor(public payload: string) {

    }
}

export class GetPhoneNumberRelatedCustomers implements Action {
    readonly type = GET_PHONE_NUMBER_RELATED_CUSTOMERS;

    constructor(public payload: number) {

    }
}

export class GetPhoneNumberRelatedCustomersComplete implements Action {
    readonly type = GET_PHONE_NUMBER_RELATED_CUSTOMERS_COMPLETE;

    constructor(public payload: MerchantRelation[]) {
    }
}

export class GetPhoneNumberRelatedCustomersError implements Action {
    readonly type = GET_PHONE_NUMBER_RELATED_CUSTOMERS_ERROR;

    constructor(public payload: string) {

    }
}

export class GetCustomerCardNumbers implements Action {
    readonly type = GET_CARD_NUMBERS;

    constructor(public payload: number) {

    }
}

export class GetCustomerCardNumbersComplete implements Action {
    readonly type = GET_CARD_NUMBERS_COMPLETE;

    constructor(public payload: MerchantCustomerCardNumbers[]) {
    }
}

export class GetCustomerCardNumbersError implements Action {
    readonly type = GET_CARD_NUMBERS_ERROR;

    constructor(public payload: string) {

    }
}

export class GetDetails implements Action {
    readonly type = GET_DETAILS;

    constructor(public payload: number) {

    }
}

export class GetDetailsComplete implements Action {
    readonly type = GET_DETAILS_COMPLETE;

    constructor(public payload: MerchantCustomer) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: MerchantCustomer) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: MerchantCustomer) {

    }
}

export class EditError implements Action {
    readonly type = EDIT_ERROR;

    constructor(public payload: string) {

    }
}

export class ClearAll implements Action {
    readonly type = CLEAR_ALL;

    constructor() {

    }
}

export class DownloadPhoneNumbers implements Action {
    readonly type = DOWNLOAD_PHONENUMBERS;

    constructor(public payload: MerchantCustomerSearchArgs) {

    }
}

export class DownloadPhoneNumbersComplete implements Action {
    readonly type = DOWNLOAD_PHONENUMBERS_COMPLETE;

    constructor(public payload: CustomerPhoneNumbers) {
    }
}

export class DownloadPhoneNumbersError implements Action {
    readonly type = DOWNLOAD_PHONENUMBERS_ERROR;

    constructor(public payload: string) {

    }

}
export class GetRegisteredPhones implements Action {
    readonly type = GET_REGISTERED_PHONES;

    constructor(public payload: number) {
    }
}

export class GetRegisteredPhonesComplete implements Action {
    readonly type = GET_REGISTERED_PHONES_COMPLETE;

    constructor(public payload: RegisteredPhoneNumbers[]) {

    }
}

export class GetRegisteredPhonesError implements Action {
    readonly type = GET_REGISTERED_PHONES_ERROR;

    constructor(public payload: string) {

    }
}

export class RemoveRegisteredPhones implements Action {
    readonly type = REMOVE_REGISTERED_PHONES;

    constructor(public id: number,public payload: RegisteredPhoneNumbers[]) {
    }
}

export class RemoveRegisteredPhonesComplete implements Action {
    readonly type = REMOVE_REGISTERED_PHONES_COMPLETE;

    constructor() {

    }
}

export class RemoveRegisteredPhonesError implements Action {
    readonly type = REMOVE_REGISTERED_PHONES_ERROR;

    constructor(public payload: string) {

    }
}

export type Actions =
    Search | SearchComplete | SearchError
    | GetDetails | GetDetailsComplete | GetDetailsError
    | Edit | EditComplete | EditError
    | GetPhoneNumberRelatedCustomers | GetPhoneNumberRelatedCustomersComplete | GetPhoneNumberRelatedCustomersError
    | GetCustomerCardNumbers | GetCustomerCardNumbersComplete | GetCustomerCardNumbersError
    | DownloadPhoneNumbers | DownloadPhoneNumbersComplete | DownloadPhoneNumbersError
    | GetRegisteredPhones | GetRegisteredPhonesComplete | GetRegisteredPhonesError
    | RemoveRegisteredPhones | RemoveRegisteredPhonesComplete | RemoveRegisteredPhonesError
    | ClearAll;