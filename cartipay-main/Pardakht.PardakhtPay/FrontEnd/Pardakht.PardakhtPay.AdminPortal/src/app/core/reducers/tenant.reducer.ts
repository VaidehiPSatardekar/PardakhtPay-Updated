import * as tenant from './../actions/tenant.actions';
import { TenantPlatformMap, Tenant, Platform } from './../models/tenant.model';
import { PaymentSetting } from '../models/payment-setting.model';
import { DomainAvailabilityCheckRequest } from '../models/domain-management.model';
import { LookupItem, TenantPlatformMapFinancialDocumentSettings } from '../models/shared.model';

export interface State {
  tenantLookupList: Tenant[];
  tenantMappings: TenantPlatformMap[];
  tenants: TenantPlatformMap[];
  tenantSelectList: TenantPlatformMap[];
  tenantListwithCurrency: TenantPlatformMap[];
  tenantListwithLanguages: TenantPlatformMap;
  tenantwithCurrency: TenantPlatformMap;
  tenantListwithOptions: TenantPlatformMap[];
  loading: boolean;
  tenantCreated: TenantPlatformMap;
  tenantUpdateSuccess: boolean;
  currencyList: LookupItem[];
  languageList: LookupItem[];
  countryList: LookupItem[];
  timeZoneList: LookupItem[];
  documentList: LookupItem[];
  financialActionDocumentList: TenantPlatformMapFinancialDocumentSettings[];
  tenantPaymentSettings: PaymentSetting[];
  tenantAvailablePaymentSettings: PaymentSetting[];
  platformDetails: Platform;
  isDomainAvailable: boolean;
  domainAvailabilityRequest: DomainAvailabilityCheckRequest;
  error?: {
    listError: string,
    createError: string,
    editError: string,
    getDetailsError: string,
    genericError: string
  };
}

const initialState: State = {
  tenantLookupList: undefined,
  tenantListwithCurrency: undefined,
  tenantListwithLanguages: undefined,
  tenantwithCurrency: undefined,
  tenantListwithOptions: undefined,
  tenantMappings: undefined,
  tenants: undefined,
  tenantSelectList: undefined,
  loading: false,
  tenantCreated: undefined,
  tenantUpdateSuccess: false,
  isDomainAvailable: false,
  currencyList: undefined,
  languageList: undefined,
  countryList: undefined,
  timeZoneList: undefined,
  documentList: undefined,
  financialActionDocumentList: undefined,
  platformDetails: undefined,
  tenantPaymentSettings: undefined,
  tenantAvailablePaymentSettings: undefined,
  domainAvailabilityRequest: undefined,
  error: undefined
};

// tslint:disable-next-line:max-func-body-length
export function reducer(state: State = initialState, action: tenant.Actions): State {
  switch (action.type) {
    case tenant.GET_AD_TENANT_CURRENCY:
    case tenant.GET_TENANT_LIST:
    case tenant.GET_TENANT_SELECT_LIST:
    case tenant.GET_TENANT_LIST_WITH_CURRENCY:
    case tenant.GET_TENANT_LIST_WITH_OPTIONS:
    case tenant.GET_TENANT_PAYMENT_SETTING_LIST:
    case tenant.GET_TENANT_AVAILABLE_PAYMENT_SETTING_LIST:

      // case tenant.GET_CURRENCY_LIST:
      // case tenant.GET_LANGUAGE_LIST:
      return {
        ...state,
        loading: true,
        error: undefined
      };

    // case tenant.GET_PLATFORM_DETAILS:
    case tenant.GET_TENANTS:
    case tenant.GET_TENANT_MAPPING_LIST:
    case tenant.UPDATE_TENANT:
    case tenant.CREATE_TENANT:
      return {
        ...state,
        loading: true,
        tenantUpdateSuccess: false,
        tenantCreated: undefined,
        error: undefined
      };

    case tenant.UPDATE_TENANT_COMPLETE: {

      const mappings: TenantPlatformMap[] = [
        ...state.tenantMappings
      ];
      if (action.payload != null) {
        mappings.splice(mappings.findIndex((q: TenantPlatformMap) => q.tenantPlatformMapGuid === action.payload.tenantPlatformMapGuid), 1, action.payload);
      }

      return {
        ...state,
        tenantMappings: mappings,
        loading: false,
        tenantCreated: undefined,
        tenantUpdateSuccess: true
      };
    }
    case tenant.GET_AD_TENANT_CURRENCY_COMPLETE:
      return {
        ...state,
        tenantwithCurrency: action.payload,
        loading: false,
        error: undefined
      };
    case tenant.GET_TENANT_PAYMENT_SETTING_LIST_COMPLETE: {
      return {
        ...state,
        tenantPaymentSettings: action.payload,
        loading: false,
        error: undefined,
      };
    }

    case tenant.GET_TENANT_AVAILABLE_PAYMENT_SETTING_LIST_COMPLETE: {
      return {
        ...state,
        tenantAvailablePaymentSettings: action.payload,
        loading: false,
        error: undefined,
      };
    }

    case tenant.GET_TENANT_LIST_COMPLETE: {
      return {
        ...state,
        tenantLookupList: action.payload,
        loading: false,
        error: undefined,
      };
    }

    case tenant.GET_TENANT_SELECT_LIST_COMPLETE: {
      return {
        ...state,
        tenantSelectList: action.payload,
        loading: false,
        error: undefined,
      };
    }

    case tenant.GET_TENANT_LIST_WITH_CURRENCY_COMPLETE: {
      return {
        ...state,
        tenantListwithCurrency: action.payload,
        loading: false,
        error: undefined,
      };
    }

    case tenant.GET_TENANT_LIST_WITH_OPTIONS_COMPLETE: {
      return {
        ...state,
        tenantListwithOptions: action.payload,
        loading: false,
        error: undefined,
      };
    }

    case tenant.GET_CURRENCY_LIST_COMPLETE: {
      return {
        ...state,
        currencyList: action.payload,
        loading: false,
        error: undefined,
      };
    }

    case tenant.GET_LANGUAGE_LIST_COMPLETE: {
      return {
        ...state,
        languageList: action.payload,
        loading: false,
        error: undefined,
      };
    }
    case tenant.GET_DOCUMENT_LIST_COMPLETE: {
      return {
        ...state,
        documentList: action.payload,
        loading: false,
        error: undefined,
      };
    }
    case tenant.GET_FIN_DOC_SETTINGS_COMPLETE: {
      return {
        ...state,
        financialActionDocumentList: action.payload,
        loading: false,
        error: undefined,
      };
    }
    case tenant.GET_TENANT_LANGUAGES: {

      return {
        ...state,
        loading: true,
        tenantListwithLanguages: undefined,
        error: undefined,
      };
    }

    case tenant.GET_TENANT_LANGUAGES_COMPLETE: {
      return {
        ...state,
        tenantListwithLanguages: action.payload,
        loading: false,
        error: undefined,
      };
    }

    case tenant.GET_COUNTRY_LIST_COMPLETE: {
      return {
        ...state,
        countryList: action.payload,
        loading: false,
        error: undefined,
      };
    }

    case tenant.GET_TIMEZONE_LIST_COMPLETE: {
      return {
        ...state,
        timeZoneList: action.payload,
        loading: false,
        error: undefined,
      };
    }

    case tenant.GET_TENANT_MAPPING_LIST_COMPLETE: {
      return {
        ...state,
        tenantMappings: action.payload.map(payload => payload),
        loading: false,
        error: undefined,
        tenantUpdateSuccess: false,
        tenantCreated: undefined,
      };
    }
    case tenant.GET_TENANTS_COMPLETE: {
      return {
        ...state,
        tenants: action.payload.map(payload => payload),
        loading: false,
        error: undefined,
        tenantUpdateSuccess: false,
        tenantCreated: undefined,
      };
    }
    case tenant.GET_PLATFORM_DETAILS_COMPLETE: {
      return {
        ...state,
        platformDetails: action.payload,
        loading: false,
        error: undefined,
      };
    }

    case tenant.CREATE_TENANT_COMPLETE: {

      const mappings: TenantPlatformMap[] = [
        ...state.tenantMappings
      ];

      mappings.push(action.payload);

      return {
        ...state,
        tenantMappings: mappings,
        loading: false,
        tenantUpdateSuccess: false,
        tenantCreated: action.payload,
      };
    }
    case tenant.GET_TENANT_LANGUAGES_ERROR:
    case tenant.GET_AD_TENANT_CURRENCY_ERROR:
    case tenant.GET_TENANTS_ERROR:
    case tenant.GET_TENANT_LIST_ERROR:
    case tenant.GET_TENANT_SELECT_LIST_ERROR:
    case tenant.GET_TENANT_AVAILABLE_PAYMENT_SETTING_LIST_ERROR:
    case tenant.GET_TENANT_MAPPING_LIST_ERROR:
    case tenant.GET_TENANT_LIST_WITH_CURRENCY_ERROR:
    case tenant.GET_TENANT_LIST_WITH_OPTIONS_ERROR:
      return {
        ...state,
        loading: false,
        error: {
          listError: action.payload,
          createError: '',
          editError: '',
          getDetailsError: '',
          genericError: ''
        },
        tenantCreated: undefined,
        tenantUpdateSuccess: false,
      };

    case tenant.UPDATE_TENANT_ERROR: {
      return {
        ...state,
        loading: false,
        error: {
          listError: '',
          createError: '',
          getDetailsError: '',
          genericError: '',
          editError: action.payload
        },
        tenantCreated: undefined,
        tenantUpdateSuccess: false
      };
    }

    case tenant.CREATE_TENANT_ERROR:
      return {
        ...state,
        loading: false,
        error: {
          listError: '',
          createError: action.payload,
          getDetailsError: '',
          editError: '',
          genericError: ''
        },
        tenantCreated: undefined,
        tenantUpdateSuccess: false
      };
    case tenant.GET_DOCUMENT_LIST_ERROR:
    case tenant.GET_FIN_DOC_SETTINGS_ERROR:
    case tenant.GET_PLATFORM_DETAILS_ERROR:
    case tenant.GET_LOOKUP_LIST_ERROR:
      return {
        ...state,
        loading: false,
        error: {
          listError: '',
          createError: '',
          editError: '',
          getDetailsError: '',
          genericError: action.payload
        },
        tenantCreated: undefined,
        tenantUpdateSuccess: false,
      };

    case tenant.CLEAR_ERRORS:
      return {
        ...state,
        loading: false,
        tenantCreated: undefined,
        tenantUpdateSuccess: false,
        tenantListwithLanguages: undefined,
        error: undefined
      };

    case tenant.POST_DOMAIN_AVAILABILITY_CHECK:
      return {
        ...state,
        loading: true
      };

    case tenant.POST_DOMAIN_AVAILABILITY_CHECK_COMPLETE:
      return {
        ...state,
        isDomainAvailable: action.payload.success,
        loading: false
      };

    default: {
      return state;
    }
  }
}
