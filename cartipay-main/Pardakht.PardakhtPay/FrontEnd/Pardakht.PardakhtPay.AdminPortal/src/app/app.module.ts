import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';
import { MatMomentDateModule } from '@angular/material-moment-adapter';
import { MatButtonModule, MatIconModule, MatSelectModule } from '@angular/material';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import 'hammerjs';
import { registerLocaleData, DecimalPipe, DatePipe } from '@angular/common';
import localeFa from '@angular/common/locales/fa';

import { FuseWidgetModule } from '../@fuse/components/widget/widget.module';

import { FuseSharedModule } from '../@fuse/shared.module';
import { FuseProgressBarModule } from '../@fuse/components/progress-bar/progress-bar.module';
import { FuseSidebarModule } from '../@fuse/components/sidebar/sidebar.module';
import { FuseThemeOptionsModule } from '../@fuse/components/theme-options/theme-options.module';
import { AgGridModule } from 'ag-grid-angular';

import { fuseConfig } from 'app/fuse-config';


import { AppComponent } from 'app/app.component';
import { LayoutModule } from 'app/layout/layout.module';
import { SampleModule } from 'app/main/sample/sample.module';
import { AppRoutingModule } from './app-routing.module';
import { PagesModule } from './main/pages/pages.module';
import { StoreModule } from '@ngrx/store';
import { reducers, metaReducers } from 'app/core';
import { StoreRouterConnectingModule } from '@ngrx/router-store';
import { environment } from 'environments/environment';
import { APP_INITIALIZER } from '@angular/core';
import { AccountService } from './core/services/account.service';
import { RouterStateSerializer } from '@ngrx/router-store';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { EffectsModule } from '@ngrx/effects';
import { AccountEffects } from './core/effects/account';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { AuthGuard } from './guards/auth.guard';
import { PermissionRouteGuard } from './guards/permission-route.guard';
import { PermissionGuard } from './guards/permission.guard';
import { TokenInterceptor } from './interceptors/token.interceptor';
import { JwtInterceptor } from './interceptors/jwt.interceptor';
import { LocaleInterceptor } from './interceptors/locale.interceptor';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { MerchantManagementComponent } from './components/merchant-management/merchant-management.component';
import { MerchantListComponent } from './components/merchant-management/merchant-list/merchant-list.component';
import { MerchantService } from './core/services/merchant/merchant.service';
import { MerchantEffects } from './core/effects/merchant';
import { MaterialModule } from './core/modules/material.module';
import { SharedModule } from './core/modules/shared.module';
import { MerchantCreateComponent } from './components/merchant-management/merchant-create/merchant-create.component';
import { TransactionListComponent } from './components/transaction-list/transaction-list.component';
import { TransactionService } from './core/services/transaction/transaction.service';
import { TransactionEffects } from './core/effects/transaction';
import { DashboardService } from './core/services/dashboard/dashboard.service';
import { DashboardEffects } from './core/effects/dashboard';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { LocalizedDatePipe } from './core/pipe/localized-date.pipe';
import { AccountingService } from './core/services/accounting/accounting.service';
import { AccountingListComponent } from './components/accounting-list/accounting-list.component';
import { AccountingEffects } from './core/effects/accounting';
import { RandomService } from './core/services/random/random.service';
import { WithdrawalEffects } from './core/effects/withdrawal';
import { WithdrawalComponent } from './components/withdrawal/withdrawal.component';
import { WithdrawalListComponent } from './components/withdrawal-list/withdrawal-list.component';
import { MerchantBankAccountEffects } from './core/effects/merchantBankAccount';
import { TenantListComponent } from './components/tenant-list/tenant-list.component';
import { TenantEffects } from './core/effects/tenant';
import { TenantComponent } from './components/tenant/tenant.component';
import { CardToCardAccountListComponent } from './components/card-to-card-account-list/card-to-card-account-list.component';
import { CardToCardAccountService } from './core/services/cardToCard/card-to-card.service';
import { CardToCardAccountEffects } from './core/effects/cardToCardAccount';
import { CardToCardAccountComponent } from './components/card-to-card-account/card-to-card-account.component';
import { BankLoginService } from './core/services/bankLogin/bank-login.service';
import { BankLoginEffects } from './core/effects/bankLogin';
import { TransferAccountService } from './core/services/transferAccount/transfer-account.service';
import { TransferAccountListComponent } from './components/transfer-account-list/transfer-account-list.component';
import { TransferAccountEffects } from './core/effects/transferAccount';
import { TransferAccountComponent } from './components/transfer-account/transfer-account.component';
import { TimeZoneService } from './core/services/timeZoneService/time-zone.service';
import { BankLoginComponent } from './components/bank-login/bank-login.component';
import { BankLoginListComponent } from './components/bank-login-list/bank-login-list.component';
import { BankLoginAccountComponent } from './components/bank-login-account/bank-login-account.component';
import { UsersComponent } from './components/users/users.component';
import { UserComponent } from './components/user/user.component';
import { BankLoginChangePasswordComponent } from './components/bank-login-change-password/bank-login-change-password.component';
import { BankStatementItemEffects } from './core/effects/bankStatement';
import { BankStatementListComponent } from './components/bank-statement-list/bank-statement-list.component';
import { TenantService } from './core/services/tenant/tenant.service';
import { TenantInterceptor } from './interceptors/tenant.interceptor';
import { TenantUrlConfigComponent } from './components/tenant-url-config/tenant-url-config.component';
import { TenantUrlConfigListComponent } from './components/tenant-url-config-list/tenant-url-config-list.component';
import { TenantUrlConfigEffects } from './core/effects/tenantUrlConfig';
import { AutoTransferListComponent } from './components/auto-transfer-list/auto-transfer-list.component';
import { AutoTransferEffects } from './core/effects/autoTransfer';
import { DeactivateLoginDialogComponent } from './components/deactivate-login-dialog/deactivate-login-dialog.component';
import { DeleteLoginDialogComponent } from './components/delete-login-dialog/delete-login-dialog.component';
import { BooleanFormatterComponent } from './components/formatters/booleanformatter';
import { DateFormatterComponent } from './components/formatters/dateformatter';
import { NumberFormatterComponent } from './components/formatters/numberformatter';
import { BooleanInverseFormatterComponent } from './components/formatters/booleaninverseformatter';
import { CardToCardAccountGroupListComponent } from './components/card-to-card-account-group-list/card-to-card-account-group-list.component';
import { CardToCardAccountGroupEffects } from './core/effects/cardToCardAccountGroup';
import { CardToCardAccountGroupComponent } from './components/card-to-card-account-group/card-to-card-account-group.component';
import { MerchantCustomerEffects } from './core/effects/merchantCustomer';
import { MerchantCustomerListComponent } from './components/merchant-customer-list/merchant-customer-list.component';
import { MerchantCustomerComponent } from './components/merchant-customer/merchant-customer.component';
import { UserSegmentGroupEffects } from './core/effects/userSegmentGroup';
import { UserSegmentGroupComponent } from './components/user-segment-group/user-segment-group.component';
import { UserSegmentGroupListComponent } from './components/user-segment-group-list/user-segment-group-list.component';
import { DownloadWithdrawalReceiptFormatterComponent } from './components/formatters/downloadWithdrawalReceiptFormatter';
import { DeleteButtonFormatterComponent } from './components/formatters/deletebuttonformatter';
import { UserEffects} from './core/effects/user.effects';
import { UserService} from './core/services/user.service';
import { ChangePasswordComponent} from './components/change-password/change-password.component';
import { ApplicationSettingsComponent } from './components/application-settings/application-settings.component'; 
import { ApplicationSettingsEffects } from './core/effects/applicationSettings';
import { IconButtonFormatterComponent } from './components/formatters/iconbuttonformatter';
import { BankConnectionProgramFormatterComponent } from './components/formatters/bankConnectionProgramFormatter';
import { RiskyKeywordComponent } from './components/risky-keyword/risky-keyword.component';
import { RiskyKeywordEffects } from './core/effects/riskyKeyword';
import { ManualTransferEffects } from './core/effects/manualTransfer';
import { ManualTransferListComponent } from './components/manual-transfer-list/manual-transfer-list.component';
import { CreateManualTransferComponent } from './components/create-manual-transfer/create-manual-transfer.component';
import { EditManualTransferComponent } from './components/edit-manual-transfer/edit-manual-transfer.component';
import { DownloadManualTransferReceiptFormatterComponent } from './components/formatters/downloadManualTransferReceiptFormatter';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MobileTransferDeviceEffects } from './core/effects/mobileTransferDevice';
import { MobileTransferDeviceListComponent } from './components/mobile-transfer-device-list/mobile-transfer-device-list.component';
import { MobileTransferDeviceComponent } from './components/mobile-transfer-device/mobile-transfer-device.component';
import { MobileTransferAccountListComponent } from './components/mobile-transfer-account-list/mobile-transfer-account-list.component';
import { MobileTransferCardAccountEffects } from './core/effects/mobileTransferCardAccount';
import { MobileTransferCardAccountComponent } from './components/mobile-transfer-card-account/mobile-transfer-card-account.component';
import { MobileTransferAccountGroupListComponent } from './components/mobile-transfer-account-group-list/mobile-transfer-account-group-list.component';
import { MobileTransferAccountGroupEffects } from './core/effects/mobileTransferAccountGroup';
import { MobileTransferAccountGroupComponent } from './components/mobile-transfer-account-group/mobile-transfer-account-group.component';
import { BlockCardsComponent } from './components/block-cards/block-cards.component';
import { BlockedPhoneNumberListComponent } from './components/blocked-phone-number-list/blocked-phone-number-list.component';
import { BlockedPhoneNumberEffects } from './core/effects/blockedPhoneNumber';
import { BlockedPhoneNumberComponent } from './components/blocked-phone-number/blocked-phone-number.component';
import { WithdrawalDetailComponent } from './components/withdrawal-detail/withdrawal-detail.component';
import { CardHolderEffects } from './core/effects/cardHolder';
import { CardHolderNameComponent } from './components/card-holder-name/card-holder-name.component';
import { ReportEffects } from './core/effects/report';
import { UserSegmentReportComponent } from './components/user-segment-report/user-segment-report.component';
import { TenantBalanceComponent } from './components/tenant-balance/tenant-balance.component';
import { DepositWithdrawalChartComponent } from './components/deposit-withdrawal-chart/deposit-withdrawal-chart.component';
import { ReportsComponent } from './components/reports/reports.component';
import { WithdrawalPaymentChartComponent } from './components/withdrawal-payment-chart/withdrawal-payment-chart.component';
import { EnvironmentService } from './core/environment/environment.service';
import { CustomRouterStateSerializer } from './core/reducers/router';

import { InvoiceOwnerSettingComponent } from './components/invoice-owner-setting/invoice-owner-setting.component';
import { InvoiceOwnerSettingListComponent } from './components/invoice-owner-setting-list/invoice-owner-setting-list.component';
import { InvoiceOwnerSettingEffects } from './core/effects/invoiceOwnerSetting';
import { InvoiceListComponent } from './components/invoice-list/invoice-list.component';
import { InvoiceDetailComponent } from './components/invoice-detail/invoice-detail.component';
import { InvoiceEffects } from './core/effects/invoice';
import { InvoicePaymentListComponent } from './components/invoice-payment-list/invoice-payment-list.component';
import { InvoicePaymentComponent } from './components/invoice-payment/invoice-payment.component';
import { InvoicePaymentEffects } from './core/effects/invoicePayment';
import { NgxCurrencyModule } from "ngx-currency";
import { OwnerSettingComponent } from './components/owner-setting/owner-setting.component';
import { QRCodeRegisterService } from './core/services/qrCodeRegister/qrcode-register.service';
//import { BankLoginEffects } from './core/effects/bankLogin';
import { QRCodeRegistrationComponent } from './components/qr-code-registration/qr-code-registration.component';
import { LoginDeviceStatusListComponent } from './components/login-device-status-list/login-device-status-list.component';
import { LoginDeviceStatusService } from './core/services/loginDeviceStatus/login-device-status.service';
import { LoginDeviceStatusEffects } from './core/effects/loginDeviceStatus';
import { DepositByAccountNumberChartComponent } from './components/deposit-by-accountnumber-chart/deposit-by-accountnumber-chart.component';
import { BlockedCardNumberEffects } from './core/effects/blocked-card-number';
import { BlockedCardNumberListComponent } from './components/blocked-card-number-list/blocked-card-number-list.component';
import { BlockedCardNumberComponent } from './components/blocked-card-number/blocked-card-number.component';
import { RemoveRegisteredPhoneNumbersComponent } from './components/remove-registered-phone/remove-registered-phone.component';

import { notificationReducer } from './core/reducers/notification';
import { CommonModule } from '@angular/common';
import { FuseConfigService } from '@fuse/services/config.service';
import { FuseModule } from '@fuse/fuse.module';
import { StaffCreateComponent } from './components/staff-create/staff-create.component';
import { StaffEditComponent } from './components/staff-edit/staff-edit.component';
import { TenantStaffUsersComponent } from './components/tenant-staff-users/tenant-staff-users.component';
import { TenantSelectComponent } from './components/tenant-select/tenant-select.component';
import { StaffListComponent } from './components/staff-list/staff-list.component';
import { ProviderStaffListComponent } from './components/provider-staff-list/provider-staff-list.component';
import { RolesComponent } from './components/roles/roles.component';
import { RoleCreateComponent } from './components/role-create/role-create.component';
import { RoleListComponent } from './components/role-list/role-list.component';
import { RoleEditComponent } from './components/role-edit/role-edit.component';
import { SimpleNotificationsModule } from 'angular2-notifications';
import { LoginAsFormatterComponent } from './components/formatter/loginasformater';
import { RoleEffects } from './core/effects/role.effects';
import { RoleService } from './core/services/role.service';
import { ConfirmationDialogComponent } from './components/confirm-dialog/confirm-dialog.component';

registerLocaleData(localeFa, 'fa');

@NgModule({
    declarations: [
        AppComponent,
        DashboardComponent,
        MerchantManagementComponent,
        MerchantListComponent,
        MerchantCreateComponent,
        TransactionListComponent,
        //FuseWidgetComponent,
        LocalizedDatePipe,
        AccountingListComponent,
        WithdrawalComponent,
        WithdrawalListComponent,
        TenantListComponent,
        TenantComponent,
        CardToCardAccountListComponent,
        CardToCardAccountComponent,
        TransferAccountListComponent,
        TransferAccountComponent,
        BankLoginComponent,
        BankLoginListComponent,
        BankLoginAccountComponent,
        UsersComponent,
        UserComponent,
        BankLoginChangePasswordComponent,
        BankStatementListComponent,
        TenantUrlConfigComponent,
        TenantUrlConfigListComponent,
        AutoTransferListComponent,
        DeactivateLoginDialogComponent,
        DeleteLoginDialogComponent,
        BooleanFormatterComponent,
        DateFormatterComponent,
        NumberFormatterComponent,
        BooleanInverseFormatterComponent,
        CardToCardAccountGroupListComponent,
        CardToCardAccountGroupComponent,
        MerchantCustomerListComponent,
        MerchantCustomerComponent,
        UserSegmentGroupComponent,
        UserSegmentGroupListComponent,
        DownloadWithdrawalReceiptFormatterComponent,
        DeleteButtonFormatterComponent,
        ApplicationSettingsComponent,
        IconButtonFormatterComponent,
        BankConnectionProgramFormatterComponent,
        RiskyKeywordComponent,
        ManualTransferListComponent,
        CreateManualTransferComponent,
        EditManualTransferComponent,
        DownloadManualTransferReceiptFormatterComponent,
        MobileTransferDeviceListComponent,
        MobileTransferDeviceComponent,
        MobileTransferAccountListComponent,
        MobileTransferCardAccountComponent,
        MobileTransferAccountGroupListComponent,
        MobileTransferAccountGroupComponent,
        BlockCardsComponent,
        BlockedPhoneNumberListComponent,
        BlockedPhoneNumberComponent,
        WithdrawalDetailComponent,
        CardHolderNameComponent,        
        UserSegmentReportComponent,
        TenantBalanceComponent,
        DepositWithdrawalChartComponent,
        ReportsComponent,
        WithdrawalPaymentChartComponent,
        InvoiceOwnerSettingComponent,
        InvoiceOwnerSettingListComponent,
        InvoiceListComponent,
        InvoiceDetailComponent,
        InvoicePaymentListComponent,
        InvoicePaymentComponent,
        OwnerSettingComponent,
        QRCodeRegistrationComponent,
        LoginDeviceStatusListComponent,
        DepositByAccountNumberChartComponent,
        BlockedCardNumberListComponent,
        BlockedCardNumberComponent,
        RemoveRegisteredPhoneNumbersComponent,
        ChangePasswordComponent,
        StaffCreateComponent,
        StaffEditComponent,
        TenantStaffUsersComponent,
        TenantSelectComponent,
        StaffListComponent,
        ProviderStaffListComponent,
        RolesComponent ,
        RoleCreateComponent,
        RoleListComponent,
        RoleEditComponent,
        LoginAsFormatterComponent,
        ConfirmationDialogComponent
    ],
    imports     : [
        CommonModule,
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        PagesModule,
        AppRoutingModule,
        //RouterModule.forRoot(appRoutes),
        SimpleNotificationsModule.forRoot(),
        TranslateModule.forRoot(),

        // Material moment date module
        MatMomentDateModule,

        // Material
        MatButtonModule,
        MatIconModule,
        MatSelectModule,

        // Fuse modules
        //FuseModule.forRoot(fuseConfig),
        FuseProgressBarModule,
        FuseSharedModule,
        FuseSidebarModule,
        FuseThemeOptionsModule,
        FuseWidgetModule,
        // App modules
        LayoutModule,
        SampleModule,
        NgxChartsModule,
        FuseModule.forRoot(fuseConfig),
        StoreModule.forRoot(reducers, { metaReducers }),
        StoreModule.forFeature('notification', notificationReducer),

        // AdminManagementModule,
        
        NgxCurrencyModule,

        /**
         * @ngrx/router-store keeps router state up-to-date in the store.
         */
        StoreRouterConnectingModule,
         AgGridModule.withComponents([]),
        /**
         * Store devtools instrument the store retaining past versions of state
         * and recalculating new states. This enables powerful time-travel
         * debugging.
         *
         * To use the debugger, install the Redux Devtools extension for either
         * Chrome or Firefox
         *
         * See: https://github.com/zalmoxisus/redux-devtools-extension
         */
        !environment.production ? StoreDevtoolsModule.instrument() : [],

        /**
         * EffectsModule.forRoot() is imported once in the root module and
         * sets up the effects class to be initialized immediately when the
         * application starts.
         *
         * See: https://github.com/ngrx/platform/blob/master/docs/effects/api.md#forroot
         */
        EffectsModule.forRoot([
            UserEffects,
            AccountEffects,
            MerchantEffects,
            TransactionEffects,
            DashboardEffects,
            AccountingEffects,
            WithdrawalEffects,
            MerchantBankAccountEffects,
            TenantEffects,
            CardToCardAccountEffects,
            BankLoginEffects,
            TransferAccountEffects,
            BankStatementItemEffects,
            TenantUrlConfigEffects,
            AutoTransferEffects,
            CardToCardAccountGroupEffects,
            MerchantCustomerEffects,
            UserSegmentGroupEffects,
            ApplicationSettingsEffects,
            RiskyKeywordEffects,
            ManualTransferEffects,
            MobileTransferDeviceEffects,
            MobileTransferCardAccountEffects,
            MobileTransferAccountGroupEffects,
            BlockedPhoneNumberEffects,
            CardHolderEffects,
            ReportEffects,
            InvoiceOwnerSettingEffects,
            InvoiceEffects,
            InvoicePaymentEffects,
            LoginDeviceStatusEffects,
            BlockedCardNumberEffects,
            RoleEffects
        ]),
        SharedModule,
        NgxMatSelectSearchModule
    ],
    providers: [
       
        AuthGuard,
        PermissionRouteGuard,
        PermissionGuard,
        EnvironmentService,
        {
            provide: APP_INITIALIZER,
            useFactory: (environmentService: EnvironmentService) => function () { return environmentService.loadAndSetEnvironment(); },
            deps: [EnvironmentService],
            multi: true
        },
       
        AccountService,
        MerchantService,
        TransactionService,
        DashboardService,
        AccountingService,
        RandomService,
        CardToCardAccountService,
        BankLoginService,
        TransferAccountService,
        TenantService,
        DecimalPipe,
        DatePipe,
        TimeZoneService,
        UserService,
        BankLoginService,
        RoleService,
        /**
         * The `RouterStateSnapshot` provided by the `Router` is a large complex structure.
         * A custom RouterStateSerializer is used to parse the `RouterStateSnapshot` provided
         * by `@ngrx/router-store` to include only the desired pieces of the snapshot.
         */
        { provide: RouterStateSerializer, useClass: CustomRouterStateSerializer },
        { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: LocaleInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: TenantInterceptor, multi: true },
    ],
    bootstrap   : [
        AppComponent
    ],
    entryComponents: [
        DeactivateLoginDialogComponent,
        DeleteLoginDialogComponent,
        BooleanFormatterComponent,
        DateFormatterComponent,
        NumberFormatterComponent,
        BooleanInverseFormatterComponent,
        DownloadWithdrawalReceiptFormatterComponent,
        DeleteButtonFormatterComponent,
        ChangePasswordComponent,
        IconButtonFormatterComponent,
        DownloadManualTransferReceiptFormatterComponent,
        BankConnectionProgramFormatterComponent,
        LoginAsFormatterComponent,
        ConfirmationDialogComponent,

    ]
})
export class AppModule
{
    
}
