import { getDictionaryForEnglish } from './reducers/localization';
// tslint:disable:typedef

import * as fromRouter from '@ngrx/router-store';
import { ActionReducer, ActionReducerMap, createFeatureSelector, createSelector, MetaReducer } from '@ngrx/store';

import { environment } from '../../environments/environment';
import { RouterStateUrl } from './reducers/router';

/**
 * storeFreeze prevents state from being mutated. When mutation occurs, an
 * exception will be thrown. This is useful during development mode to
 * ensure that none of the reducers accidentally mutates the state.
 */

// import { storeFreeze } from 'ngrx-store-freeze';

/**
 * Every reducer module's default export is the reducer function itself. In
 * addition, each module should export a type or interface that describes
 * the state of the reducer plus any selector functions. The `* as`
 * notation packages up all of the exports into a single object.
 */
 import * as fromNotification from './reducers/notification';

import * as fromAccount from './reducers/account';
import * as fromMerchant from './reducers/merchant';
import * as fromTransaction from './reducers/transaction';
import * as fromDashboard from './reducers/dashboard';
import * as fromAccounting from './reducers/accounting';
import * as fromWithdrawal from './reducers/withdrawal';
import * as fromMerchantBankAccount from './reducers/merchantBankAccount';
import * as fromTenant from './reducers/tenant';
import * as fromCardToCardAccount from './reducers/cardToCardAccount';
import * as fromBankLogin from './reducers/bankLogin';
import * as fromTransferAccount from './reducers/transferAccount';
import * as fromBankStatement from './reducers/bankStatement';
import * as fromTenantUrlConfig from './reducers/tenantUrlConfig';
import * as fromAutoTransfer from './reducers/autoTransfer';
import * as fromCardToCardAccountGroup from './reducers/cardToCardAccountGroup';
import * as fromMerchantCustomer from './reducers/merchantCustomer';
import * as fromUserSegmentGroup from './reducers/userSegmentGroup';
import * as fromApplicationSettings from './reducers/applicationSettings';
import * as fromRiskyKeyword from './reducers/riskyKeyword';
import * as fromManualTransfer from './reducers/manualTransfer';
import * as fromMobileTransferDevice from './reducers/mobileTransferDevice';
import * as fromMobileTransferCardAccount from './reducers/mobileTransferCardAccount';
import * as fromMobileTransferCardAccountGroup from './reducers/mobileTransferAccountGroup';
import * as fromBlockedPhoneNumber from './reducers/blockedPhoneNumber';
import * as fromCardHolder from './reducers/cardHolder';
import * as fromReport from './reducers/report';
import * as fromInvoiceOwnerSetting from './reducers/invoiceOwnerSetting';
import * as fromInvoice from './reducers/invoice';
import * as fromInvoicePayment from './reducers/invoicePayment';
import * as fromLoginDeviceStatus from './reducers/loginDeviceStatus';
import * as fromBlockedCardNumber from './reducers/blockedcardnumber';
import * as fromUser from './reducers/user';
import * as fromRole from './reducers/role';
import * as fromTenantManagement from './reducers/tenant.reducer';


/**
 * As mentioned, we treat each reducer like a table in a database. This means
 * our top level state interface is just a map of keys to inner state types.
 */
// tslint:disable-next-line:interface-name
export interface State {

    account: fromAccount.State;
    merchant: fromMerchant.State;
    transaction: fromTransaction.State;
    dashboard: fromDashboard.State;
    accounting: fromAccounting.State;
    withdrawal: fromWithdrawal.State;
    merchantBankAccount: fromMerchantBankAccount.State;
    tenant: fromTenant.State;
    cardToCardAccount: fromCardToCardAccount.State;
    bankLogin: fromBankLogin.State;
    transferAccount: fromTransferAccount.State;
    bankStatement: fromBankStatement.State;
    tenantUrlConfig: fromTenantUrlConfig.State;
    autoTransfer: fromAutoTransfer.State;
    cardToCardAccountGroup: fromCardToCardAccountGroup.State;
    merchantCustomer: fromMerchantCustomer.State;
    userSegmentGroup: fromUserSegmentGroup.State;
    applicationSettings: fromApplicationSettings.State;
    riskyKeyword: fromRiskyKeyword.State;
    manualTransfer: fromManualTransfer.State;
    mobileTransferDevice: fromMobileTransferDevice.State;
    mobileTransferCardAccount: fromMobileTransferCardAccount.State;
    mobileTransferCardAccountGroup: fromMobileTransferCardAccountGroup.State;
    blockedPhoneNumber: fromBlockedPhoneNumber.State;
    cardHolder: fromCardHolder.State;
    report: fromReport.State;
    invoiceOwnerSetting : fromInvoiceOwnerSetting.State;
    invoice: fromInvoice.State;
    invoicePayment: fromInvoicePayment.State;
    loginDeviceStatus: fromLoginDeviceStatus.State;
    blockedCardNumber: fromBlockedCardNumber.State;
  users: fromUser.State;
  roles: fromRole.State;
  adminManagementTenant: fromTenantManagement.State;

}

/**
 * Our state is composed of a map of action reducer functions.
 * These reducer functions are called with each dispatched action
 * and the current or initial state and return a new immutable state.
 */
export const reducers: ActionReducerMap<State> = {

    account: fromAccount.reducer,
    merchant: fromMerchant.reducer,
    transaction: fromTransaction.reducer,
    dashboard: fromDashboard.reducer,
    accounting: fromAccounting.reducer,
    withdrawal: fromWithdrawal.reducer,
    merchantBankAccount: fromMerchantBankAccount.reducer,
    tenant: fromTenant.reducer,
    cardToCardAccount: fromCardToCardAccount.reducer,
    bankLogin: fromBankLogin.reducer,
    transferAccount: fromTransferAccount.reducer,
    bankStatement: fromBankStatement.reducer,
    tenantUrlConfig: fromTenantUrlConfig.reducer,
    autoTransfer: fromAutoTransfer.reducer,
    cardToCardAccountGroup: fromCardToCardAccountGroup.reducer,
    merchantCustomer: fromMerchantCustomer.reducer,
    userSegmentGroup: fromUserSegmentGroup.reducer,
    applicationSettings: fromApplicationSettings.reducer,
    riskyKeyword: fromRiskyKeyword.reducer,
    manualTransfer: fromManualTransfer.reducer,
    mobileTransferDevice: fromMobileTransferDevice.reducer,
    mobileTransferCardAccount: fromMobileTransferCardAccount.reducer,
    mobileTransferCardAccountGroup: fromMobileTransferCardAccountGroup.reducer,
    blockedPhoneNumber: fromBlockedPhoneNumber.reducer,
    cardHolder: fromCardHolder.reducer,
    report: fromReport.reducer,
    invoiceOwnerSetting : fromInvoiceOwnerSetting.reducer,
    invoice: fromInvoice.reducer,
    invoicePayment: fromInvoicePayment.reducer,
    loginDeviceStatus: fromLoginDeviceStatus.reducer,
    blockedCardNumber: fromBlockedCardNumber.reducer,
  users: fromUser.reducer,
  roles: fromRole.reducer,
  adminManagementTenant: fromTenantManagement.reducer,


};

// console.log all actions
export function logger(reducer: ActionReducer<State>): ActionReducer<State> {
  // tslint:disable-next-line:no-function-expression
  return function (state: State, action: any): State {
    //console.log('state', state);
    //console.log('action', action);

    return reducer(state, action);
  };
}

/**
 * By default, @ngrx/store uses combineReducers with the reducer map to compose
 * the root meta-reducer. To add more meta-reducers, provide an array of meta-reducers
 * that will be composed to form the root meta-reducer.
 */
export const metaReducers: MetaReducer<State>[] = !environment.production
  ? [logger] // , storeFreeze]
  : [];

/**
* Router Reducers
*/
export const getRouterState = createFeatureSelector<fromRouter.RouterReducerState<RouterStateUrl>>('router');
export const getCurrentRoute = createSelector(getRouterState, state => state !== undefined ? state.state : undefined);


/**
 * Tenant Reducers
 */
export const getTenant = createFeatureSelector<fromTenantManagement.State>('adminManagementTenant');
export const getTenantLoadingv2 = createSelector(getTenant, state => state.loading);
export const getTenantMappingList = createSelector(getTenant, state => state.tenantMappings);
export const getTenants = createSelector(getTenant, state => state.tenants);
export const getTenantSelectList = createSelector(getTenant, state => state.tenantSelectList);
export const getTenantPaymentSettings = createSelector(getTenant, state => state.tenantPaymentSettings);
export const getTenantwithCurrencyList = createSelector(getTenant, state => state.tenantListwithCurrency);
export const getTenantCurrency = createSelector(getTenant, state => state.tenantwithCurrency);
export const getTenantLookupList = createSelector(getTenant, state => state.tenantLookupList);
export const getTenantCurrencyList = createSelector(getTenant, state => state.currencyList);
export const getTenantLanguageList = createSelector(getTenant, state => state.languageList);
export const getTenantDocumentList = createSelector(getTenant, state => state.documentList);
export const getTenantFinancialActionDocumentSettings = createSelector(getTenant, state => state.financialActionDocumentList);
export const getTenantCountryList = createSelector(getTenant, state => state.countryList);
export const getTenantTimeZoneList = createSelector(getTenant, state => state.timeZoneList);
export const getPlaformDetails = createSelector(getTenant, state => state.platformDetails);
export const getTenantwithOptionsList = createSelector(getTenant, state => state.tenantListwithOptions);
export const getTenantAvailablePaymentSettings = createSelector(getTenant, state => state.tenantAvailablePaymentSettings);
export const getTenantMappingListError = createSelector(getTenant, state => state.error !== undefined ? state.error.listError : undefined);
export const getTenantWithLanguages = createSelector(getTenant, state => state.tenantListwithLanguages);
export const getTenantCreatedv2 = createSelector(getTenant, state => state.tenantCreated);
export const getTenantCreateErrorv2 = createSelector(getTenant, state => state.error !== undefined ? state.error.createError : undefined);

export const getTenantUpdateSuccess = createSelector(getTenant, state => state.tenantUpdateSuccess);
export const getTenantUpdateError = createSelector(getTenant, state => state.error !== undefined ? state.error.editError : undefined);

export const postDomainAvailabilityRequestLoading = createSelector(getTenant, state => state.loading);
export const postDomainAvailabilityRequest = createSelector(getTenant, state => state.isDomainAvailable);



/**
 * User Reducers
 */
 export const getUserState = createFeatureSelector<fromUser.State>('users');
 export const getUserLoading = createSelector(getUserState, state => state.loading);
 export const getIsLoggedIn = createSelector(getUserState, state => state.isLoggedIn);
 export const getPasswordChanged = createSelector(getUserState, state => state.passwordChanged);
 export const getUserCreated = createSelector(getUserState, state => state.userCreated);
 export const getUserEditError = createSelector(getUserState, state => state.error !== undefined ? state.error.edit : undefined);
 export const getPasswordResetResult = createSelector(getUserState, state => state.passwordResetResult);
 export const getStaffUsers = createSelector(getUserState, state => state.users);
 export const getUserGetError = createSelector(getUserState, state => state.error !== undefined ? state.error.get : undefined);
 export const getUserLoginAsError = createSelector(getUserState, state => state.error == undefined ? undefined : state.error.loginAs);
 export const getPlatformConfig = createSelector(getUserState, state => state.platformConfig);

 
/***
 * Role Reducers
 */
export const getRolesState = createFeatureSelector<fromRole.State>('roles');
export const getRolesResults = createSelector(getRolesState, state => state.roles);
export const getPermissionsResults = createSelector(getRolesState, state => state.permissions);
export const getRolesLoading = createSelector(getRolesState, state => state.loading !== undefined ? state.loading : true);
export const getRolesError = createSelector(getRolesState, state => state.error);
export const getPermissionGroupsResults = createSelector(getRolesState, state => state.permissionGroups);
export const getRoleUpdateSuccess = createSelector(getRolesState, state => state.updateSuccess);
export const getRoleCreateSuccess = createSelector(getRolesState, state => state.createSuccess);



/**
 * Account Reducers
 */
export const getAccountState = createFeatureSelector<fromAccount.State>('account');
export const getAccountLoading = createSelector(getAccountState, state => state.loading);
export const getLoginError = createSelector(getAccountState, state => state.error !== undefined ? state.error.message : undefined);
export const getForgotPasswordError = createSelector(getAccountState, state => state.error !== undefined ? state.error.forgot : undefined);
export const getAccountErrors = createSelector(getAccountState, state => state.error);
// export const getIsLoggedIn = createSelector(getAccountState, state => state.isLoggedIn);
export const getLoggedOnUser = createSelector(getAccountState, state => state.user);
export const getPasswordChangeComplete = createSelector(getAccountState, state => state.passwordChanged);
export const getPasswordChangeLoading = createSelector(getAccountState, state => state.loading);
export const getPasswordChangeError = createSelector(getAccountState, state => state.error !== undefined ? state.error.message : undefined);
export const getAccountTenantGuid = createSelector(getAccountState, state => state.tenantGuid);
export const getAccountIsProviderAdmin = createSelector(getAccountState, state => state.isProviderAdmin);
export const getAccountIsProviderUser = createSelector(getAccountState, state => state.isProviderUser);
export const getAccountIsTenantAdmin = createSelector(getAccountState, state => state.isTenantAdmin);
export const getAccountIsStandardUser = createSelector(getAccountState, state => state.isStandardUser);
export const getAccountGuid = createSelector(getAccountState, state => state.accountGuid);
export const getUsername = createSelector(getAccountState, state => state.username);
export const getOwners = createSelector(getAccountState, state => state.owners);

// export const getUserCreated = createSelector(getAccountState, state => state.createdSuccess);
export const getUserCreateError = createSelector(getAccountState, state => state.error == undefined ? undefined : state.error.message);

export const getUserDetail = createSelector(getAccountState, state => state.detailUser);
export const getUserDetailError = createSelector(getAccountState, state => state.error == undefined ? undefined : state.error.message);
export const getUserUpdateSuccess = createSelector(getAccountState, state => state.updateSuccess);

/*
 * Merchant Reducers
 */

export const getMerchantState = createFeatureSelector<fromMerchant.State>('merchant');
export const getMerchantLoading = createSelector(getMerchantState, state => state.loading);
export const getMerchantSearchResults = createSelector(getMerchantState, state => state.merchants);
export const getMerchantSearchResultforValidate = createSelector(getMerchantState, state => state.merchants);
export const getMerchantSearchQuery = createSelector(getMerchantState, state => state.query);
export const getMerchantSearchError = createSelector(getMerchantState, state => state.error == undefined ? undefined : state.error.searchError);
export const getMerchantCreateError = createSelector(getMerchantState, state => state.error == undefined ? undefined : state.error.createMerchant);
export const getMerchantCreateSuccess = createSelector(getMerchantState, getMerchantLoading, (a, b) => a === undefined && b === false);
export const getMerchantEditError = createSelector(getMerchantState, state => state.error == undefined ? undefined : state.error.editMerchant);
export const getMerchantEditSuccess = createSelector(getMerchantState, state => state.merchantUpdateSuccess);
export const getMerchantDetails = createSelector(getMerchantState, state => state.merchantDetails);
export const getMerchantDetailError = createSelector(getMerchantState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getMerchantDetailLoading = createSelector(getMerchantState, state => state.getDetailLoading);
export const getMerchantCreated = createSelector(getMerchantState, state => state.merchantCreated);
export const getAllMerchants = createSelector(getMerchantState, state => state.allMerchants);
export const getAllMerchantError = createSelector(getMerchantState, state => state.error == undefined ? undefined : state.error.getAll);
export const getMerchantDeleteSuccess = createSelector(getMerchantState, state => state.deleteSuccess);
export const getMerchantDeleteError = createSelector(getMerchantState, state => state.error == undefined ? undefined : state.error.deleteError);

/*
 * Transaction Reducers
 */

export const getTransactionState = createFeatureSelector<fromTransaction.State>('transaction');
export const getTransactionLoading = createSelector(getTransactionState, state => state.loading);
export const getTransactionSearchResults = createSelector(getTransactionState, state => state.transactions);
export const getTransactionSearchError = createSelector(getTransactionState, state => state.error == undefined ? undefined : state.error.searchError);
export const getTransactionSetAsCompletedSuccess = createSelector(getTransactionState, state => state.setAsCompletedSuccess);
export const getTransactionSetAsCompletedError = createSelector(getTransactionState, state => state.error == undefined ? undefined : state.error.setAsCompleted);
export const getTransactionSetAsExpiredSuccess = createSelector(getTransactionState, state => state.setAsExpiredSuccess);
export const getTransactionSetAsExpiredError = createSelector(getTransactionState, state => state.error == undefined ? undefined : state.error.setAsExpired);
export const transactionCallbackToMerchant = createSelector(getTransactionState, state => state.callbackToMerchant);
export const transactionCallbackToMerchantError = createSelector(getTransactionState, state => state.error == undefined ? undefined : state.error.transactionCallbackToMerchantError);

/*
 * Dashboard Reducers 
 */

export const getDashboardState = createFeatureSelector<fromDashboard.State>('dashboard');
export const getDashboardLoading = createSelector(getDashboardState, state => state.loading);
export const getDashboardTransactionWidget = createSelector(getDashboardState, state => state.transactionWidget);
export const getDashboardMerchantTransactionWidget = createSelector(getDashboardState, state => state.merchantTransactionWidget);
export const getDashboardTransactionGraphWidget = createSelector(getDashboardState, state => state.transactionGraphWidget);
export const getDashboardAccountingGraphWidget = createSelector(getDashboardState, state => state.accountingGraphWidget);
export const getDashboardTransactionByPaymentTypeWidget = createSelector(getDashboardState, state => state.transactionByPaymentTypeWidget);

export const getDashboardTransactionWidgetError = createSelector(getDashboardState, state => state.error == undefined ? undefined : state.error.getTransactionWidget);
export const getDashboardMerchantTransactionWidgetError = createSelector(getDashboardState, state => state.error == undefined ? undefined : state.error.getMerchantTransactionWidget);
export const getDashboardTransactionGraphWidgetError = createSelector(getDashboardState, state => state.error == undefined ? undefined : state.error.getTransactionGraphWidget);
export const getDashboardAccountingGraphWidgetError = createSelector(getDashboardState, state => state.error == undefined ? undefined : state.error.getAccountingGraphWidget);
export const getDashboardAccountStatusWidget = createSelector(getDashboardState, state => state.accountStatusWidget);
export const getDashboardAccountStatusWidgetError = createSelector(getDashboardState, state => state.error == undefined ? undefined : state.error.getAccountStatusWidget);
export const getDashboardTransactionDepositBreakDownGraphWidgetError = createSelector(getDashboardState, state => state.error == undefined ? undefined : state.error.getTransactionDepositBreakDownGraphWidget);
export const getDashboardTransactionDepositBreakDownGraphWidget = createSelector(getDashboardState, state => state.transactionDepositBreakDownGraphWidget);
export const getDashboardTransactionWithdrawalWidget = createSelector(getDashboardState, state => state.transactionWithdrawalWidget);
export const getDashboardTransactionWithdrawalWidgetError = createSelector(getDashboardState, state => state.error == undefined ? undefined : state.error.getTransactionWithdrawalWidget);
export const getDashboardTransactionByPaymentTypeWidgetError = createSelector(getDashboardState, state => state.error == undefined ? undefined : state.error.getTransactionByPaymentTypeWidget);

/*
 * Accounting Reducers
 */

export const getAccountingState = createFeatureSelector<fromAccounting.State>('accounting');
export const getAccountingLoading = createSelector(getAccountingState, state => state.loading);
export const getAccountingSearchResults = createSelector(getAccountingState, state => state.items);
export const getAccountingSearchError = createSelector(getAccountingState, state => state.error == undefined ? undefined : state.error.searchError);

/*
 * Withdrawal Reducers
 */

export const getWithdrawalState = createFeatureSelector<fromWithdrawal.State>('withdrawal');
export const getWithdrawalLoading = createSelector(getWithdrawalState, state => state.loading);
export const getWithdrawalSearchResults = createSelector(getWithdrawalState, state => state.withdrawals);
export const getWithdrawalSearchResultforValidate = createSelector(getWithdrawalState, state => state.withdrawals);
export const getWithdrawalSearchQuery = createSelector(getWithdrawalState, state => state.query);
export const getWithdrawalSearchError = createSelector(getWithdrawalState, state => state.error == undefined ? undefined : state.error.searchError);
export const getWithdrawalCreateError = createSelector(getWithdrawalState, state => state.error == undefined ? undefined : state.error.createWithdrawal);
export const getWithdrawalCreateSuccess = createSelector(getWithdrawalState, getWithdrawalLoading, (a, b) => a === undefined && b === false);
export const getWithdrawalCreated = createSelector(getWithdrawalState, state => state.withdrawalCreated);
export const getWithdrawalReceipt = createSelector(getWithdrawalState, state => state.receipt);
export const getWithdrawalReceiptError = createSelector(getWithdrawalState, state => state.error == undefined ? undefined : state.error.receipt);
export const getWithdrawalCancelSuccess = createSelector(getWithdrawalState, state => state.withdrawalCancelSuccess);
export const getWithdrawalCancelError = createSelector(getWithdrawalState, state => state.error == undefined ? undefined : state.error.cancel);
export const getWithdrawalRetrySuccess = createSelector(getWithdrawalState, state => state.withdrawalRetrySuccess);
export const getWithdrawalRetryError = createSelector(getWithdrawalState, state => state.error == undefined ? undefined : state.error.retry);
export const getWithdrawalSetAsCompletedSuccess = createSelector(getWithdrawalState, state => state.withdrawalSetAsCompletedSuccess);
export const getWithdrawalSetAsCompletedError = createSelector(getWithdrawalState, state => state.error == undefined ? undefined : state.error.setAsCompleted);
export const getWithdrawalChangeProcessTypeSuccess = createSelector(getWithdrawalState, state => state.withdrawalChangeProcessTypeSuccess);
export const getWithdrawalChangeProcessTypeError = createSelector(getWithdrawalState, state => state.error == undefined ? undefined : state.error.changeProcessType);
export const getWithdrawalChangeAllProcessTypeSuccess = createSelector(getWithdrawalState, state => state.withdrawalChangeAllProcessTypeSuccess);
export const getWithdrawalHistories = createSelector(getWithdrawalState, state => state.histories);
export const getWithdrawalHistoryError = createSelector(getWithdrawalState, state => state.error == undefined ? undefined : state.error.history);
export const withdrawalCallbackToMerchant = createSelector(getWithdrawalState, state => state.callbackToMerchant);
export const withdrawalCallbackToMerchantError = createSelector(getWithdrawalState, state => state.error == undefined ? undefined : state.error.withdrawalCallbackToMerchantError);


/*
 * Merchant Bank Account Reducers
 */

export const getMerchantBankAccountState = createFeatureSelector<fromMerchantBankAccount.State>('merchantBankAccount');
export const getMerchantBankAccountLoading = createSelector(getMerchantBankAccountState, state => state.loading);
export const getMerchantBankAccountSearchResults = createSelector(getMerchantBankAccountState, state => state.items);
export const getMerchantBankAccountSearchError = createSelector(getMerchantBankAccountState, state => state.error == undefined ? undefined : state.error.getAccountsError);

/*
 * Tenant Reducers
 */

export const getTenantState = createFeatureSelector<fromTenant.State>('tenant');
export const getTenantLoading = createSelector(getTenantState, state => state.loading);
export const getTenantSearchResults = createSelector(getTenantState, state => state.tenants);
export const getTenantSearchResultforValidate = createSelector(getTenantState, state => state.tenants);
export const getTenantSearchQuery = createSelector(getTenantState, state => state.query);
export const getTenantSearchError = createSelector(getTenantState, state => state.error == undefined ? undefined : state.error.searchError);
export const getTenantCreateError = createSelector(getTenantState, state => state.error == undefined ? undefined : state.error.createTenant);
export const getTenantCreateSuccess = createSelector(getTenantState, getTenantLoading, (a, b) => a === undefined && b === false);
export const getTenantCreated = createSelector(getTenantState, state => state.tenantCreated);
export const getSelectedTenant = createSelector(getTenantState, state => state.selectedTenant);

/*
 * CardToCardAccount Reducers
 */

export const getCardToCardAccountState = createFeatureSelector<fromCardToCardAccount.State>('cardToCardAccount');
export const getCardToCardAccountLoading = createSelector(getCardToCardAccountState, state => state.loading);
export const getCardToCardAccountSearchResults = createSelector(getCardToCardAccountState, state => state.cardToCardAccounts);
export const getCardToCardAccountSearchResultforValidate = createSelector(getCardToCardAccountState, state => state.cardToCardAccounts);
export const getCardToCardAccountSearchQuery = createSelector(getCardToCardAccountState, state => state.query);
export const getCardToCardAccountSearchError = createSelector(getCardToCardAccountState, state => state.error == undefined ? undefined : state.error.searchError);
export const getCardToCardAccountCreateError = createSelector(getCardToCardAccountState, state => state.error == undefined ? undefined : state.error.createCardToCardAccount);
export const getCardToCardAccountCreateSuccess = createSelector(getCardToCardAccountState, getCardToCardAccountLoading, (a, b) => a === undefined && b === false);
export const getCardToCardAccountEditError = createSelector(getCardToCardAccountState, state => state.error == undefined ? undefined : state.error.editCardToCardAccount);
export const getCardToCardAccountEditSuccess = createSelector(getCardToCardAccountState, state => state.cardToCardAccountUpdateSuccess);
export const getCardToCardAccountDetails = createSelector(getCardToCardAccountState, state => state.cardToCardAccountDetails);
export const getCardToCardAccountDetailError = createSelector(getCardToCardAccountState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getCardToCardAccountDetailLoading = createSelector(getCardToCardAccountState, state => state.getDetailLoading);
export const getCardToCardAccountCreated = createSelector(getCardToCardAccountState, state => state.cardToCardAccountCreated);
export const getAllCardToCardAccounts = createSelector(getCardToCardAccountState, state => state.allCardToCardAccounts);
export const getAllCardToCardAccountError = createSelector(getCardToCardAccountState, state => state.error == undefined ? undefined : state.error.getAll);

/*
 * CardToCardAccountGroup Reducers
 */

export const getCardToCardAccountGroupState = createFeatureSelector<fromCardToCardAccountGroup.State>('cardToCardAccountGroup');
export const getCardToCardAccountGroupLoading = createSelector(getCardToCardAccountGroupState, state => state.loading);
export const getCardToCardAccountGroupCreateError = createSelector(getCardToCardAccountGroupState, state => state.error == undefined ? undefined : state.error.createCardToCardAccountGroup);
export const getCardToCardAccountGroupCreateSuccess = createSelector(getCardToCardAccountGroupState, getCardToCardAccountGroupLoading, (a, b) => a === undefined && b === false);
export const getCardToCardAccountGroupEditError = createSelector(getCardToCardAccountGroupState, state => state.error == undefined ? undefined : state.error.editCardToCardAccountGroup);
export const getCardToCardAccountGroupEditSuccess = createSelector(getCardToCardAccountGroupState, state => state.cardToCardAccountGroupUpdateSuccess);
export const getCardToCardAccountGroupDetails = createSelector(getCardToCardAccountGroupState, state => state.cardToCardAccountGroupDetails);
export const getCardToCardAccountGroupDetailError = createSelector(getCardToCardAccountGroupState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getCardToCardAccountGroupDetailLoading = createSelector(getCardToCardAccountGroupState, state => state.getDetailLoading);
export const getCardToCardAccountGroupCreated = createSelector(getCardToCardAccountGroupState, state => state.cardToCardAccountGroupCreated);
export const getAllCardToCardAccountGroups = createSelector(getCardToCardAccountGroupState, state => state.allCardToCardAccountGroups);
export const getAllCardToCardAccountGroupError = createSelector(getCardToCardAccountGroupState, state => state.error == undefined ? undefined : state.error.getAll);
export const getCardToCardAccountGroupDeleteSuccess = createSelector(getCardToCardAccountGroupState, state => state.cardToCardAccountDeletedSuccess);
export const getCardToCardAccountGroupDeleteError = createSelector(getCardToCardAccountGroupState, state => state.error == undefined ? undefined : state.error.deleteError);

/*
 * Bank Login Reducers
 */

export const getBankLoginState = createFeatureSelector<fromBankLogin.State>('bankLogin');
export const getBankLoginLoading = createSelector(getBankLoginState, state => state.loading);
export const getBankLoginSearchResults = createSelector(getBankLoginState, state => state.bankLogins);
export const getBankLoginSearchError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.searchError);


export const getBankAccountsLoading = createSelector(getBankLoginState, state => state.accountsLoading);
export const getBankAccountSearchResults = createSelector(getBankLoginState, state => state.bankAccounts);
export const getBankAccountSearchError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.searchBankAccountError);

export const getBanksLoading = createSelector(getBankLoginState, state => state.banksLoading);
export const getBanksSearchResult = createSelector(getBankLoginState, state => state.banks);
export const getBanksSearchError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.searchBanksError);

export const getBankLoginCreated = createSelector(getBankLoginState, state => state.created);
export const getBankLoginCreateSuccess = createSelector(getBankLoginState, state => state.createSuccess);
export const getBankLoginCreateError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.createError);
export const getBankLoginUpdateSuccess = createSelector(getBankLoginState, state => state.updateSuccess);

export const getBankLoginDetail = createSelector(getBankLoginState, state => state.detail);
export const getBankLoginDetailLoading = createSelector(getBankLoginState, state => state.getDetailLoading);
export const getBankLoginDetailError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.getDetailError);

export const getBankLoginUpdateInformationSuccess = createSelector(getBankLoginState, state => state.updateLoginSuccess);
export const getBankLoginUpdateInformationError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.updateLogin);

export const getCreateBankLoginFromRequestSuccess = createSelector(getBankLoginState, state => state.createLoginSuccess);
export const getCreateBankLoginFromRequestLoading = createSelector(getBankLoginState, state => state.createLoginFromLoginRequestLoading);
export const getCreateBankLoginFromRequestError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.createLoginError);

export const getOwnerBankLogins = createSelector(getBankLoginState, state => state.ownerLogins);
export const getOwnerBankLoginsLoading = createSelector(getBankLoginState, state => state.ownerBankLoginLoading);
export const getOwnerBankLoginError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.ownerBankLoginError);

export const deactivateOwnerLoginProcessing = createSelector(getBankLoginState, state => state.deactivatingLogin);
export const deactivateOwnerLoginSuccess = createSelector(getBankLoginState, state => state.deactiveLoginSuccess);
export const deactivateOwnerLoginError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.deactivaLoginError);

export const deleteOwnerLoginProcessing = createSelector(getBankLoginState, state => state.deletingLogin);
export const deleteOwnerLoginSuccess = createSelector(getBankLoginState, state => state.deleteLoginSuccess);
export const deleteOwnerLoginError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.deleteLoginError);

export const activateOwnerLoginProcessing = createSelector(getBankLoginState, state => state.activatingLogin);
export const activateOwnerLoginSuccess = createSelector(getBankLoginState, state => state.activateLoginSuccess);
export const activateOwnerLoginError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.activateLoginError);

export const getBlockedCardDetails = createSelector(getBankLoginState, state => state.blockedCards);
export const getBlockedCardDetailsError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.blockedCardError);
export const getBlockedCardDetailsLoading = createSelector(getBankLoginState, state => state.blockedCardDetailLoading);

export const getPassword = createSelector(getBankLoginState, state => state.password);
export const getPasswordError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.showPasswordError);

export const qrCodeRegisterProcessing = createSelector(getBankLoginState, state => state.qrCodeRegister);
export const qrCodeRegisterSuccess = createSelector(getBankLoginState, state => state.qrCodeRegisterSuccess);
export const qrCodeRegisterError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.qrCodeRegisterError);

export const getQrCodeRegistrationCompleted = createSelector(getBankLoginState, state => state.qrCodeRegistrationCompleted);

export const getLoginDeviceStatusState = createFeatureSelector<fromLoginDeviceStatus.State>('loginDeviceStatus');
export const getLoginDeviceStatus = createSelector(getLoginDeviceStatusState, state => state.loginDeviceStatusDesc);
export const getLoginDeviceStatusError = createSelector(getLoginDeviceStatusState, state => state.error == undefined ? undefined : state.error.showloginDeviceStatusDescError);

export const getOTP = createSelector(getBankLoginState, state => state.getOTP);
export const getOTPError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.getOTPError);

export const getBankLoginRequestRegisteredSuccess = createSelector(getBankLoginState, state => state.registerLoginRequestSuccess);
export const getBankLoginRequestRegisteredLoading = createSelector(getBankLoginState, state => state.registerLoginRequestLoading);
export const getBankLoginRequestRegisteredError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.registerLoginRequestError);

export const switchBankConnectionProgram = createSelector(getBankLoginState, state => state.switchBankConnectionProgram);
export const switchBankConnectionProgramError = createSelector(getBankLoginState, state => state.error == undefined ? undefined : state.error.switchBankConnectionProgramError);


/*
 * TransferAccount Reducers
 */

export const getTransferAccountState = createFeatureSelector<fromTransferAccount.State>('transferAccount');
export const getTransferAccountLoading = createSelector(getTransferAccountState, state => state.loading);
export const getTransferAccountSearchResults = createSelector(getTransferAccountState, state => state.transferAccounts);
export const getTransferAccountSearchResultforValidate = createSelector(getTransferAccountState, state => state.transferAccounts);
export const getTransferAccountSearchQuery = createSelector(getTransferAccountState, state => state.query);
export const getTransferAccountSearchError = createSelector(getTransferAccountState, state => state.error == undefined ? undefined : state.error.searchError);
export const getTransferAccountCreateError = createSelector(getTransferAccountState, state => state.error == undefined ? undefined : state.error.createTransferAccount);
export const getTransferAccountCreateSuccess = createSelector(getTransferAccountState, getTransferAccountLoading, (a, b) => a === undefined && b === false);
export const getTransferAccountEditError = createSelector(getTransferAccountState, state => state.error == undefined ? undefined : state.error.editTransferAccount);
export const getTransferAccountEditSuccess = createSelector(getTransferAccountState, state => state.transferAccountUpdateSuccess);
export const getTransferAccountDetails = createSelector(getTransferAccountState, state => state.transferAccountDetails);
export const getTransferAccountDetailError = createSelector(getTransferAccountState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getTransferAccountDetailLoading = createSelector(getTransferAccountState, state => state.getDetailLoading);
export const getTransferAccountCreated = createSelector(getTransferAccountState, state => state.transferAccountCreated);
export const getAllTransferAccounts = createSelector(getTransferAccountState, state => state.allTransferAccounts);
export const getAllTransferAccountError = createSelector(getTransferAccountState, state => state.error == undefined ? undefined : state.error.getAll)
export const getTransferAccountDeleteSuccess = createSelector(getTransferAccountState, state => state.transferAccountDeleteSuccess);
export const getTransferAccountDeleteError = createSelector(getTransferAccountState, state => state.error == undefined ? undefined : state.error.deleteError);


/*
 * Bank Statement Reducers
 */

export const getBankStatementState = createFeatureSelector<fromBankStatement.State>('bankStatement');
export const getBankStatementLoading = createSelector(getBankStatementState, state => state.loading);
export const getBankStatementSearchResults = createSelector(getBankStatementState, state => state.bankStatements);
export const getBankStatementSearchError = createSelector(getBankStatementState, state => state.error == undefined ? undefined : state.error.searchError);

/*
 * TenantUrlConfig Reducers
 */

export const getTenantUrlConfigState = createFeatureSelector<fromTenantUrlConfig.State>('tenantUrlConfig');
export const getTenantUrlConfigLoading = createSelector(getTenantUrlConfigState, state => state.loading);
export const getTenantUrlConfigCreateError = createSelector(getTenantUrlConfigState, state => state.error == undefined ? undefined : state.error.createTenantUrlConfig);
export const getTenantUrlConfigCreateSuccess = createSelector(getTenantUrlConfigState, getTenantUrlConfigLoading, (a, b) => a === undefined && b === false);
export const getTenantUrlConfigEditError = createSelector(getTenantUrlConfigState, state => state.error == undefined ? undefined : state.error.editTenantUrlConfig);
export const getTenantUrlConfigEditSuccess = createSelector(getTenantUrlConfigState, state => state.tenantUrlConfigUpdateSuccess);
export const getTenantUrlConfigDetails = createSelector(getTenantUrlConfigState, state => state.tenantUrlConfigDetails);
export const getTenantUrlConfigDetailError = createSelector(getTenantUrlConfigState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getTenantUrlConfigDetailLoading = createSelector(getTenantUrlConfigState, state => state.getDetailLoading);
export const getTenantUrlConfigCreated = createSelector(getTenantUrlConfigState, state => state.tenantUrlConfigCreated);
export const getAllTenantUrlConfigs = createSelector(getTenantUrlConfigState, state => state.allTenantUrlConfigs);
export const getAllTenantUrlConfigError = createSelector(getTenantUrlConfigState, state => state.error == undefined ? undefined : state.error.getAll)

/*
 * AutoTransfer Reducers
 */

export const getAutoTransferState = createFeatureSelector<fromAutoTransfer.State>('autoTransfer');
export const getAutoTransferLoading = createSelector(getAutoTransferState, state => state.loading);
export const getAutoTransferSearchResults = createSelector(getAutoTransferState, state => state.autoTransfers);
export const getAutoTransferSearchError = createSelector(getAutoTransferState, state => state.error == undefined ? undefined : state.error.searchError);

/*
 * MerchantCustomer Reducers
 */

export const getMerchantCustomerState = createFeatureSelector<fromMerchantCustomer.State>('merchantCustomer');
export const getMerchantCustomerLoading = createSelector(getMerchantCustomerState, state => state.loading);
export const getMerchantCustomerSearchResults = createSelector(getMerchantCustomerState, state => state.items);
export const getMerchantCustomerSearchError = createSelector(getMerchantCustomerState, state => state.error == undefined ? undefined : state.error.searchError);

export const getMerchantCustomerUpdateSuccess = createSelector(getMerchantCustomerState, state => state.updateSuccess);
export const getMerchantCustomerUpdateError = createSelector(getMerchantCustomerState, state => state.error == undefined ? undefined : state.error.createError);

export const getMerchantCustomerDetailLoading = createSelector(getMerchantCustomerState, state => state.detailLoading);
export const getMerchantCustomerDetail = createSelector(getMerchantCustomerState, state => state.detail);
export const getMerchantCustomerDetailError = createSelector(getMerchantCustomerState, state => state.error == undefined ? undefined : state.error.detailError);

export const getPhoneNumberRelatedCustomers = createSelector(getMerchantCustomerState, state => state.phoneNumberRelateds);
export const getPhoneNumberRelatedCustomerError = createSelector(getMerchantCustomerState, state => state.error == undefined ? undefined : state.error.phoneNumberRelated);

export const getMerchantCustomerCards = createSelector(getMerchantCustomerState, state => state.cardNumbers);
export const getMerchantCustomerCardsError = createSelector(getMerchantCustomerState, state => state.error == undefined ? undefined : state.error.cardNumbers);

export const downloadPhoneNumbers = createSelector(getMerchantCustomerState, state => state.downloadPhoneNumbers);
export const downloadPhoneNumbersError = createSelector(getMerchantCustomerState, state => state.error == undefined ? undefined : state.error.downloadPhoneNumbersError);

export const getRegisteredPhoneNumbers = createSelector(getMerchantCustomerState, state => state.registeredPhoneNumbers );
export const getRegisteredPhoneNumbersLoading = createSelector(getMerchantCustomerState, state => state.registeredPhoneNumbersLoading);

export const removeRegisteredPhoneNumbersSuccess = createSelector(getMerchantCustomerState, state => state.removeRegisteredPhonesSuccess);
export const removeRegisteredPhoneNumbersLoading = createSelector(getMerchantCustomerState, state => state.removeRegisteredPhonesLoading);
export const removeRegisteredPhoneNumbersError = createSelector(getMerchantCustomerState, state => state.error == undefined ? undefined : state.error.removeRegisteredPhonesError);



/*
 * UserSegmentGroup Reducers
 */

export const getUserSegmentGroupState = createFeatureSelector<fromUserSegmentGroup.State>('userSegmentGroup');
export const getUserSegmentGroupLoading = createSelector(getUserSegmentGroupState, state => state.loading);
export const getUserSegmentGroupCreateError = createSelector(getUserSegmentGroupState, state => state.error == undefined ? undefined : state.error.createUserSegmentGroup);
export const getUserSegmentGroupCreateSuccess = createSelector(getUserSegmentGroupState, getUserSegmentGroupLoading, (a, b) => a === undefined && b === false);
export const getUserSegmentGroupEditError = createSelector(getUserSegmentGroupState, state => state.error == undefined ? undefined : state.error.editUserSegmentGroup);
export const getUserSegmentGroupEditSuccess = createSelector(getUserSegmentGroupState, state => state.userSegmentGroupUpdateSuccess);
export const getUserSegmentGroupDetails = createSelector(getUserSegmentGroupState, state => state.userSegmentGroupDetails);
export const getUserSegmentGroupDetailError = createSelector(getUserSegmentGroupState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getUserSegmentGroupDetailLoading = createSelector(getUserSegmentGroupState, state => state.getDetailLoading);
export const getUserSegmentGroupCreated = createSelector(getUserSegmentGroupState, state => state.userSegmentGroupCreated);
export const getAllUserSegmentGroups = createSelector(getUserSegmentGroupState, state => state.allUserSegmentGroups);
export const getAllUserSegmentGroupError = createSelector(getUserSegmentGroupState, state => state.error == undefined ? undefined : state.error.getAll);
export const getUserSegmentGroupDeleteSuccess = createSelector(getUserSegmentGroupState, state => state.deleteSuccess);
export const getUserSegmentGroupDeleteError = createSelector(getUserSegmentGroupState, state => state.error == undefined ? undefined : state.error.deleteError);

/*
 * Application Settings Reducers
 */

export const getApplicationSettingsState = createFeatureSelector<fromApplicationSettings.State>('applicationSettings');
export const getApplicationSettingsLoading = createSelector(getApplicationSettingsState, state => state.loading);
export const getApplicationSettingsUpdateSuccess = createSelector(getApplicationSettingsState, state => state.applicationSettingsUpdateSuccess);
export const getApplicationSettingsCreated = createSelector(getApplicationSettingsState, state => state.applicationSettingsCreated);
export const getApplicationSettingsUpdateError = createSelector(getApplicationSettingsState, state => state.error == undefined ? undefined : state.error.editApplicationSettings);
export const getApplicationSettingsLoadError = createSelector(getApplicationSettingsState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getApplicationSettingsDetail = createSelector(getApplicationSettingsState, state => state.applicationSettingsDetail);
export const getTransferStatuses = createSelector(getApplicationSettingsState, state => state.transferStatuses);
export const getTransferStatusesError = createSelector(getApplicationSettingsState, state => state.error == undefined ? undefined : state.error.transferStatuses);
export const getOwnerSetting = createSelector(getApplicationSettingsState, state => state.ownerSetting);
export const getOwnerSettingError = createSelector(getApplicationSettingsState, state => state.error == undefined ? undefined : state.error.ownerSetting);
export const getOwnerSettingUpdated = createSelector(getApplicationSettingsState, state => state.ownerSettingUpdated);

/*
 * Risky Keywords List Reducers
 */

export const getRiskyKeywordState = createFeatureSelector<fromRiskyKeyword.State>('riskyKeyword');
export const getRiskyKeywordsLoading = createSelector(getRiskyKeywordState, state => state.loading);
export const getRiskyKeywordsDetail = createSelector(getRiskyKeywordState, state => state.riskyKeywordDetail);
export const getRiskyKeywordUpdateSuccess = createSelector(getRiskyKeywordState, state => state.riskyKeywordUpdateSuccess);
export const getRiskyKeywordLoadError = createSelector(getRiskyKeywordState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getRiskyKeywordUpdateError = createSelector(getRiskyKeywordState, state => state.error == undefined ? undefined : state.error.updateError);

/*
 * ManualTransfer Reducers
 */

export const getManualTransferState = createFeatureSelector<fromManualTransfer.State>('manualTransfer');
export const getManualTransferLoading = createSelector(getManualTransferState, state => state.loading);
export const getManualTransferSearchResults = createSelector(getManualTransferState, state => state.manualTransfers);
export const getManualTransferSearchResultforValidate = createSelector(getManualTransferState, state => state.manualTransfers);
export const getManualTransferSearchError = createSelector(getManualTransferState, state => state.error == undefined ? undefined : state.error.searchError);
export const getManualTransferCreateError = createSelector(getManualTransferState, state => state.error == undefined ? undefined : state.error.createManualTransfer);
export const getManualTransferCreateSuccess = createSelector(getManualTransferState, state => state.manualTransferCreateSuccess);
export const getManualTransferEditError = createSelector(getManualTransferState, state => state.error == undefined ? undefined : state.error.editManualTransfer);
export const getManualTransferEditSuccess = createSelector(getManualTransferState, state => state.manualTransferUpdateSuccess);
export const getManualTransferDetails = createSelector(getManualTransferState, state => state.manualTransferDetails);
export const getManualTransferDetailError = createSelector(getManualTransferState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getManualTransferDetailLoading = createSelector(getManualTransferState, state => state.getDetailLoading);
export const getManualTransferCreated = createSelector(getManualTransferState, state => state.manualTransferCreated);
export const getManualTransferDeleteSuccess = createSelector(getManualTransferState, state => state.manualTransferDeleteSuccess);
export const getManualTransferDeleteError = createSelector(getManualTransferState, state => state.error == undefined ? undefined : state.error.deleteError);
export const getManualTransferCancelSuccess = createSelector(getManualTransferState, state => state.manualTransferCancelSuccess);
export const getManualTransferCancelError = createSelector(getManualTransferState, state => state.error == undefined ? undefined : state.error.cancelError);
export const getManualTransferReceipt = createSelector(getManualTransferState, state => state.receipt);
export const getManualTransferReceiptError = createSelector(getManualTransferState, state => state.error == undefined ? undefined : state.error.receipt);
export const getManualTransferCancelDetailSuccess = createSelector(getManualTransferState, state => state.manualTransferDetailCancelSuccess);
export const getManualTransferCancelDetailError = createSelector(getManualTransferState, state => state.error == undefined ? undefined : state.error.cancelDetailError);
export const getManualTransferRetryDetailSuccess = createSelector(getManualTransferState, state => state.manualTransferDetailRetrySuccess);
export const getManualTransferRetryDetailError = createSelector(getManualTransferState, state => state.error == undefined ? undefined : state.error.retryDetailError);
export const getManualTransferSetAsCompletedDetailSuccess = createSelector(getManualTransferState, state => state.manualTransferDetailSetAsCompletedSuccess);
export const getManualTransferSetAsCompletedDetailError = createSelector(getManualTransferState, state => state.error == undefined ? undefined : state.error.setAsCompletedError);

/*
 * MobileTransferDevice Reducers
 */

export const getMobileTransferDeviceState = createFeatureSelector<fromMobileTransferDevice.State>('mobileTransferDevice');
export const getMobileTransferDeviceLoading = createSelector(getMobileTransferDeviceState, state => state.loading);
export const getMobileTransferDeviceSearchResults = createSelector(getMobileTransferDeviceState, state => state.allMobileTransferDevices);
export const getMobileTransferDeviceSearchResultforValidate = createSelector(getMobileTransferDeviceState, state => state.allMobileTransferDevices);
export const getMobileTransferDeviceSearchQuery = createSelector(getMobileTransferDeviceState, state => state.query);
export const getMobileTransferDeviceSearchError = createSelector(getMobileTransferDeviceState, state => state.error == undefined ? undefined : state.error.searchError);
export const getMobileTransferDeviceCreateError = createSelector(getMobileTransferDeviceState, state => state.error == undefined ? undefined : state.error.createMobileTransferDevice);
export const getMobileTransferDeviceCreateSuccess = createSelector(getMobileTransferDeviceState, getMobileTransferDeviceLoading, (a, b) => a === undefined && b === false);
export const getMobileTransferDeviceEditError = createSelector(getMobileTransferDeviceState, state => state.error == undefined ? undefined : state.error.editMobileTransferDevice);
export const getMobileTransferDeviceEditSuccess = createSelector(getMobileTransferDeviceState, state => state.mobileTransferDeviceUpdateSuccess);
export const getMobileTransferDeviceDetails = createSelector(getMobileTransferDeviceState, state => state.mobileTransferDeviceDetails);
export const getMobileTransferDeviceDetailError = createSelector(getMobileTransferDeviceState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getMobileTransferDeviceDetailLoading = createSelector(getMobileTransferDeviceState, state => state.getDetailLoading);
export const getMobileTransferDeviceCreated = createSelector(getMobileTransferDeviceState, state => state.mobileTransferDeviceCreated);
export const getAllMobileTransferDevices = createSelector(getMobileTransferDeviceState, state => state.allMobileTransferDevices);
export const getAllMobileTransferDeviceError = createSelector(getMobileTransferDeviceState, state => state.error == undefined ? undefined : state.error.getAll)
export const getMobileTransferDeviceSendSmsError = createSelector(getMobileTransferDeviceState, state => state.error == undefined ? undefined : state.error.sendSmsError);
export const getMobileTransferDeviceSendSmsSuccess = createSelector(getMobileTransferDeviceState, state => state.sendSmsCompleted);
export const getMobileTransferDeviceActivateError = createSelector(getMobileTransferDeviceState, state => state.error == undefined ? undefined : state.error.activateError);
export const getMobileTransferDeviceActivateSucces = createSelector(getMobileTransferDeviceState, state => state.activateCompleted);
export const getMobileTransferDeviceCheckStatusError = createSelector(getMobileTransferDeviceState, state => state.error == undefined ? undefined : state.error.checkStatusError);
export const getMobileTransferDeviceCheckStatusComplete = createSelector(getMobileTransferDeviceState, state => state.checkStatusCompleted);
export const getMobileTransferDeviceRemoveError = createSelector(getMobileTransferDeviceState, state => state.error == undefined ? undefined : state.error.removeError);
export const getMobileTransferDeviceRemoveComplete = createSelector(getMobileTransferDeviceState, state => state.removeCompleted);

/*
 * MobileTransferCardAccount Reducers
 */

export const getMobileTransferCardAccountState = createFeatureSelector<fromMobileTransferCardAccount.State>('mobileTransferCardAccount');
export const getMobileTransferCardAccountLoading = createSelector(getMobileTransferCardAccountState, state => state.loading);
export const getMobileTransferCardAccountCreateError = createSelector(getMobileTransferCardAccountState, state => state.error == undefined ? undefined : state.error.createMobileTransferCardAccount);
export const getMobileTransferCardAccountCreateSuccess = createSelector(getMobileTransferCardAccountState, getMobileTransferCardAccountLoading, (a, b) => a === undefined && b === false);
export const getMobileTransferCardAccountEditError = createSelector(getMobileTransferCardAccountState, state => state.error == undefined ? undefined : state.error.editMobileTransferCardAccount);
export const getMobileTransferCardAccountEditSuccess = createSelector(getMobileTransferCardAccountState, state => state.mobileTransferCardAccountUpdateSuccess);
export const getMobileTransferCardAccountDetails = createSelector(getMobileTransferCardAccountState, state => state.mobileTransferCardAccountDetails);
export const getMobileTransferCardAccountDetailError = createSelector(getMobileTransferCardAccountState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getMobileTransferCardAccountDetailLoading = createSelector(getMobileTransferCardAccountState, state => state.getDetailLoading);
export const getMobileTransferCardAccountCreated = createSelector(getMobileTransferCardAccountState, state => state.mobileTransferCardAccountCreated);
export const getAllMobileTransferCardAccounts = createSelector(getMobileTransferCardAccountState, state => state.allMobileTransferCardAccounts);
export const getAllMobileTransferCardAccountError = createSelector(getMobileTransferCardAccountState, state => state.error == undefined ? undefined : state.error.getAll)

/*
 * MobileTransferCardAccountGroup Reducers
 */

export const getMobileTransferCardAccountGroupState = createFeatureSelector<fromMobileTransferCardAccountGroup.State>('mobileTransferCardAccountGroup');
export const getMobileTransferCardAccountGroupLoading = createSelector(getMobileTransferCardAccountGroupState, state => state.loading);
export const getMobileTransferCardAccountGroupCreateError = createSelector(getMobileTransferCardAccountGroupState, state => state.error == undefined ? undefined : state.error.createMobileTransferCardAccountGroup);
export const getMobileTransferCardAccountGroupCreateSuccess = createSelector(getMobileTransferCardAccountGroupState, getMobileTransferCardAccountGroupLoading, (a, b) => a === undefined && b === false);
export const getMobileTransferCardAccountGroupEditError = createSelector(getMobileTransferCardAccountGroupState, state => state.error == undefined ? undefined : state.error.editMobileTransferCardAccountGroup);
export const getMobileTransferCardAccountGroupEditSuccess = createSelector(getMobileTransferCardAccountGroupState, state => state.mobileTransferCardAccountGroupUpdateSuccess);
export const getMobileTransferCardAccountGroupDetails = createSelector(getMobileTransferCardAccountGroupState, state => state.mobileTransferCardAccountGroupDetails);
export const getMobileTransferCardAccountGroupDetailError = createSelector(getMobileTransferCardAccountGroupState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getMobileTransferCardAccountGroupDetailLoading = createSelector(getMobileTransferCardAccountGroupState, state => state.getDetailLoading);
export const getMobileTransferCardAccountGroupCreated = createSelector(getMobileTransferCardAccountGroupState, state => state.mobileTransferCardAccountGroupCreated);
export const getAllMobileTransferCardAccountGroups = createSelector(getMobileTransferCardAccountGroupState, state => state.allMobileTransferCardAccountGroups);
export const getAllMobileTransferCardAccountGroupError = createSelector(getMobileTransferCardAccountGroupState, state => state.error == undefined ? undefined : state.error.getAll);
export const getMobileTransferCardAccountGroupDeleteSuccess = createSelector(getMobileTransferCardAccountGroupState, state => state.cardToCardAccountDeletedSuccess);
export const getMobileTransferCardAccountGroupDeleteError = createSelector(getMobileTransferCardAccountGroupState, state => state.error == undefined ? undefined : state.error.deleteError);

/*
 * BlockedPhoneNumber Reducers
 */

export const getBlockedPhoneNumberState = createFeatureSelector<fromBlockedPhoneNumber.State>('blockedPhoneNumber');
export const getBlockedPhoneNumberLoading = createSelector(getBlockedPhoneNumberState, state => state.loading);
export const getBlockedPhoneNumberCreateError = createSelector(getBlockedPhoneNumberState, state => state.error == undefined ? undefined : state.error.createBlockedPhoneNumber);
export const getBlockedPhoneNumberCreateSuccess = createSelector(getBlockedPhoneNumberState, getBlockedPhoneNumberLoading, (a, b) => a === undefined && b === false);
export const getBlockedPhoneNumberEditError = createSelector(getBlockedPhoneNumberState, state => state.error == undefined ? undefined : state.error.editBlockedPhoneNumber);
export const getBlockedPhoneNumberEditSuccess = createSelector(getBlockedPhoneNumberState, state => state.blockedPhoneNumberUpdateSuccess);
export const getBlockedPhoneNumberDetails = createSelector(getBlockedPhoneNumberState, state => state.blockedPhoneNumberDetails);
export const getBlockedPhoneNumberDetailError = createSelector(getBlockedPhoneNumberState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getBlockedPhoneNumberDetailLoading = createSelector(getBlockedPhoneNumberState, state => state.getDetailLoading);
export const getBlockedPhoneNumberCreated = createSelector(getBlockedPhoneNumberState, state => state.blockedPhoneNumberCreated);
export const getAllBlockedPhoneNumbers = createSelector(getBlockedPhoneNumberState, state => state.allBlockedPhoneNumbers);
export const getAllBlockedPhoneNumberError = createSelector(getBlockedPhoneNumberState, state => state.error == undefined ? undefined : state.error.getAll);
export const getBlockedPhoneNumberDeleteSuccess = createSelector(getBlockedPhoneNumberState, state => state.cardToCardAccountDeletedSuccess);
export const getBlockedPhoneNumberDeleteError = createSelector(getBlockedPhoneNumberState, state => state.error == undefined ? undefined : state.error.deleteError);

/*
 * CardHolder Reducers
 */

export const getCardHolderState = createFeatureSelector<fromCardHolder.State>('cardHolder');
export const getCardHolderLoading = createSelector(getCardHolderState, state => state.loading);
export const getCardHolderSearchResult = createSelector(getCardHolderState, state => state.cardHolder);
export const getCardHolderSearchError = createSelector(getCardHolderState, state => state.error == undefined ? undefined : state.error.searchError);

/*
 * Report Reducers
 */

export const getReportState = createFeatureSelector<fromReport.State>('report');
export const getReportUserSegmentReport = createSelector(getReportState, state => state.userSegmentReportItems);
export const getReportUserSegmentReportLoading = createSelector(getReportState, state => state.userSegmentReportLoading);
export const getReportUserSegmentReportError = createSelector(getReportState, state => state.error == undefined ? undefined : state.error.userSegmentReport);
export const getReportTenantBalance = createSelector(getReportState, state => state.tenantBalances);
export const getReportTenantBalanceLoading = createSelector(getReportState, state => state.tenantBalanceLoading);
export const getReportTenantBalanceError = createSelector(getReportState, state => state.error == undefined ? undefined : state.error.tenantBalance);
export const getReportDepositWithdrawalWidget = createSelector(getReportState, state => state.depositWithdrawalWidget);
export const getReportDepositWithdrawalLoading = createSelector(getReportState, state => state.depositWithdrawalLoading);
export const getReportDepositWithdrawalError = createSelector(getReportState, state => state.error == undefined ? undefined : state.error.depositWithdrawalWidget);
export const getReportWithdrawalPaymentWidget = createSelector(getReportState, state => state.withdrawalPaymentWidget);
export const getReportWithdrawalPaymentLoading = createSelector(getReportState, state => state.withdrawalPaymentLoading);
export const getReportWithdrwalPaymentError = createSelector(getReportState, state => state.error == undefined ? undefined : state.error.withdrawalPayment);
export const getReportDepositByAccountNumberWidget = createSelector(getReportState, state => state.depositByAccountNumberWidget);
export const getReportDepositByAccountNumberLoading = createSelector(getReportState, state => state.depositByAccountNumberLoading);
export const getReportDepositByAccountNumberError = createSelector(getReportState, state => state.error == undefined ? undefined : state.error.depositByAccountNumberWidget);

export const getReportDepositBreakDownListByPaymentType = createSelector(getReportState, state => state.depositBreakDownList);
export const getReportDepositBreakDownListByPaymentTypeLoading = createSelector(getReportState, state => state.depositBreakDownListLoading);
export const getReportDepositBreakDownListByPaymentTypeError = createSelector(getReportState, state => state.error == undefined ? undefined : state.error.depositBreakDownList);

/*
 * InvoiceOwnerSetting Reducers
 */

export const getInvoiceOwnerSettingState = createFeatureSelector<fromInvoiceOwnerSetting.State>('invoiceOwnerSetting');
export const getInvoiceOwnerSettingLoading = createSelector(getInvoiceOwnerSettingState, state => state.loading);
export const getInvoiceOwnerSettingCreateError = createSelector(getInvoiceOwnerSettingState, state => state.error == undefined ? undefined : state.error.createInvoiceOwnerSetting);
export const getInvoiceOwnerSettingCreateSuccess = createSelector(getInvoiceOwnerSettingState, getInvoiceOwnerSettingLoading, (a, b) => a === undefined && b === false);
export const getInvoiceOwnerSettingEditError = createSelector(getInvoiceOwnerSettingState, state => state.error == undefined ? undefined : state.error.editInvoiceOwnerSetting);
export const getInvoiceOwnerSettingEditSuccess = createSelector(getInvoiceOwnerSettingState, state => state.invoiceOwnerSettingUpdateSuccess);
export const getInvoiceOwnerSettingDetails = createSelector(getInvoiceOwnerSettingState, state => state.invoiceOwnerSettingDetails);
export const getInvoiceOwnerSettingDetailError = createSelector(getInvoiceOwnerSettingState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getInvoiceOwnerSettingDetailLoading = createSelector(getInvoiceOwnerSettingState, state => state.getDetailLoading);
export const getInvoiceOwnerSettingCreated = createSelector(getInvoiceOwnerSettingState, state => state.invoiceOwnerSettingCreated);
export const getAllInvoiceOwnerSettings = createSelector(getInvoiceOwnerSettingState, state => state.allInvoiceOwnerSettings);
export const getAllInvoiceOwnerSettingError = createSelector(getInvoiceOwnerSettingState, state => state.error == undefined ? undefined : state.error.getAll);

/*
 * Invoice Reducers
 */

export const getInvoiceState = createFeatureSelector<fromInvoice.State>('invoice');
export const getInvoiceLoading = createSelector(getInvoiceState, state => state.loading);
export const getInvoiceDetailLoading = createSelector(getInvoiceState, state => state.detailLoading);
export const getInvoiceSearchResults = createSelector(getInvoiceState, state => state.invoices);
export const getInvoiceSearchResultError = createSelector(getInvoiceState, state => state.error == undefined ? undefined : state.error.searchError);
export const getInvoiceDetail = createSelector(getInvoiceState, state => state.invoiceDetail);
export const getInvoiceDetailError = createSelector(getInvoiceState, state => state.error == undefined ? undefined : state.error.getDetails);

/*
 * InvoicePayment Reducers
 */

export const getInvoicePaymentState = createFeatureSelector<fromInvoicePayment.State>('invoicePayment');
export const getInvoicePaymentLoading = createSelector(getInvoicePaymentState, state => state.loading);
export const getInvoicePaymentCreateError = createSelector(getInvoicePaymentState, state => state.error == undefined ? undefined : state.error.createInvoicePayment);
export const getInvoicePaymentCreateSuccess = createSelector(getInvoicePaymentState, getInvoicePaymentLoading, (a, b) => a === undefined && b === false);
export const getInvoicePaymentEditError = createSelector(getInvoicePaymentState, state => state.error == undefined ? undefined : state.error.editInvoicePayment);
export const getInvoicePaymentEditSuccess = createSelector(getInvoicePaymentState, state => state.invoicePaymentUpdateSuccess);
export const getInvoicePaymentDetails = createSelector(getInvoicePaymentState, state => state.invoicePaymentDetails);
export const getInvoicePaymentDetailError = createSelector(getInvoicePaymentState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getInvoicePaymentDetailLoading = createSelector(getInvoicePaymentState, state => state.getDetailLoading);
export const getInvoicePaymentCreated = createSelector(getInvoicePaymentState, state => state.invoicePaymentCreated);
export const getAllInvoicePayments = createSelector(getInvoicePaymentState, state => state.allInvoicePayments);
export const getAllInvoicePaymentError = createSelector(getInvoicePaymentState, state => state.error == undefined ? undefined : state.error.getAll);

/*
 * BlockedCardNumber Reducers
 */

export const getBlockedCardNumberState = createFeatureSelector<fromBlockedCardNumber.State>('blockedCardNumber');
export const getBlockedCardNumberLoading = createSelector(getBlockedCardNumberState, state => state.loading);
export const getBlockedCardNumberCreateError = createSelector(getBlockedCardNumberState, state => state.error == undefined ? undefined : state.error.createBlockedCardNumber);
export const getBlockedCardNumberCreateSuccess = createSelector(getBlockedCardNumberState, getBlockedCardNumberLoading, (a, b) => a === undefined && b === false);
export const getBlockedCardNumberEditError = createSelector(getBlockedCardNumberState, state => state.error == undefined ? undefined : state.error.editBlockedCardNumber);
export const getBlockedCardNumberEditSuccess = createSelector(getBlockedCardNumberState, state => state.blockedCardNumberUpdateSuccess);
export const getBlockedCardNumberDetails = createSelector(getBlockedCardNumberState, state => state.blockedCardNumberDetails);
export const getBlockedCardNumberDetailError = createSelector(getBlockedCardNumberState, state => state.error == undefined ? undefined : state.error.getDetails);
export const getBlockedCardNumberDetailLoading = createSelector(getBlockedCardNumberState, state => state.getDetailLoading);
export const getBlockedCardNumberCreated = createSelector(getBlockedCardNumberState, state => state.blockedCardNumberCreated);
export const getAllBlockedCardNumbers = createSelector(getBlockedCardNumberState, state => state.allBlockedCardNumbers);
export const getAllBlockedCardNumberError = createSelector(getBlockedCardNumberState, state => state.error == undefined ? undefined : state.error.getAll);
export const getBlockedCardNumberDeleteSuccess = createSelector(getBlockedCardNumberState, state => state.cardToCardAccountDeletedSuccess);
export const getBlockedCardNumberDeleteError = createSelector(getBlockedCardNumberState, state => state.error == undefined ? undefined : state.error.deleteError);


 /**
 * Alerts Reducers
 */
  export const getNotificationState = createFeatureSelector<fromNotification.NotificationState>('notification');
  export const getLatestNotification = createSelector(getNotificationState, state => 
      state && state.messages && state.messageCount > 0 ? state.messages[state.messageCount - 1] : undefined);
  export const getLatestNotificationCount = createSelector(getNotificationState, state => state.messageCount);
  