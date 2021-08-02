import { FuseNavigation } from '../../@fuse/types';
import * as permissions from '../models/permissions';
import { Permissions } from '../models/index';

export const navigation: FuseNavigation[] = [
    {
        id: 'applications',
        title: null,
        type: 'group',
        order:'0001',
        permission: '',
        children: [
            {
                id: 'dashboard',
                title: 'DASHBOARD.TITLE',
                type: 'item',
                icon: 'dashboard',
                url: '/dashboard',
                permission: ''
            },
            {
                id: 'transactions',
                title: 'TRANSACTION.GENERAL.TRANSACTIONS',
                type: 'item',
                icon: 'add_to_queue',
                url: '/transactions',
                permission: permissions.ListDeposits
            },
            // {
            //     id: 'statement',
            //     title: 'BANK-STATEMENT.GENERAL.BANK-STATEMENTS',
            //     type: 'item',
            //     icon: 'list_alt',
            //     url: '/statement',
            //     permission: permissions.ListBankStatements
            // },
            // {
            //     id: 'autotransfer',
            //     title: 'AUTO-TRANSFER.GENERAL.TRANSFERS',
            //     type: 'item',
            //     icon: 'autorenew',
            //     url: '/autotransfers',
            //     permission: permissions.ListAutoTransfers
            // },
            {
                id: 'withdrawal',
                title: 'WITHDRAWAL.GENERAL.WITHDRAWALS',
                type: 'item',
                icon: 'remove_from_queue',
                url: '/withdrawals',
                permission: permissions.ListWithdrawals
            },
            {
                id: 'accounting',
                title: 'ACCOUNTING.GENERAL.ACCOUNTING',
                type: 'item',
                icon: 'receipt',
                url: '/accounting',
                permission: permissions.ListAccounting
            },
            {
                id: 'manualtransfers',
                title: 'MANUAL-TRANSFER.GENERAL.MANUAL-TRANSFERS',
                type: 'item',
                icon: 'schedule',
                url: '/manualtransfers',
                permission: permissions.ListManualTransfers
            },
            {
                id: 'transferaccounts',
                title: 'TRANSFER-ACCOUNT.GENERAL.TRANSFER-ACCOUNTS',
                type: 'item',
                icon: 'person_add',
                url: '/transferaccounts',
                permission: permissions.ListTransferAccounts
            },
            {
                id: 'merchantCustomer',
                title: 'MERCHANT-CUSTOMER.GENERAL.MERCHANT-CUSTOMERS',
                type: 'item',
                icon: 'people',
                url: '/merchantcustomers',
                permission: permissions.ListCustomers
            },
            {
                id: 'reports',
                title: 'REPORTS.REPORTS',
                type: 'item',
                icon: 'assessment',
                url: '/reports',
                permission: permissions.ShowReports
            },
            {
                id: 'cardHolders',
                title: 'NAV.CARD-HOLDER-NAMES',
                type: 'item',
                icon: 'how_to_reg',
                url: '/cardHolders',
                permission: permissions.ListCardHolderNames
            },
            {
                id: 'invoices',
                title: 'INVOICE.GENERAL.INVOICES',
                type: 'item',
                icon: 'attach_money',
                url: '/invoices',
                permission: permissions.ListInvoices
            },
            {
                id: 'invoicePayments',
                title: 'INVOICE-PAYMENT.GENERAL.INVOICE-PAYMENTS',
                type: 'item',
                icon: 'payment',
                url: '/invoicepayments',
                permission: permissions.ListInvoicePayments
            }

        ]
    },
    {
        id: 'mobile',
        title: 'NAV.MOBILE-TRANSFER',
        type: 'group',
        order:'0002',
        permission: '',
        children: [
            // {
            //     id: 'mobiletransferdevices',
            //     title: 'MOBILE-TRANSFER-DEVICE.GENERAL.DEVICES',
            //     type: 'item',
            //     icon: 'smartphone',
            //     url: '/mobiletransferdevices',
            //     permission: permissions.ListMobileTransferDevices
            // },
            {
                id: 'mobiletransfercardaccounts',
                title: 'MOBILE-TRANSFER-CARD-ACCOUNT.GENERAL.CARD-ACCOUNTS',
                type: 'item',
                icon: 'subtitles',
                url: '/mobiletransfercardaccounts',
                permission: permissions.ListMobileTransferAccounts
            },
            {
                id: 'mobiletransferaccountgroupss',
                title: 'MOBILE-TRANSFER-CARD-ACCOUNT-GROUP.GENERAL.GROUPS',
                type: 'item',
                icon: 'library_books',
                url: '/mobiletransferaccountgroups',
                permission: permissions.ListMobileTransferGroups
            }
        ]
    },
    
    {
        id: 'setup',
        title: 'NAV.SETUP',
        type: 'group',
        order:'0003',
        permission: '',
        children: [
            // {
            //     id: 'banklogins',
            //     title: 'BANK-LOGIN.GENERAL.BANK-LOGINS',
            //     type: 'item',
            //     icon: 'account_balance',
            //     url: '/banklogins',
            //     permission: permissions.ListBankLogins
            // },
            // {
            //     id: 'loginDeviceStatusList',
            //     title: 'BANK-LOGIN.GENERAL.LOGINS-DEVICE-STATUS',
            //     type: 'item',
            //     icon: 'account_balance',
            //     url: '/loginDeviceStatusList',
            //     permission: permissions.LoginDeviceStatusSettings
            // },
            // {
            //     id: 'cardtocardaccounts',
            //     title: 'CARD-TO-CARD.GENERAL.CARD-TO-CARD-ACCOUNTS',
            //     type: 'item',
            //     icon: 'account_balance_wallet',
            //     url: '/cardtocardaccounts',
            //     permission: permissions.ListBankAccounts
            // },
            // {
            //     id: 'cardtocardaccountgroups',
            //     title: 'CARD-TO-CARD-ACCOUNT-GROUP.GENERAL.CARD-TO-CARD-ACCOUNT-GROUPS',
            //     type: 'item',
            //     icon: 'group_work',
            //     url: '/cardtocardaccountgroups',
            //     permission: permissions.ListBankAccountGroups
            // },
            {
                id: 'merchants',
                title: 'MERCHANT.GENERAL.MERCHANT-MANAGEMENT',
                type: 'item',
                icon: 'business',
                url: '/merchants',
                permission: permissions.ListMerchants
            },
            {
                id: 'usersegmentgroups',
                title: 'USER-SEGMENT.GENERAL.USER-SEGMENT-GROUPS',
                type: 'item',
                icon: 'category',
                url: '/usersegmentgroups',
                permission: permissions.ListUserSegments
            },
            {
                id: 'applicationsettings',
                title: 'APPLICATION-SETTINGS.GENERAL.APPLICATION-SETTINGS',
                type: 'item',
                icon: 'settings',
                url: '/applicationsettings',
                permission: permissions.ListApplicationSettings
            },
            {
                id: 'invoiceownersettings',
                title: 'INVOICE-OWNER-SETTINGS.GENERAL.INVOICE-OWNER-SETTINGS',
                type: 'item',
                icon: 'vertical_split',
                url: '/invoiceownersettings',
                permission: permissions.ListInvoiceOwnerSetting
            },
            {
                id: 'riskykeywords',
                title: 'DASHBOARD.RISKY-KEYWORDS',
                type: 'item',
                icon: 'block',
                url: '/riskykeywords',
                permission: permissions.ListApplicationSettings
            },
            {
                id: 'ownersettings',
                title: 'APPLICATION-SETTINGS.GENERAL.SETTINGS',
                type: 'item',
                icon: 'confirmation_number',
                url: '/ownersetting',
                permission: permissions.OwnerSettings
            },
            {
                id: 'blockedphonenumbers',
                title: 'BLOCKED-PHONE-NUMBER.GENERAL.BLOCKED-PHONE-NUMBERS',
                type: 'item',
                icon: 'sim_card',
                url: '/blockedphonenumbers',
                permission: permissions.ListBlockedPhoneNumbers
            },
            {
                id: 'blockedcardnumbers',
                title: 'BLOCKED-CARD-NUMBER.GENERAL.BLOCKED-CARD-NUMBERS',
                type: 'item',
                icon: 'sim_card',
                url: '/blockedcardnumbers',
                permission: permissions.ListBlockedCardNumbers
            },
            {
                id: 'user-management',
                order: '050',
                title: 'NAV.USER-MANAGEMENT',
                type: 'collapsable',
                icon: 'supervised_user_circle',
                permission: Permissions.UserManagement,
                children: [
                    {
                        id: 'provider',
                        title: 'NAV-MENU.PROVIDER-USERS',
                        type: 'item',
                        icon: 'supervised_user_circle',
                        url: '/provider',
                        permission: Permissions.ProviderAccess
                    },
                    {
                        id: 'tenantStaffUsers',
                        title: 'NAV-MENU.OPERATOR-TENANT-STAFF-USERS',
                        type: 'item',
                        icon: 'store',
                        url: '/tenant-staff-users',
                        permission: Permissions.TenantStaffUsers
                    },
                    {
                        id: 'rolemanagement',
                        title: 'NAV-MENU.ROLES',
                        type: 'item',
                        icon: 'vpn_key',
                        url: '/roles',
                        permission: Permissions.RoleAccess
                    }
                ]
            },
            // ,
            // {
            //     id: 'tenanturlconfig',
            //     title: 'TENANT-URL-CONFIG.GENERAL.TENANT-URL-CONFIG',
            //     type: 'item',
            //     icon: 'domain',
            //     url: '/tenanturlconfiglist',
            //     permission: permissions.ListUrlConfiguration
            // }
        ]
    }
];
