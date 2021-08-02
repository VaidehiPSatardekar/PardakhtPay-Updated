import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './main/pages/authentication/login/login.component';
import { AuthGuard } from 'app/guards/auth.guard';
import { MerchantCreateComponent } from './components/merchant-management/merchant-create/merchant-create.component';
import { MerchantListComponent } from './components/merchant-management/merchant-list/merchant-list.component';
import { TransactionListComponent } from './components/transaction-list/transaction-list.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AccountingListComponent } from './components/accounting-list/accounting-list.component';
import { WithdrawalListComponent } from './components/withdrawal-list/withdrawal-list.component';
import { CardToCardAccountListComponent } from './components/card-to-card-account-list/card-to-card-account-list.component';
import { CardToCardAccountComponent } from './components/card-to-card-account/card-to-card-account.component';
import { TransferAccountListComponent } from './components/transfer-account-list/transfer-account-list.component';
import { TransferAccountComponent } from './components/transfer-account/transfer-account.component';
import { BankLoginListComponent } from './components/bank-login-list/bank-login-list.component';
import { BankLoginComponent } from './components/bank-login/bank-login.component';
import { BankLoginAccountComponent } from './components/bank-login-account/bank-login-account.component';
import { BankLoginChangePasswordComponent } from './components/bank-login-change-password/bank-login-change-password.component';
import { BankStatementListComponent } from './components/bank-statement-list/bank-statement-list.component';
import { PermissionGuard } from './guards/permission.guard';
import { PermissionRouteGuard } from './guards/permission-route.guard';
import { TenantUrlConfigListComponent } from './components/tenant-url-config-list/tenant-url-config-list.component';
import { TenantUrlConfigComponent } from './components/tenant-url-config/tenant-url-config.component';
import { AutoTransferListComponent } from './components/auto-transfer-list/auto-transfer-list.component';
import { CardToCardAccountGroupListComponent } from './components/card-to-card-account-group-list/card-to-card-account-group-list.component';
import { CardToCardAccountGroupComponent } from './components/card-to-card-account-group/card-to-card-account-group.component';
import { MerchantCustomerListComponent } from './components/merchant-customer-list/merchant-customer-list.component';
import { UserSegmentGroupListComponent } from './components/user-segment-group-list/user-segment-group-list.component';
import { UserSegmentGroupComponent } from './components/user-segment-group/user-segment-group.component';
import { MerchantCustomerComponent } from './components/merchant-customer/merchant-customer.component';
import { ApplicationSettingsComponent } from './components/application-settings/application-settings.component';
import { RiskyKeywordComponent } from './components/risky-keyword/risky-keyword.component';
import { ManualTransferListComponent } from './components/manual-transfer-list/manual-transfer-list.component';
import { CreateManualTransferComponent } from './components/create-manual-transfer/create-manual-transfer.component';
import { EditManualTransferComponent } from './components/edit-manual-transfer/edit-manual-transfer.component';
import * as permissions from './models/permissions';
import { MobileTransferDeviceListComponent } from './components/mobile-transfer-device-list/mobile-transfer-device-list.component';
import { MobileTransferDeviceComponent } from './components/mobile-transfer-device/mobile-transfer-device.component';
import { MobileTransferAccountListComponent } from './components/mobile-transfer-account-list/mobile-transfer-account-list.component';
import { MobileTransferCardAccountComponent } from './components/mobile-transfer-card-account/mobile-transfer-card-account.component';
import { MobileTransferAccountGroupListComponent } from './components/mobile-transfer-account-group-list/mobile-transfer-account-group-list.component';
import { MobileTransferAccountGroupComponent } from './components/mobile-transfer-account-group/mobile-transfer-account-group.component';
import { BlockCardsComponent } from './components/block-cards/block-cards.component';
import { BlockedPhoneNumberListComponent } from './components/blocked-phone-number-list/blocked-phone-number-list.component';
import { BlockedPhoneNumberComponent } from './components/blocked-phone-number/blocked-phone-number.component';
import { WithdrawalDetailComponent } from './components/withdrawal-detail/withdrawal-detail.component';
import { CardHolderNameComponent } from './components/card-holder-name/card-holder-name.component';
import { UserSegmentReportComponent } from './components/user-segment-report/user-segment-report.component';
import { TenantBalanceComponent } from './components/tenant-balance/tenant-balance.component';
import { DepositWithdrawalChartComponent } from './components/deposit-withdrawal-chart/deposit-withdrawal-chart.component';
import { ReportsComponent } from './components/reports/reports.component';
import { WithdrawalPaymentChartComponent } from './components/withdrawal-payment-chart/withdrawal-payment-chart.component';
import { InvoiceOwnerSettingListComponent } from './components/invoice-owner-setting-list/invoice-owner-setting-list.component';
import { InvoiceOwnerSettingComponent } from './components/invoice-owner-setting/invoice-owner-setting.component';
import { InvoiceListComponent } from './components/invoice-list/invoice-list.component';
import { InvoicePaymentListComponent } from './components/invoice-payment-list/invoice-payment-list.component';
import { InvoicePaymentComponent } from './components/invoice-payment/invoice-payment.component';
import { OwnerSettingComponent } from './components/owner-setting/owner-setting.component';
import { QRCodeRegistrationComponent } from './components/qr-code-registration/qr-code-registration.component';
import { LoginDeviceStatusListComponent } from './components/login-device-status-list/login-device-status-list.component';
import { DepositByAccountNumberChartComponent } from './components/deposit-by-accountnumber-chart/deposit-by-accountnumber-chart.component';
import { BlockedCardNumberComponent } from './components/blocked-card-number/blocked-card-number.component';
import { BlockedCardNumberListComponent } from './components/blocked-card-number-list/blocked-card-number-list.component';
import { RemoveRegisteredPhoneNumbersComponent } from './components/remove-registered-phone/remove-registered-phone.component';
import { RolesComponent } from './components/roles/roles.component';
import { TenantStaffUsersComponent } from './components/tenant-staff-users/tenant-staff-users.component';
import { ProviderStaffListComponent } from './components/provider-staff-list/provider-staff-list.component';
import { Permissions } from './models/index';

const routes: Routes = [
    { path: 'login', component: LoginComponent },
    //{ path: 'changepassword', canActivate: [AuthGuard], component: ResetPasswordComponent },
    { path: 'merchants', component: MerchantListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMerchants } },
    { path: 'merchant/:id', component: MerchantCreateComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMerchants } },
    { path: 'merchant', component: MerchantCreateComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMerchants } },
    { path: 'transactions', component: TransactionListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListDeposits } },
    { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
    { path: 'accounting', component: AccountingListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListAccounting } },
    { path: 'withdrawals', component: WithdrawalListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListWithdrawals } },
    { path: 'withdrawaldetail/:id', component: WithdrawalDetailComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListWithdrawals } },
    { path: 'cardtocardaccounts', component: CardToCardAccountListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankAccounts } },
    { path: 'cardtocardaccount/:id', component: CardToCardAccountComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankAccounts } },
    { path: 'transferaccounts', component: TransferAccountListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListTransferAccounts } },
    { path: 'transferaccount', component: TransferAccountComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListTransferAccounts } },
    { path: 'transferaccount/:id', component: TransferAccountComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListTransferAccounts } },
    { path: 'banklogins', component: BankLoginListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankLogins } },
    { path: 'banklogin', component: BankLoginComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankLogins } },
    { path: 'banklogin/:id', component: BankLoginComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankLogins } },
    { path: 'bankloginaccount/:id', component: BankLoginAccountComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankLogins } },
    { path: 'changelogininformation/:id', component: BankLoginChangePasswordComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankLogins } },
    { path: 'statement', component: BankStatementListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankStatements } },
    { path: 'autotransfers', component: AutoTransferListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListAutoTransfers } },
    { path: 'tenanturlconfiglist', component: TenantUrlConfigListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListUrlConfiguration } },
    { path: 'tenanturlconfig', component: TenantUrlConfigComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListUrlConfiguration } },
    { path: 'tenanturlconfig/:id', component: TenantUrlConfigComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListUrlConfiguration } },
    { path: 'cardtocardaccountgroups', component: CardToCardAccountGroupListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankAccountGroups } },
    { path: 'cardtocardaccountgroup', component: CardToCardAccountGroupComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankAccountGroups } },
    { path: 'cardtocardaccountgroup/:id', component: CardToCardAccountGroupComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankAccountGroups } },
    { path: 'merchantcustomers', component: MerchantCustomerListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListCustomers } },
    { path: 'merchantcustomer/:id', component: MerchantCustomerComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListCustomers } },
    { path: 'usersegmentgroups', component: UserSegmentGroupListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListUserSegments } },
    { path: 'usersegmentgroup', component: UserSegmentGroupComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListUserSegments } },
    { path: 'usersegmentgroup/:id', component: UserSegmentGroupComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListUserSegments } },
    { path: 'applicationsettings', component: ApplicationSettingsComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListApplicationSettings } },
    { path: 'riskykeywords', component: RiskyKeywordComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListApplicationSettings } },
    { path: 'manualtransfers', component: ManualTransferListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListManualTransfers } },
    { path: 'newmanualtransfer', component: CreateManualTransferComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListManualTransfers } },
    { path: 'newmanualtransfer/:id', component: CreateManualTransferComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListManualTransfers } },
    { path: 'editmanualtransfer/:id', component: EditManualTransferComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListManualTransfers } },
    { path: 'mobiletransferdevices', component: MobileTransferDeviceListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMobileTransferDevices } },
    { path: 'mobiletransferdevice', component: MobileTransferDeviceComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMobileTransferDevices } },
    { path: 'mobiletransferdevice/:id', component: MobileTransferDeviceComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMobileTransferDevices } },
    { path: 'mobiletransfercardaccounts', component: MobileTransferAccountListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMobileTransferAccounts } },
    { path: 'mobiletransfercardaccount', component: MobileTransferCardAccountComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMobileTransferAccounts } },
    { path: 'mobiletransfercardaccount/:id', component: MobileTransferCardAccountComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMobileTransferAccounts } },
    { path: 'mobiletransferaccountgroups', component: MobileTransferAccountGroupListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMobileTransferGroups } },
    { path: 'mobiletransferaccountgroup', component: MobileTransferAccountGroupComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMobileTransferGroups } },
    { path: 'mobiletransferaccountgroup/:id', component: MobileTransferAccountGroupComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListMobileTransferGroups } },
    { path: 'blockedphonenumbers', component: BlockedPhoneNumberListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBlockedPhoneNumbers } },
    { path: 'blockedphonenumber/:id', component: BlockedPhoneNumberComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.EditBlockedPhoneNumber } },
    { path: 'blockedphonenumber', component: BlockedPhoneNumberComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.AddBlockedPhoneNumber } },
    { path: 'blockedcards', component: BlockCardsComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankLogins } },
    { path: 'tenantbalance', component: TenantBalanceComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.TenantCurrentBalance } },
    { path: 'segmentreport', component: UserSegmentReportComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.UserSegmentReport } },
    { path: 'depositwithdrawalchart', component: DepositWithdrawalChartComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.DepositWithdrawalReport } },
    { path: 'reports', component: ReportsComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ShowReports } },
    { path: 'withdrawalpaymentchart', component: WithdrawalPaymentChartComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.WithdrawalPayments } },
    { path: 'cardHolders', component: CardHolderNameComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListCardHolderNames } },
    { path: 'invoiceownersettings', component: InvoiceOwnerSettingListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListInvoiceOwnerSetting } },
    { path: 'invoiceownersetting', component: InvoiceOwnerSettingComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListInvoiceOwnerSetting } },
    { path: 'invoiceownersetting/:id', component: InvoiceOwnerSettingComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListInvoiceOwnerSetting } },
    { path: 'invoices', component: InvoiceListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListInvoices } },
    { path: 'invoicepayments', component: InvoicePaymentListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListInvoicePayments } },
    { path: 'invoicepayment', component: InvoicePaymentComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListInvoicePayments } },
    { path: 'invoicepayment/:id', component: InvoicePaymentComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListInvoicePayments } },
    { path: 'ownersetting', component: OwnerSettingComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.OwnerSettings } },
    { path: '', component: DashboardComponent, canActivate: [AuthGuard] },
    { path: '**', redirectTo: 'under-construction' },
    { path: 'banklogin/getqrregistrationdetails/:id', component: QRCodeRegistrationComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBankLogins } },
    { path: 'loginDeviceStatusList', component: LoginDeviceStatusListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.LoginDeviceStatusSettings } },
    { path: 'depositbyaccountnumberchart', component: DepositByAccountNumberChartComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.DepositWithdrawalReport } },
    { path: 'blockedcardnumbers', component: BlockedCardNumberListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListBlockedCardNumbers } },
    { path: 'blockedcardnumber/:id', component: BlockedCardNumberComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.AddBlockedCardNumber } },
    { path: 'blockedcardnumber', component: BlockedCardNumberComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.AddBlockedCardNumber } },
    { path: 'merchantcustomer/getregisteredphones/:id', component: RemoveRegisteredPhoneNumbersComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: permissions.ListCustomers } },
    { path: 'roles', component: RolesComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: Permissions.RoleAccess } },
    { path: 'tenant-staff-users', component: TenantStaffUsersComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: Permissions.TenantStaffUsers } },
    { path: 'provider', component: ProviderStaffListComponent, canActivate: [AuthGuard, PermissionRouteGuard], data: { permissionCode: Permissions.ProviderAccess } },

];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { useHash: false })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }

