// tslint:disable:typedef
// tslint:disable:max-classes-per-file
// tslint:disable:no-reserved-keywords
import { Action } from '@ngrx/store';
import { TenantPlatformMap, CreateTenantPlatformMapRequest, Tenant, UpdateTenantPlatformMapRequest, Platform } from '../models/tenant.model';
import { DomainAvailabilityCheckRequest } from '../models/domain-management.model';
import { LookupItem, TenantPlatformMapFinancialDocumentSettings } from '../models/shared.model';
import { PaymentSetting } from '../models/payment-setting.model';

export const GET_TENANT_SELECT_LIST = '[TenantAdminManagement] Get Tenant Select List';
export const GET_TENANT_SELECT_LIST_COMPLETE = '[TenantAdminManagement] Get Tenant Select List Complete';
export const GET_TENANT_SELECT_LIST_ERROR = '[TenantAdminManagement] Get Tenant Select List Error';

export const GET_TENANT_LIST_WITH_CURRENCY = '[TenantAdminManagement] Get Tenant List with Currency';
export const GET_TENANT_LIST_WITH_CURRENCY_COMPLETE = '[TenantAdminManagement] Get Tenant List with Currency Complete';
export const GET_TENANT_LIST_WITH_CURRENCY_ERROR = '[TenantAdminManagement] Get Tenant List with Currency Error';

export const GET_TENANT_LIST_WITH_OPTIONS = '[TenantAdminManagement] Get Tenant List with Options';
export const GET_TENANT_LIST_WITH_OPTIONS_COMPLETE = '[TenantAdminManagement] Get Tenant List with Options Complete';
export const GET_TENANT_LIST_WITH_OPTIONS_ERROR = '[TenantAdminManagement] Get Tenant List with Options Error';

export const GET_TENANT_LIST = '[TenantAdminManagement] Get Tenant List';
export const GET_TENANT_LIST_COMPLETE = '[TenantAdminManagement] Get Tenant List Complete';
export const GET_TENANT_LIST_ERROR = '[TenantAdminManagement] Get Tenant List Error';

export const GET_TENANTS = '[TenantAdminManagement] Get Tenants ';
export const GET_TENANTS_COMPLETE = '[TenantAdminManagement] Get Tenants Complete';
export const GET_TENANTS_ERROR = '[TenantAdminManagement] Get Tenants Error';

export const GET_AD_TENANT_CURRENCY = '[TenantAdminManagement] Get Tenant Currency';
export const GET_AD_TENANT_CURRENCY_COMPLETE = '[TenantAdminManagement] Get Tenant Currency Complete';
export const GET_AD_TENANT_CURRENCY_ERROR = '[TenantAdminManagement] Get Tenant Currency Error';


export const GET_TENANT_MAPPING_LIST = '[TenantAdminManagement] Get Tenant Mapping List';
export const GET_TENANT_MAPPING_LIST_COMPLETE = '[TenantAdminManagement] Get Tenant Mapping List Complete';
export const GET_TENANT_MAPPING_LIST_ERROR = '[TenantAdminManagement] Get Tenant Mapping List Error';

export const CREATE_TENANT = '[TenantAdminManagement] Create Tenant';
export const CREATE_TENANT_COMPLETE = '[TenantAdminManagement] Create Tenant Complete';
export const CREATE_TENANT_ERROR = '[TenantAdminManagement] Create Tenant Error';

// export const GET_TENANT_DETAILS = '[TenantAdminManagement] Get Tenant Details';
// export const GET_TENANT_DETAILS_COMPLETE = '[TenantAdminManagement] Get Tenant Details Complete';
// export const GET_TENANT_DETAILS_ERROR = '[TenantAdminManagement] Get Tenant Details Error';

export const UPDATE_TENANT = '[TenantAdminManagement] Update Tenant';
export const UPDATE_TENANT_COMPLETE = '[TenantAdminManagement] Update Tenant Complete';
export const UPDATE_TENANT_ERROR = '[TenantAdminManagement] Update Tenant Error';

export const GET_CURRENCY_LIST = '[TenantAdminManagement] Get Currency List';
export const GET_CURRENCY_LIST_COMPLETE = '[TenantAdminManagement] Get Currency List Complete';

export const GET_LANGUAGE_LIST = '[TenantAdminManagement] Get Language List';
export const GET_LANGUAGE_LIST_COMPLETE = '[TenantAdminManagement] Get Language List Complete';

export const GET_COUNTRY_LIST = '[TenantAdminManagement] Get Country List';
export const GET_COUNTRY_LIST_COMPLETE = '[TenantAdminManagement] Get Country List Complete';

export const GET_TIMEZONE_LIST = '[TenantAdminManagement] Get TimeZone List';
export const GET_TIMEZONE_LIST_COMPLETE = '[TenantAdminManagement] Get TimeZone List Complete';

export const GET_PLATFORM_DETAILS = '[PlatformAdminManagement] Get Platform Details';
export const GET_PLATFORM_DETAILS_COMPLETE = '[PlatformAdminManagement] Get Platform Details Complete';
export const GET_PLATFORM_DETAILS_ERROR = '[PlatformAdminManagement] Get Platform Details Error';

export const GET_TENANT_PAYMENT_SETTING_LIST = '[PaymentAdminManagement] Get Payment List';
export const GET_TENANT_PAYMENT_SETTING_LIST_COMPLETE = '[PaymentAdminManagement] Get Payment List Complete';
export const GET_TENANT_PAYMENT_SETTING_LIST_ERROR = '[PaymentAdminManagement] Get Payment List Error';

export const GET_TENANT_AVAILABLE_PAYMENT_SETTING_LIST = '[PaymentAdminManagement] Get Available Tenant Payment List';
export const GET_TENANT_AVAILABLE_PAYMENT_SETTING_LIST_COMPLETE = '[PaymentAdminManagement] Get Available Tenant Payment List Complete';
export const GET_TENANT_AVAILABLE_PAYMENT_SETTING_LIST_ERROR = '[PaymentAdminManagement] Get Available Tenant Payment List Error';

export const GET_LOOKUP_LIST_ERROR = '[TenantAdminManagement] Get Lookup List Error';

export const POST_DOMAIN_AVAILABILITY_CHECK = '[TenantAdminManagement] Check Domain Availability';
export const POST_DOMAIN_AVAILABILITY_CHECK_COMPLETE = '[TenantAdminManagement] Check Domain Availability Complete';
export const POST_DOMAIN_AVAILABILITY_CHECK_ERROR = '[TenantAdminManagement] Check Domain Availability Error';

export const GET_TENANT_LANGUAGES = '[TenantAdminManagement] Get Tenant With Languages';
export const GET_TENANT_LANGUAGES_COMPLETE = '[TenantAdminManagement] Get Tenant With Languages Complete';
export const GET_TENANT_LANGUAGES_ERROR = '[TenantAdminManagement]  Get Tenant With Languages Error';

export const GET_DOCUMENT_LIST = '[TenantAdminManagement] Get Document List';
export const GET_DOCUMENT_LIST_COMPLETE = '[TenantAdminManagement] Get Document List Complete';
export const GET_DOCUMENT_LIST_ERROR = '[TenantAdminManagement] Get Document List Error';

export const GET_FIN_DOC_SETTINGS = '[TenantAdminManagement] Get Financial Document Setting List';
export const GET_FIN_DOC_SETTINGS_COMPLETE = '[TenantAdminManagement] Get Financial Document Setting List Complete';
export const GET_FIN_DOC_SETTINGS_ERROR = '[TenantAdminManagement] Get Financial Document Setting List Error';


export const CLEAR_ERRORS = '[TenantAdminManagement] Clear Errors';

export class GetTenantList implements Action {
  readonly type = GET_TENANT_LIST;

}

export class GetTenantsWithLanguages implements Action {
  readonly type = GET_TENANT_LANGUAGES;

  constructor(public payload: string) { }
}

export class GetTenantsWithLanguagesComplete implements Action {
  readonly type = GET_TENANT_LANGUAGES_COMPLETE;

  constructor(public payload: TenantPlatformMap) { }
}

export class GetTenantsWithLanguagesError implements Action {
  readonly type = GET_TENANT_LANGUAGES_ERROR;

  constructor(public payload: string | any | null) { }
}


export class GetTenantCurrency implements Action {
  readonly type = GET_AD_TENANT_CURRENCY;
  constructor(public payload: string) { }
}

export class GetTenantCurrencyComplete implements Action {
  readonly type = GET_AD_TENANT_CURRENCY_COMPLETE;

  constructor(public payload: TenantPlatformMap) { }
}

export class GetTenantCurrencyError implements Action {
  readonly type = GET_AD_TENANT_CURRENCY_ERROR;

  constructor(public payload: string) { }
}

export class GetTenants implements Action {
  readonly type = GET_TENANTS;
}

export class GetTenantsComplete implements Action {
  readonly type = GET_TENANTS_COMPLETE;

  constructor(public payload: TenantPlatformMap[]) { }
}

export class GetTenantsError implements Action {
  readonly type = GET_TENANTS_ERROR;

  constructor(public payload: string) { }
}

export class GetTenantListComplete implements Action {
  readonly type = GET_TENANT_LIST_COMPLETE;

  constructor(public payload: Tenant[]) { }
}

export class GetTenantListError implements Action {
  readonly type = GET_TENANT_LIST_ERROR;

  constructor(public payload: string) { }
}

export class GetTenantSelectList implements Action {
  readonly type = GET_TENANT_SELECT_LIST;
}

export class GetTenantSelectListComplete implements Action {
  readonly type = GET_TENANT_SELECT_LIST_COMPLETE;

  constructor(public payload: TenantPlatformMap[]) { }
}

export class GetTenantSelectListError implements Action {
  readonly type = GET_TENANT_SELECT_LIST_ERROR;

  constructor(public payload: string) { }
}

export class GetTenantListWithCurrency implements Action {
  readonly type = GET_TENANT_LIST_WITH_CURRENCY;
}

export class GetTenantListWithCurrencyComplete implements Action {
  readonly type = GET_TENANT_LIST_WITH_CURRENCY_COMPLETE;

  constructor(public payload: TenantPlatformMap[]) { }
}

export class GetTenantListWithCurrencyError implements Action {
  readonly type = GET_TENANT_LIST_WITH_CURRENCY_ERROR;

  constructor(public payload: string) { }
}

export class GetTenantListWithOptions implements Action {
  readonly type = GET_TENANT_LIST_WITH_OPTIONS;
}

export class GetTenantListWithOptionsComplete implements Action {
  readonly type = GET_TENANT_LIST_WITH_OPTIONS_COMPLETE;

  constructor(public payload: TenantPlatformMap[]) { }
}

export class GetTenantListWithOptionsError implements Action {
  readonly type = GET_TENANT_LIST_WITH_OPTIONS_ERROR;

  constructor(public payload: string) { }
}

export class GetTenantMappingList implements Action {
  readonly type = GET_TENANT_MAPPING_LIST;
}

export class GetTenantMappingListComplete implements Action {
  readonly type = GET_TENANT_MAPPING_LIST_COMPLETE;

  constructor(public payload: TenantPlatformMap[]) { }
}

export class GetTenantMappingListError implements Action {
  readonly type = GET_TENANT_MAPPING_LIST_ERROR;

  constructor(public payload: string) { }
}

export class CreateTenant implements Action {
  readonly type = CREATE_TENANT;

  constructor(public payload: CreateTenantPlatformMapRequest) { }
}

export class CreateTenantComplete implements Action {
  readonly type = CREATE_TENANT_COMPLETE;

  constructor(public payload: TenantPlatformMap) { }
}

export class CreateTenantError implements Action {
  readonly type = CREATE_TENANT_ERROR;

  constructor(public payload: string) { }
}

export class GetPlatformDetails implements Action {
  readonly type = GET_PLATFORM_DETAILS;
}

export class GetPlatformDetailsComplete implements Action {
  readonly type = GET_PLATFORM_DETAILS_COMPLETE;

  constructor(public payload: Platform) { }
}

export class GetPlatformDetailsError implements Action {
  readonly type = GET_PLATFORM_DETAILS_ERROR;

  constructor(public payload: string) { }
}

export class GetTenantPaymentSettingList implements Action {
  readonly type = GET_TENANT_PAYMENT_SETTING_LIST;
  constructor(public payload: string) { }
}

export class GetTenantPaymentSettingListComplete implements Action {
  readonly type = GET_TENANT_PAYMENT_SETTING_LIST_COMPLETE;
  constructor(public payload: PaymentSetting[]) { }
}

export class GetTenantPaymentSettingListError implements Action {
  readonly type = GET_TENANT_PAYMENT_SETTING_LIST_ERROR;
  constructor(public payload: string) { }
}

export class GetTenantAvailablePaymentSettings implements Action {
  readonly type = GET_TENANT_AVAILABLE_PAYMENT_SETTING_LIST;
  constructor(public payload: string) { }
}

export class GetTenantAvailablePaymentSettingsComplete implements Action {
  readonly type = GET_TENANT_AVAILABLE_PAYMENT_SETTING_LIST_COMPLETE;
  constructor(public payload: PaymentSetting[]) { }
}

export class GetTenantAvailablePaymentSettingsError implements Action {
  readonly type = GET_TENANT_AVAILABLE_PAYMENT_SETTING_LIST_ERROR;
  constructor(public payload: string) { }
}


// export class GetTenantDetails implements Action {
//   readonly type = GET_TENANT_DETAILS;

//   constructor(public tenantGuid: string) { }
// }

// export class GetTenantDetailsComplete implements Action {
//   readonly type = GET_TENANT_DETAILS_COMPLETE;

//   constructor(public payload: TenantPlatformMap) { }
// }

// export class GetTenantDetailsError implements Action {
//   readonly type = GET_TENANT_DETAILS_ERROR;

//   constructor(public payload: string) { }
// }

export class UpdateTenant implements Action {
  readonly type = UPDATE_TENANT;

  constructor(public payload: UpdateTenantPlatformMapRequest) { }
}

export class UpdateTenantComplete implements Action {
  readonly type = UPDATE_TENANT_COMPLETE;

  constructor(public payload: TenantPlatformMap) { }
}

export class UpdateTenantError implements Action {
  readonly type = UPDATE_TENANT_ERROR;

  constructor(public payload: string) { }
}

export class ClearErrors implements Action {
  readonly type = CLEAR_ERRORS;
}

export class GetCurrencyList implements Action {
  readonly type = GET_CURRENCY_LIST;
}

export class GetCurrencyListComplete implements Action {
  readonly type = GET_CURRENCY_LIST_COMPLETE;

  constructor(public payload: LookupItem[]) { }
}

export class GetLanguageList implements Action {
  readonly type = GET_LANGUAGE_LIST;
}

export class GetLanguageListComplete implements Action {
  readonly type = GET_LANGUAGE_LIST_COMPLETE;

  constructor(public payload: LookupItem[]) { }
}

export class GetCountryList implements Action {
  readonly type = GET_COUNTRY_LIST;
}

export class GetCountryListComplete implements Action {
  readonly type = GET_COUNTRY_LIST_COMPLETE;

  constructor(public payload: LookupItem[]) { }
}

export class GetTimeZoneList implements Action {
  readonly type = GET_TIMEZONE_LIST;
}

export class GetTimeZoneListComplete implements Action {
  readonly type = GET_TIMEZONE_LIST_COMPLETE;

  constructor(public payload: LookupItem[]) { }
}

export class GetLookupListError implements Action {
  readonly type = GET_LOOKUP_LIST_ERROR;

  constructor(public payload: string) { }
}

export class PostDomainAvailabilityCheck implements Action {
  readonly type = POST_DOMAIN_AVAILABILITY_CHECK;

  constructor(public domainAvailabilityCheckRequest: DomainAvailabilityCheckRequest) {
  }
}

export class PostDomainAvailabilityCheckComplete implements Action {
  readonly type = POST_DOMAIN_AVAILABILITY_CHECK_COMPLETE;

  constructor(public payload: any) {
  }
}

export class PostDomainAvailabilityCheckError implements Action {
  readonly type = POST_DOMAIN_AVAILABILITY_CHECK_ERROR;

  constructor(public payload: string) { }
}
export class GetDocumentList implements Action {
  readonly type = GET_DOCUMENT_LIST;
  constructor() { }
}
export class GetDocumentListComplete implements Action {
  readonly type = GET_DOCUMENT_LIST_COMPLETE;
  constructor(public payload: LookupItem[]) { }
}
export class GetDocumentListError implements Action {
  readonly type = GET_DOCUMENT_LIST_ERROR;
  constructor(public payload: string) { }
}

export class GetFinDocumentSettingList implements Action {
  readonly type = GET_FIN_DOC_SETTINGS;
  constructor() { }
}
export class GetFinDocumentSettingListComplete implements Action {
  readonly type = GET_FIN_DOC_SETTINGS_COMPLETE;
  constructor(public payload: TenantPlatformMapFinancialDocumentSettings[]) { }
}
export class GetFinDocumentSettingListError implements Action {
  readonly type = GET_FIN_DOC_SETTINGS_ERROR;
  constructor(public payload: string) { }
}
// /**
//  * Export a type alias of all actions in this action group
//  * so that reducers can easily compose action types
//  */
export type Actions =
  GetTenantsWithLanguages | GetTenantsWithLanguagesComplete | GetTenantsWithLanguagesError |
  GetTenantCurrency | GetTenantCurrencyComplete | GetTenantCurrencyError |
  GetTenants | GetTenantsComplete | GetTenantsError |
  GetTenantList | GetTenantListComplete | GetTenantListError |
  GetTenantMappingList | GetTenantMappingListComplete | GetTenantMappingListError |
  CreateTenant | CreateTenantComplete | CreateTenantError |
  GetPlatformDetails | GetPlatformDetailsComplete | GetPlatformDetailsError |
  UpdateTenant | UpdateTenantComplete | UpdateTenantError |
  PostDomainAvailabilityCheck | PostDomainAvailabilityCheckComplete | PostDomainAvailabilityCheckError |
  GetCurrencyList | GetCurrencyListComplete |
  GetLanguageList | GetLanguageListComplete |
  GetCountryList | GetCountryListComplete |
  GetTimeZoneList | GetTimeZoneListComplete |
  GetTenantPaymentSettingList | GetTenantPaymentSettingListComplete | GetTenantPaymentSettingListError |
  GetTenantAvailablePaymentSettings | GetTenantAvailablePaymentSettingsComplete | GetTenantAvailablePaymentSettingsError |
  GetTenantSelectList | GetTenantSelectListComplete | GetTenantSelectListError |
  GetTenantListWithCurrency | GetTenantListWithCurrencyComplete | GetTenantListWithCurrencyError |
  GetTenantListWithOptions | GetTenantListWithOptionsComplete | GetTenantListWithOptionsError |
  GetLookupListError | GetDocumentList | GetDocumentListComplete | GetDocumentListError |
  GetFinDocumentSettingList | GetFinDocumentSettingListComplete | GetFinDocumentSettingListError
  | ClearErrors;
