export const locale = {
    lang: 'en',
    data: {
        'NAV': {
            'APPLICATIONS': '',
            'SETUP': 'Setup',
            'SAMPLE': {
                'TITLE': 'Sample',
                'BADGE': '25'
            },
            'LOGOUT': 'Logout',
            'CHANGE_PASSWORD': 'Change Password',
            'MOBILE-TRANSFER': 'PardakhtPal PG',
            'CARD-HOLDER-NAMES': 'Account Holder Inquiry'
        },
        'MERCHANT': {
            'LIST-COLUMNS': {
                'EMAIL': 'Email',
                'PRIMARY-DOMAIN': 'Primary Domain',
                'TITLE': 'Title',
                'CARD-TO-CARD-ACCOUNT': 'Bank Accounts',
                'TENANT': 'Tenant',
                'MINIMUM-AMOUNT': 'Minimum Amount',
                'USE-PARTIAL-PAYMENT-FOR-WITHDRAWAL': 'Use Partial Payment For Withdrawals'
            },
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            },
            'GENERAL': {
                'MERCHANT-MANAGEMENT': 'Merchant Management',
                'CREATE-MERCHANT': 'Create Merchant',
                'CANCEL-CREATE-MERCHANT': 'Cancel Create Merchant',
                'NEW_MERCHANT': 'New Merchant',
                'ADD_MERCHANT': 'Add Merchant',
                'SAVE': 'Save Merchant',
                'EDIT_MERCHANT': 'Edit Merchant'
            },
            'TAB': {
                'MERCHANT_DETAIL': 'Merchant Detail',
                'MERCHANT_ADMIN': 'Merchant Admin',
                'CARD_TO_CARD_ACCOUNTS': 'Card to Card Accounts'
            },
            'BANK_ACCOUNT': {
                'ACCOUNT_NUMBER': 'Account Number',
                'CARD_NUMBER': 'Card Number',
                'BUSINESS_ACCOUNT_NUMBER': 'Business Account Number',
                'TRANSFER_TRESHOLD': 'Transfer Threshold',
                'API_KEY': 'Api Key',
                'IS_ACTIVE': 'Is Active',
                'CREATE': 'Create new account',
                'DELETE': 'Delete account',
                'GENERATE_API_KEY': 'Generate Api Key',
                'USE-CARD-TO-CARD-PAYMENT-FOR-WITHDRAWAL': 'Use Card To Card Payment For Withdrawal'
            },
            'FIELDS': {
                'FIRST-NAME': 'First Name',
                'LAST-NAME': 'Last Name',
                'USERNAME': 'Username',
                'PASSWORD': 'Password'
            }
        },
        'DASHBOARD': {
            'TITLE': 'Dashboard',
            'TOTAL_TRANSACTION_AMOUNT': 'Total Deposit Amount',
            'TOTAL_TRANSACTION_COUNT': 'Total Deposit Count',
            'ACCOUNT-SUMMARY': 'Account Summary',
            'ACCOUNT-NO': 'Account No',
            'LOGIN-NAME': 'Login Name',
            'BANK': 'Bank',
            'STATUS': 'Status',
            'BLOCKED': 'Login Status',
            'OPEN': 'Open',
            'ACCOUNT-STATUS': 'Account Status',
            'CARD-HOLDER-NAME': 'Card Holder Name',
            'CARD-NUMBER': 'Card Number',
            'ACCOUNT-BALANCE': 'Account Balance',
            'NORMAL-WITHDRAWABLE': 'N. W.',
            'PAYA-WITHDRAWABLE': 'P. W.',
            'SATNA-WITHDRAWABLE': 'S. W.',
            'TOTAL-DEPOSIT-TODAY': 'T. Dep. T',
            'TOTAL-WITHDRAWAL-TODAY': 'T. With. T',
            'BLOCKED-BALANCE': 'Blocked Balance',
            'PENDING-WITHDRAWAL-AMOUNT': 'Pending Withdrawal Amount',
            'TOTAL_WITHDRAWAL_AMOUNT': 'Total Withdrawal Amount',
            'TOTAL_WITHDRAWAL_COUNT': 'Total Withdrawal Count',
            'RISKY-KEYWORDS': 'Risky Keywords',
            'TOTAL_TRANSACTION_WITHDRAWAL_AMOUNT': 'Total Withdrawal Amount',
            'MOBILE-COUNT': 'PardakhtPal Deposit Count',
            'PAYMENTGATEWAY-COUNT': 'Payment Gateway Count',
            'MOBILE-AMOUNT': 'PardakhtPal Deposit Amount',
            'PAYMENTGATEWAY-AMOUNT': 'Payment Gateway Amount',
            'PENDING-WITHDRAWAL-COUNT': 'Pending Withdrawal Count'
 
        },
        'TRANSACTIONS': {
            'ACTIONS': {
                'REFRESH': 'Refresh',
                'SEARCH': 'Search'
            }
        },
        'TRANSACTION': {
            'ACTION': {
                'REFRESH': 'Refresh',
                'SEARCH': 'Search'
            },
            'GENERAL': {
                'TRANSACTIONS': 'Deposits',
                'REFRESH': 'Refresh',
                'SEARCH_MERCHANTS': 'Search Merchants',
                'SEARCH_TENANTS': 'Search Tenants',
                'DEPOSIT_BREAKDOWN': 'Deposit Break Down',
            },
            'LIST-COLUMNS': {
                'AMOUNT': 'Amount',
                'DATE': 'Deposit Date',
                'TRANSFERRED_DATE': 'Transferred Date',
                'TITLE': 'Merchant',
                'STATUS': 'Status',
                'ACCOUNT_NUMBER': 'Account Number',
                'MERCHANT_CARD_NUMBER': 'Merchant Card Number',
                'CUSTOMER_CARD_NUMBER': 'Customer Card Number',
                'BANK_NUMBER': 'Transaction Number',
                'TENANT': 'Tenant',
                'TOKEN': 'Token',
                'ID': 'Transaction Id',
                'EXTERNAL-ID': 'External Id',
                'EXTERNAL-MESSAGE': 'External Message',
                'PROCESS-ID': 'Process Id'
            },
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            },
            'STATUS': {
                'STARTED': 'Started',
                'TOKEN_VALIDATED': 'Waiting Payment',
                'WAITING_CONFIRMATION': 'Waiting Confirmation',
                'COMPLETED': 'Completed',
                'EXPIRED': 'Expired',
                'CANCELLED': 'Cancelled',
                'PLACE_HOLDER': 'Search Status',
                'FRAUD': 'Fraud',
                'REVERSED': 'Reversed'
            },
            'PAYMENT-TYPES': {
                'CARD-TO-CARD': 'Card To Card',
                'MOBILE': 'Carti Pal',
                'PLACE-HOLDER': 'Payment Types',
                'PAYMENT-GATEWAY': 'Payment Gateway',
                'SAMANBANK':'Samanbank',
                'MELI':'Meli',
                'ZARINPAL':'Zarinpal',
                'MELLAT':'Mellat',
                'NOVIN':'Novin'
            }
        },
        'WITHDRAWAL': {
            'GENERAL': {
                'WITHDRAWALS': 'Withdrawals',
                'REFRESH': 'Refresh',
                'CREATE_WITHDRAWAL': 'Create Withdrawal',
                'ADD_WITHDRAWAL': 'Add Withdrawal',
                'PROCESS-TYPE-TO-APPROVE': 'Are you sure to pay this withdrawal by withdraw accounts?'
            },
            'LIST-COLUMNS': {
                'AMOUNT': 'Amount',
                'DATE': 'Withdrawal Date',
                'TRANSFER_REQUEST_DATE': 'Transfer Request Date',
                'TRANSFER_DATE': 'Transfer Date',
                'TITLE': 'Merchant',
                'STATUS': 'Status',
                'ACCOUNT_NUMBER': 'From Account Number',
                'BUSINESS_ACCOUNT_NUMBER': 'To Account Number',
                'TRANSFER_NOTES': 'Transfer Notes',
                'PRIORITY': 'Priority',
                'TRANSFER_TYPE': 'Transfer Type',
                'TENANT': 'Tenant',
                'TRANSFER_ACCOUNT': 'Transfer Account',
                'TO-IBAN-NUMBER': 'IBAN Number',
                'EXPECTED-TRANSFER-DATE': 'Planned Transfer Date',
                'REFERENCE': 'Reference Key',
                'FIRST-NAME': 'First Name',
                'LAST-NAME': 'Last Name',
                'TRACKING-NUMBER': 'Tracking Number',
                'DOWNLOAD_RECEIPT': 'Download Receipt',
                'ID': 'Withdrawal Id',
                'PROCESS-TYPE': 'Process Type',
                'CARD-TO-CARD-TRY-COUNT': 'Card To Card Try Count',
                'TRANSFER-FROM-ACCOUNT': 'Transfer By Withdraw Accounts',
                'REMAINING-AMOUNT': 'Remaining Amount',
                'PENDING-APPROVAL-AMOUNT': 'Pending Approval Amount',
                'TRANSFER-ALL-FROM-ACCOUNT': 'Transfer All By Withdrawal Accounts'                
            },
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            },
            'STATUS': {
                'WAITING': 'Waiting',
                'COMPLETED': 'Completed',
                'PLACE_HOLDER': 'Search Status',
                'ALL': 'All',
                'PENDING': 'Pending',
                'TRANSFERRED': 'Transferred',
                'CANCELLED': 'Cancelled',
                'SENT': 'Sent',
                'CONFIRMED': 'Confirmed',
                'REFUNDED': 'Refunded',
                'PENDING-APPROVAL': 'Pending Approval',
                'PARTIAL-PAID': 'Partial Paid'
            },
            'PROCESS_TYPES': {
                'TRANSFER': 'Transfer',
                'CARD-TO-CARD': 'Card To Card',
                'BOTH': 'Both'
            },
            'STATUSES': {
                'PENDING': 'Pending',
                'CANCELLED-BY-USER': 'Cancelled By User',
                'CANCELLED-BY-SYSTEM': 'Cancelled By System',
                'CONFIRMED': 'Confirmed',
                'INSUFFICIENT-BALANCE': 'Insufficient Balance',
                'SENT': 'Sent',
                'REFUNDED': 'Refunded',
                'PENDING-CARD-TO-CARD-CONFIRMATION': 'Withdrawal needs to be confirmed',
                'PARTIAL-PAID': 'Partial Paid'
            }
        },
        'ACCOUNTING': {
            'GENERAL': {
                'ACCOUNTING': 'Accounting',
                'REFRESH': 'Refresh',
                'GROUP-TYPE': 'Group Type',
                'DOWNLOAD': 'Download'
            },
            'LIST-COLUMNS': {
                'AMOUNT': 'Amount',
                'DATE': 'Date',
                'TITLE': 'Merchant',
                'COUNT': 'Deposit Count',
                'ACCOUNT_NUMBER': 'Account Number',
                'TENANT': 'Tenant',
                'CARD_NUMBER': 'Card Number',
                'CARD_HOLDER_NAME': 'Card Holder Name',
                'TOTAL-DEPOSIT-AMOUNT-PERCENTAGE': 'Deposit (%)',
                'TOTAL-WITHDRAWAL-AMOUNT-PERCENTAGE': 'Withdrawal (%)'
            },
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            },
            'DASHBOARD': {
                'SALES': 'Sales',
                'WITHDRAWALS': 'Withdrawals'
            }
        },
        'DATERANGE': {
            'TODAY': 'Today',
            'YESTERDAY': 'Yesterday',
            'THIS_WEEK': 'This Week',
            'LAST_WEEK': 'Last Week',
            'THIS_MONTH': 'This Month',
            'LAST_MONTH': 'Last Month',
            'ALL': 'All',
            'WEEKLY': 'Weekly',
            'MONTHLY': 'Monthly',
            'QUARTERLY': 'Quarterly',
            'PLACE_HOLDER': 'Search Date',
            'DAILY': 'Daily',
            'FROM-DATE': 'From',
            'TO-DATE': 'To'
        },
        'GENERAL': {
            'OK': 'Ok',
            'NO': 'No',
            'PARDAKHTPAY': 'PardakhtPay Payment',
            'MERCHANT': 'Merchant',
            'CRITERIA_SELECTED': 'criteria(s) selected',
            'ID': 'Id',
            'COUNT': 'Count',
            'VALUE': 'Value',
            'CANCELLER-ID': 'Canceller User',
            'CREATOR-ID': 'Creator User',
            'UPDATER-ID': 'Updater User',
            'BANK-ID': 'Bank Id',
            'BANK-ACCOUNT-ID': 'Bank Account Id',
            'BANK-LOGIN-ID': 'Bank Login Id',
            'FILTER': 'Filter',
            'COPY': 'Copy'
        },
        'ACCOUNT': {
            'LOGIN': 'Login',
            'LOGIN_TO_YOUR_ACCOUNT': 'Login To Your Account',
            'USERNAME': 'Username',
            'USERNAME_IS_REQUIRED': 'Username is required',
            'USERNAME_IS_NOT_VALID': 'Please enter a valid username',
            'CHANGE_PASSWORD': 'Change Password',
            'PASSWORD_IS_REQUIRED': 'Password is required',
            'OLD_PASSWORD_IS_REQUIRED': 'Old password is required',
            'PASSWORD_CONFIRMATION_IS_REQUIRED': 'Password confirmation is required',
            'PASSWORD_MUST_MATCH': 'Password must match',
            'GO_TO_DASHBOARD': 'Go to dashboard',
            'OLD_PASSWORD': 'Old Password',
            'PASSWORD': 'Password',
            'PASSWORD_CONFIRM': 'Password Confirm',
            'LOGIN-AS-USERNAME': 'Login As'
        },
        'TENANTS': {
            'SELECTED_TENANT': 'Selected Tenant',
        },
        'TENANT': {
            'SELECTED-TENANT': 'Selected Tenant',
            'GENERAL': {
                'TENANTS': 'Tenants',
                'REFRESH': 'Refresh',
                'ADD_TENANT': 'Add Tenant'
            },
            'LIST-COLUMNS': {
                'NAME': 'Name',
                'TENANT_GUID': 'Tenant Guid',
                'DOMAIN_NAME': 'Domain Name'
            },
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            },
            'FIELDS': {
                'FIRST-NAME': 'First Name',
                'LAST-NAME': 'Last Name',
                'USERNAME': 'Username',
                'PASSWORD': 'Password',
                'EMAIL': 'Email'
            }
        },
        'CARD-TO-CARD': {
            'GENERAL': {
                'CARD-TO-CARD-ACCOUNTS': 'Bank accounts',
                'CREATE-CARD-TO-CARD-ACCOUNT': 'Create Bank Account',
                'SAVE': 'Save Bank account',
                'NEW-CARD-TO-CARD': 'New Bank Account',
                'EDIT-CARD-TO-CARD': 'Edit Bank Account',
                'ADD-CARD-TO-CARD': 'Add Bank account',
                'CARD-TO-CARD_DETAIL': 'Bank account detail',
                'REGISTER-DEVICE': 'Register device',
            },
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            },
            'LIST-COLUMNS': {
                'LOGIN-NAME': 'Login Name',
                'ACCOUNT-NUMBER': 'Account Number',
                'CARD-NUMBER': 'Card Number',
                'CARD-HOLDER-NAME': 'Card Holder Name',
                'SAFE-ACCOUNT-NUMBER': 'Safe Account Number',
                'TRANSFER-THRESHOLD-AMOUNT': 'Transfer Threshold Amount',
                'IS-ACTIVE': 'Is Active',
                'IS_TRANSFER_THRESHOLD_ACTIVE': 'Activate Auto Transfer',
                'TRANSFER_THRESHOLD_LIMIT': 'Transfer Threshold Limit',
                'OWNER': 'Owner',
                'SWITCH-LIMIT': 'Account Switch Limit',
                'SWITCH-ON-LIMIT': 'Switch Account When Reaches Limit',
                'SWITCH-IF-RESERVE-ACCOUNT': 'Switch Account If There Is a Reserved One',
                'SWITCH-DAILY-CREDIT-LIMIT': 'Daily Credit Limit For Switching',
                'OTP': 'OTP'
            }
        },
        'TRANSFER-ACCOUNT': {
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            },
            'GENERAL': {
                'TRANSFER-ACCOUNTS': 'Payee Management',
                'CREATE-TRANSFER-ACCOUNT': 'Create Payee',
                'ADD-TRANSFER-ACCOUNT': 'Add Payee',
                'SAVE': 'Save Payee',
                'NEW-TRANSFER-ACCOUNT': 'New Payee',
                'EDIT-TRANSFER-ACCOUNT': 'Edit Payee',
                'TRANSFER-ACCOUNT-DETAIL': 'Payee Detail'
            },
            'LIST-COLUMNS': {
                'FRIENDLY-NAME': 'Friendly Name',
                'ACCOUNT-NO': 'Account No',
                'ACCOUNT-HOLDER-NAME': 'Account Holder Name',
                'ACCOUNT-HOLDER-FIRST-NAME': 'Account Holder First Name',
                'ACCOUNT-HOLDER-LAST-NAME': 'Account Holder Last Name',
                'IBAN': 'Iban',
                'BANK-NAME': 'Bank Name',
                'TENANT': 'Tenant',
                'IS-ACTIVE': 'Is Active',
                'OWNER': 'Owner'
            }
        },
        'TIMEZONE': {
            'IRAN': 'Iran Standard Time',
            'UTC': 'UTC',
            'PLACE_HOLDER': 'Select Time Zone'
        },
        'BANK-LOGIN': {
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            },
            'LIST-COLUMNS': {
                'FRIENDLY-NAME': 'Friendly Name',
                'BANK-NAME': 'Bank Name',
                'TENANT': 'Tenant',
                'USERNAME': 'Web Banking Username',
                'PASSWORD': 'Web Banking Password',
                'MOBILEUSERNAME': 'Mobile Banking Username',
                'MOBILEPASSWORD': 'Mobile Banking Password',
                'OWNER': 'Owner',
                'BANK': 'Bank',
                'STATUS': 'Verification Status',
                'MANAGE-ACCOUNTS': 'Manage Accounts',
                'EDIT': 'Edit Bank Login',
                'IS-BLOCKED': 'Login Status',
                'ACCOUNT-NO': 'Account Number',
                'LOAD_PREVIOUS_STATEMENTS': 'Load Previous Statements',
                'DEACTIVATE': 'Deactivate',
                'ACTIVATE': 'Activate',
                'DELETE': 'Delete',
                'LOGIN-TYPE': 'Login Type',
                'BLOCK-CARDS': 'Block Cards',
                'SECOND-PASSWORD': 'Second Password',
                'SHOW-PASSWORD': 'Show Password',
                'LAST-PASSWORD-CHANGED-DATE': 'Last Password Changed Date',
                'MOBILENUMBER': 'Mobile Number',
                'QRREGISTER': 'QR code register',
                'QRREGISTERSTATUS': 'QR code register status',
                'QRCODEOTP': 'QR Code OTP',
                'LOGINDEVICESTATUS': 'Login device status',
                'SHOW-LOGINDEVICESTATUS': 'Show device status',
                'GENERATE-OTP': 'Generate OTP',
                'EMAIL-ADDRESS':'Email Address',
                'EMAIL-PASSWORD': 'Email Password',
                'ISMOBILELOGIN': 'Mobile Login',
                'OTP': 'OTP',
                'PROCESSCOUNT-IN-24HRS': 'Process count in 24 Hrs',
                'SWITCH-BANK-CONNECTION-PROGRAM': 'Switch Bank Connection Program'
            },
            'GENERAL': {
                'BANK-LOGINS': 'Bank Logins',
                'LOGINS-DEVICE-STATUS': 'Logins device status',
                'CREATE-BANK-LOGIN': 'Create Bank Login',
                'NEW-BANK-LOGIN': 'New Bank Login',
                'EDIT-BANK-LOGIN': 'Edit Bank Login',
                'ADD-BANK-LOGIN': 'Add Bank Login',
                'SAVE': 'Save',
                'CHANGE-LOGIN-INFORMATION': 'Change Bank Login Information',
                'BLOCKED': 'Blocked',
                'NOT-BLOCKED': 'Not Blocked',
                'DEACTIVATE-DIALOG': 'Are you sure deactivating this login ?',
                'ACTIVATE-DIALOG': 'Are you sure activating this login ?',
                'DELETE-DIALOG': 'Are you sure deleting this login?',
                'CARD-TO-CARD': 'Card To Card',
                'WITHDRAWAL': 'Withdrawal',
                'BLOCKED-CARDS': 'Blocked Cards',
                'TRANSFER-ONLY': 'Transfer Only',
                'INFO': 'Info',
                'CURRENT-PASSWORD': 'Current password is {{password}}',                
                'CREATE-REGISTRATION': 'Create registration',
                'GET-SMS': 'Get SMS',
                'GET-DEVICE-STATUS': 'Get device status',
                'DEVICE-OTP': 'One Time Passowrd is {{otp}}',
                'DEVICE-NOT-REGISTERED': 'Device is not registered.',
                'WRONG-EMAIL-ADDRESS-PROVIDER': 'Only gmail and yahoo mail addresses are supported',
                'USERNAME-REQUIRED': 'Please enter Username corresponding to password',
                'PASSWORD-REQUIRED': 'Please enter Password corresponding to username',                
                'LOGIN-CREDENTIAL-REQUIRED': 'Any one (web/mobile) login credential required',
                'BANK-CONNECTION-PROGRAM': 'Login switched to {{bankConnectionProgram}}'
            },
            'STATUS': {
                'WAITING-INFORMATION': 'Waiting Information',
                'SUCCESS': 'Success',
                'FAIL': 'Failed',
                'FAILED': 'Login is failed',
                'INFORMATION-WAITING-FROM-SERVER': 'Login status information is being awaited from server',
                'WAITING-APPROVE': 'Login approved by PardakhtPay. Please check the account number and approve your login information',
                'QR-CODE-RESOLVING': 'QR registration is in progress',
                'SMS-WAITING': 'Waiting for sms',
                'QR-REGISTRATION-FAILED': 'QR code registration failed',
                'QR-REGISTRATION-COMPLETED': 'QR code registration completed',
                'PLEASE-REQUEST-REGISTRATION': 'Please request registration',
                'DEVICE_ACTIVE': 'Active',
                'DEVICE_INACTIVE': 'InActive',
                'DEVICE_ERROR': 'Error',
                'DEVICE_NOTCONFIGURED' : 'Mobile number not configured'
            }
        },
        'USER': {
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            },
            'LIST-COLUMNS': {
                'TENANT': 'Tenant',
                'USERNAME': 'Username',
                'FIRST-NAME': 'First Name',
                'LAST-NAME': 'Last Name',
                'EMAIL': 'E-mail',
                'PASSWORD': 'Password'
            },
            'GENERAL': {
                'USERS': 'Users',
                'CREATE-USER': 'Create User',
                'NEW-USER': 'New User',
                'EDIT-USER': 'Edit User',
                'ADD-USER': 'Add User',
                'SAVE': 'Save User'
            }
        },
        'BANK-STATEMENT': {
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            },
            'LIST-COLUMNS': {
                'TENANT': 'Tenant',
                'TRANSACTION-NO': 'Transaction No',
                'CHECK-NO': 'Check No',
                'DATE': 'Date',
                'DEBIT': 'Debit',
                'CREDIT': 'Credit',
                'BALANCE': 'Balance',
                'DESCRIPTION': 'Description',
                'USED-DATE': 'Used Date',
                'NOTES': 'Notes',
                'ACCOUNT-NO': 'Account No',
                'WITHDRAWAL-ID': 'Withdrawal ID'
            },
            'GENERAL': {
                'BANK-STATEMENTS': 'Bank Statements',
                'REFRESH': 'Refresh',
                'LOGIN-PLACE-HOLDER': 'Selected Login',
                'ACCOUNT-PLACE-HOLDER': 'Selected Account',
                'AMOUNT': 'Amount',
                'TYPE': 'Type',
                'UNCONFIRMED': 'Unconfirmed',
                'INCLUDE-DELETED-LOGINS': 'Include Deleted Logins'
            }
        },
        'TENANT-URL-CONFIG': {
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            },
            'LIST-COLUMNS': {
                'TENANT': 'Tenant',
                'MERCHANT': 'Merchant',
                'URL': 'Url',
                'IS-PAYMENT-URL': 'Is Payment Url',
                'IS-SERVICE-URL': 'Is Service Url'
            },
            'GENERAL': {
                'TENANT-URL-CONFIG-MANAGEMENT': 'Tenant Url Configuration Management',
                'TENANT-URL-CONFIG': 'Tenant Url Configuration',
                'SAVE': 'Save',
                'NEW_TENANT-URL-CONFIG': 'New Configuration',
                'CREATE-TENANT-URL-CONFIG': 'Create Configuration',
                'EDIT_TENANT-URL-CONFIG': 'Edit Tenant Url Configuration',
                'ALL': 'All',
                'ADD-TENANT-URL-CONFIG': 'Add'
            }
        },
        'AUTO-TRANSFER': {
            'GENERAL': {
                'TRANSFERS': 'Auto Transfers',
                'REFRESH': 'Refresh'
            },
            'LIST-COLUMNS': {
                'AMOUNT': 'Amount',
                'DATE': 'Transfer Request Date',
                'TRANSFERRED_DATE': 'Transferred Date',
                'STATUS': 'Status',
                'TENANT': 'Tenant',
                'FROM-ACCOUNT-NO': 'From Account No',
                'TO-ACCOUNT-NO': 'To Account No',
                'IS-CANCELLED': 'Is Cancelled',
                'CANCEL-DATE': 'Cancel Date',
                'LOGIN-NAME': 'Login Name',
                'TRANSFER-REQUEST-ID': 'Transfer Request Id'
            },
            'LIST': {
                'NO-DATA': 'No data to display',
                'TOTAL': 'total',
                'LOADING-DATA': 'Loading data...'
            }
        },
        'CARD-TO-CARD-ACCOUNT-GROUP': {
            'GENERAL': {
                'CARD-TO-CARD-ACCOUNT-GROUPS': 'Bank Account Groups',
                'SAVE': 'Save',
                'CREATE-GROUP': 'Create Group',
                'ACTIVE': 'Active',
                'RESERVED': 'Reserved',
                'BLOCKED': 'Blocked',
                'SELECT-ACCOUNT': 'Select a bank account to add',
                'HIDE-CARD-NUMBER': 'Hide Card Number',
                'DORMANT': 'Dormant',
                'PAUSED': 'Paused'
            },
            'LIST-COLUMNS': {
                'NAME': 'Name',
                'OWNER': 'Owner',
                'STATUS': 'Status'
            }
        },
        'MERCHANT-CUSTOMER': {
            'GENERAL': {
                'MERCHANT-CUSTOMERS': 'Customers',
                'BASIC-INFORMATION': 'Basic Information',
                'SAVE': 'Save',
                'DOWNLOAD' : 'Download Phone Numbers'
            },
            'LIST-COLUMNS': {
                'USER-ID': 'User Id',
                'WEBSITE-NAME': 'Website Name',
                'TOTAL-WITHDRAW': 'Total Withdrawal',
                'TOTAL-DEPOSIT': 'Total Deposit',
                'REGISTER-DATE': 'Register Date',
                'WITHDRAW-NUMBER': 'Withdraw Number',
                'DEPOSIT-NUMBER': 'Deposit Number',
                'ACTIVITY-SCORE': 'Activity Score',
                'GROUP-NAME': 'Group Name',
                'LAST-ACTIVITY-DATE': 'Last Activity Date',
                'TOTAL-TRANSACTION-COUNT': 'Total Deposit Count',
                'TOTAL-COMPLETED-TRANSACTION-COUNT': 'Total Completed Deposit Count',
                'TOTAL-DEPOSIT-AMOUNT': 'Total Deposit Amount',
                'TOTAL-WITHDRAWAL-COUNT': 'Total Withdrawal Count',
                'TOTAL-COMPLETED-WITHDRAWAL-COUNT': 'Total Completed Withdrawal Count',
                'TOTAL-WITHDRAWAL-AMOUNT': 'Total Withdrawal Amount',
                'TOTAL-SPORTBOOK-AMOUNT': 'Total Sportbook Amount',
                'TOTAL-SPORTBOOK-COUNT': 'Total Sportbook Count',
                'TOTAL-CASINO-AMOUNT': 'Total Casino Amount',
                'TOTAL-CASINO-COUNT': 'Total Casino Count',
                'PHONE-NUMBER': 'Phone Number',
                'PHONE-NUMBER-RELATED-CUSTOMERS': 'Phone Number Related Customers',
                'DIFFERENT-CARD-NUMBER-COUNT': 'Different Card Number Count',
                'USED-CARDS': 'Used Cards',
                'RELATED-CUSTOMERS': 'Related Customers',
                'RELATION-TYPE': 'Relation Type',
                'SAME-DEVICE': 'Same Device',
                'SAME-PHONE-NUMBER': 'Same Phone Number',
                'SAME-CARD-NUMBER': 'Same Card Number',
                'DEVICE-RELATED-CUSTOMERS': 'Device Related Customers',
                'CARD-NUMBER-RELATED-CUSTOMERS': 'Card Number Related Customers',
                'REMOVE-REGISTERED-PHONE': 'Remove Registered Phone Number',
                'PHONENUMBERS': 'Registered Phone Numbers'
            }
        },
        'USER-SEGMENT': {
            'GENERAL': {
                'USER-SEGMENT-GROUPS': 'User Segment Groups',
                'CREATE-GROUP': 'Create Group',
                'SAVE': 'Save',
                'SELECT-TYPE': 'Select Type',
                'RULES': 'Rules'
            },
            'LIST-COLUMNS': {
                'OWNER': 'Owner',
                'NAME': 'Name',
                'ORDER': 'Order',
                'VALUE': 'Value',
                'COMPARE-TYPE': 'Compare Type',
                'IS-DEFAULT': 'Is Default',
                'GROUP': 'User Segment Group',
                'PRIORITY': 'Priority'
            },
            'TYPES': {
                'TOTAL-SUCCESSFUL-TRANSACTION-COUNT': 'Total Successful Deposit Count',
                'TOTAL-SUCCESSFUL-TRANSACTION-AMOUNT': 'Total Successful Deposit Amount',
                'TOTAL-UNPAID-TRANSACTION-COUNT': 'Total Unpaid Deposit Count',
                'TOTAL-EXPIRED-TRANSACTION-COUNT': 'Total Expired Deposit Count',
                'TOTAL-WITHDRAWAL-COUNT-PARDAKHTPAY': 'Total Withdrawal Count - PardakhtPay',
                'TOTAL-WITHDRAWAL-COUNT-MERCHANT': 'Total Withdrawal Count -Merchant',
                'TOTAL-WITHDRAWAL-AMOUNT-PARDAKHTPAY': 'Total Withdrawal Amount - PardakhtPay',
                'TOTAL-WITHDRAWAL-AMOUNT-MERCHANT': 'Total Withdrawal Amount - Merchant',
                'TOTAL-DEPOSIT-COUNT-MERCHANT': 'Total Deposit Count - Merchant',
                'TOTAL-DEPOSIT-AMOUNT-MERCHANT': 'Total Deposit Amount - Merchant',
                'REGISTRATION-DATE': 'Day(s) Since Registration',
                'GROUP-NAME': 'Group Name',
                'LAST-ACITIVITY-DATE': 'Day(s) Since Last Activity',
                'ACTIVITY-SCORE': 'Activity Score',
                'WEBSITE-NAME': 'Website Name',
                'TOTAL-SPORTBOOK-AMOUNT': 'Total Sportbook Amount',
                'TOTAL-SPORTBOOK-COUNT': 'Total Sportbook Count',
                'TOTAL-CASINO-AMOUNT': 'Total Casino Amount',
                'TOTAL-CASINO-COUNT': 'Total Casino Count'
            },
            'COMPARE-TYPES': {
                'LESS-THAN': '<',
                'LESS-THAN-AND-EQUAL': '<=',
                'EQUALS': '==',
                'NOT-EQUAL': '<>',
                'MORE-THAN-AND-EQUAL': '>=',
                'MORE-THAN': '>'
            }
        },
        'APPLICATION-SETTINGS': {
            'GENERAL': {
                'SAVE': 'Save',
                'URL': 'Url',
                'API-KEY': 'Api Key',
                'SMS-API-KEY': 'Sms Api Key',
                'SMS-SECRET-KEY': 'Sms Secret Key',
                'TEMPLATE-ID': 'Template Id',
                'EXPIRE-SECONDS': 'Expire Seconds',
                'USE-SMS-CONFIRMATION': 'Use Sms Confirmation',
                'APPLICATION-SETTINGS': 'Application Settings',
                'SMS-CONFIGURATION': 'Sms Configuration',
                'MALICIOUS-CUSTOMERS': 'Malicious Customers',
                'FAKE-CARD-NUMBER': 'Fake Card Number',
                'FAKE-CARD-HOLDER-NAME': 'Fake Card Holder Name',
                'BANK-ACCOUNTS': 'Bank Accounts',
                'ACCOUNT-BLOCK-LIMIT': 'Account Block Limit',
                'USE-SAME-ACCOUNT-NUMBER-FOR-CUSTOMER': 'Use Previous Account Number For Withdrawals',
                'MAXIMUM-TRY-COUNT-TO-REGISTER-DEVICE': 'Maximum Try Count To Register Device',
                'SAVE-DEVICE': 'Save As a Device',
                'PARDAKHTPAL': 'PardakhtPal',
                'USE-ASANPARDAKHT-API': 'Use Asanpardakht Api',
                'ASANPARDAKHT-ORDER': 'Asanpardakht Api Order',
                'USE-HAMRAH-CARD-API': 'Hamrah Card Api',
                'HAMRAH-CARD-ORDER': 'Hamrah Card Order',
                'HAMRAH-CARD-TRY-COUNT': 'Hamrah Card Try Count',
                'USE-ASANPARDAKHT-API-FOR-WITHDRAWALS': 'Use Asanpardakht For Withdrawals',
                'USE-HAMRAH-CARD-API-FOR-WITHDRAWALS': 'Use Hamrah Card For Withdrawals',
                'USE-SEKEH-API': 'Use Sekeh Api',
                'SEKEH-ORDER': 'Sekeh Api Order',
                'SEKEH-TRY-COUNT': 'Sekeh Api Try Count',
                'USE-SEKEH-API-FOR-WITHDRAWALS': 'Use Sekeh Api For Withdrawls',
                'ASANPARDAKHT-WITHDRAW-ORDER': 'Asanpardakht Withdraw Order',
                'HAMRAH-CARD-WITHDRAW-ORDER': 'Hamrah Card Withdraw Order',
                'SEKEH-WITHDRAW-ORDER': 'Sekeh Withdraw Order',
                'USE-SES-API': 'Use Ses Api',
                'SES-ORDER': 'Ses Api Order',
                'SES-TRY-COUNT': 'Ses Api Try Count',
                'USE-SES-API-FOR-WITHDRAWALS': 'Use Ses Api For Withdrawls',
                'SES-WITHDRAW-ORDER': 'Ses Withdraw Order',
                'SES-LIMIT': 'Ses Transaction Limit',
                'DEFAULT-REGISTRATION': 'Default Device Registration API',
                'WAIT-AMOUNT': 'Perform withdrawals with order',
                'SETTINGS': 'Settings',
                'USE-SADAT-PSP-API': 'Use Sadat Psp Api',
                'SADAT-PSP-ORDER': 'Sadat Psp Order',
                'SADAT-PSP-WITHDRAW-ORDER': 'Sadat Psp Withdraw Order',
                'SADAT-PSP-TRY-COUNT': 'Sadat Psp Try Count',
                'USE-SADAT-PSP-API-FOR-WITHDRAWALS': 'Use Sadat Psp For Withdrawals',
                'USE-MY-DIGI-API': 'Use My Digi API',
                'MY-DIGI-ORDER': 'My Digi Order',
                'MY-DIGI-WITHDRAW-ORDER': 'My Digi Withdrawl Order',
                'MY-DIGI-TRY-COUNT': 'My Digi Try Count',
                'USE-MY-DIGI-API-FOR-WITHDRAWALS': 'Use My Digi For Withdrawals',
                'TEST-ACCOUNTS': 'Test Accounts',
                'USE-PAYMENT780-API': 'Use Payment780 Api',
                'PAYMENT780-ORDER': 'Payment780 Api Order',
                'PAYMENT780-TRY-COUNT': 'Payment780 Api Try Count',
                'USE-PAYMENT780-API-FOR-WITHDRAWALS': 'Use Payment780 Api For Withdrawls',
                'PAYMENT780-WITHDRAW-ORDER': 'Payment780 Withdraw Order'
            }
        },
        'RISKY-KEYWORDS': {
            'ADD-NEW-KEYWORD': 'Add new keyword',
            'IS-RISKY': 'Is Risky'
        },
        'MANUAL-TRANSFER': {
            'GENERAL': {
                'MANUAL-TRANSFERS': 'Manual Transfers',
                'ACCOUNT-PLACE-HOLDER': 'Selected Account',
                'REFRESH': 'Refresh',
                'ADD-MANUAL-TRANSFER': 'Create Manual Transfer',
                'EDIT-MANUAL-TRANSFER': 'Edit Manual Transfer',
                'SAVE-MANUAL-TRANSFER': 'Save',
                'CANCELATION-APPROVE': 'Are you sure do you want to cancel this transfer ?',
                'DETAILS': 'Details',
                'IMMEDIATE-APPROVE': 'Are you sure do you want to transfer money immediately ?',
                'RETRY-APPROVE': 'Are you sure do you want to retry this transfer ?',
                'COMPLETE-APPROVE': 'Are you sure do you want to set this transfer as completed ?',
                'CALLBACK-APPROVE': 'Are you sure do you want to send callback ?'
            },
            'TRANSFER-TYPES': {
                'TRANSFER-TYPES': 'Transfer Types',
                'NORMAL': 'Normal',
                'PAYA': 'Paya',
                'SATNA': 'Satna'
            },
            'LIST-COLUMNS': {
                'OWNER': 'Owner',
                'CREATE-DATE': 'Create Date',
                'TRANSFER-TYPE': 'Transfer Type',
                'AMOUNT': 'Amount',
                'FROM-ACCOUNT-NO': 'From Account No',
                'TO-ACCOUNT-NO': 'To Account No',
                'IBAN': 'Iban',
                'FIRST-NAME': 'First Name',
                'LAST-NAME': 'Last Name',
                'STATUS': 'Status',
                'PRIORITY': 'Priority',
                'PROCESSED-DATE': 'Processed Date',
                'CANCELLED-DATE': 'Cancelled Date',
                'IMMEDIATE-TRANSFER': 'Immediate Transfer',
                'SCHEDULED-DATE': 'Scheduled Date',
                'CANCEL': 'Cancel',
                'COMMENT': 'Comment',
                'PAYEE': 'Payee',
                'TRANSFER-WHOLE-AMOUNT': 'Transfer Whole Balance',
                'BALANCE': 'Balance',
                'RETRY': 'Retry',
                'COMPLETED': 'Completed',
                'WITHDRAWALCALLBACKTOMERCHANT': 'Callback'
               
            },
            'PRIORITIES': {
                'LOW': 'Low',
                'MEDIUM': 'Medium',
                'HIGH': 'High'
            },
            'STATUS': {
                'PENDING': 'Pending',
                'PROCESSING': 'Processing',
                'PARTIAL-SENT': 'Partial Sent',
                'SENT': 'Sent',
                'PARTIAL-COMPLETED': 'Partially Completed',
                'COMPLETED': 'Completed',
                'CANCELLED': 'Cancelled',
                'BLOCKED-ACCOUNT': 'Blocked Account',
                'DELETED-ACCOUNT': 'Deleted Account',
                'INSUFFICIENT-BALANCE-OR-DAILY-LIMIT': 'Insufficient Balance or Daily Limit'
            }
        },
        'TRANSFER-STATUS': {
            'NOT-SENT': 'Not Processed',
            'INCOMPLETE': 'Incomplete',
            'COMPLETE': 'Transfer Completed',
            'ACCOUNT-BALANCE-LOW': 'Account Balance Low',
            'LOWER-THAN-MINIMUM-LIMIT': 'Lower Than Minimum Limit',
            'HIGHER-THAN-MAXIMUM-LIMIT': 'Higher Than Maximum Limit',
            'INSUFFICIENT-DAILY-LIMIT': 'Insufficient Daily Limit',
            'INSUFFICIENT-MONTHLY-LIMIT': 'Insufficient Monthly Limit',
            'INVALID': 'Invalid',
            'PENDING': 'Transfer Pending',
            'INSUFFICIENT-TIME': 'Insufficient Time',
            'DETAIL-RECORDED': 'Detail Recorded',
            'REJECTED-DUE-TO-BLOCKED-ACCOUNT': 'Rejected Due To Blocked Account',
            'CANCELLED': 'Cancelled',
            'AWAITING-BANK-INFORMATION': 'Awaiting Bank Confirmation',
            'INSUFFICIENT-BALANCE': 'Insufficient Balance',
            'FAILED-FROM-BANK': 'Failed From Bank',
            'REFUND-FROM-BANK': 'Refund From Bank',
            'SUBMITTED': 'Request Submitted',
            'INVALID-IBAN-NUMBER': 'Invalid Iban Number',
            'COMPLETED-WITH-NO-RECEIPT': 'Completed With No Receipt',
            'FAILED-BECAUSE-OF-CREDENTIAL': 'Failed Because of Authentication',
            'DOWNLOADING-RECEIPT': 'Downloading Receipt',
            'TARGET-PASSWORD-REQUIRED': 'Target Password Required',
            'ACCOUNT-NUMBER-INVALID': 'Account Number Invalid',
            'ONE-TIME-PASSWORD-REQUIRED': 'One Time Password Required',
            'SECOND-PASSWORD-REQUIRED': 'Second Password Required'
        },
        'MOBILE-TRANSFER-DEVICE': {
            'GENERAL': {
                'DEVICES': 'Devices',
                'ADD-DEVICE': 'Add Device',
                'NEW-DEVICE': 'Add New',
                'SAVE': 'Save',
                'SEND-SMS-APPROVE': 'Are you sure do you want to send verification code ?',
                'SEND-VERIFICATION-CODE': 'Send Verification Code',
                'REMOVE-APPROVE': 'Are you sure do you want to remove this device ?'
            },
            'LIST-COLUMNS': {
                'PHONE-NUMBER': 'Phone Number',
                'STATUS': 'Status',
                'VERIFICATION-CODE-SEND-DATE': 'Verification Code Send Date',
                'VERIFIED-DATE': 'Verified Date',
                'EXTERNAL-ID': 'External Id',
                'EXTERNAL-STATUS': 'External Status',
                'IS-ACTIVE': 'Is Active',
                'SEND-SMS': 'Send Sms',
                'VERIFICATION-CODE': 'Verification Code',
                'CHECK-STATUS': 'Check Status',
                'REMOVE': 'Remove'
            },
            'STATUS': {
                'CREATED': 'Created',
                'VERIFICATION-CODE-SENDED': 'Verification Code Sent',
                'PHONE-NUMBER-VERIFIED': 'Verified',
                'REMOVED': 'Removed',
                'ERROR': 'Error',
                'UNKNOWN': 'Unknown'
            }
        },
        'MOBILE-TRANSFER-CARD-ACCOUNT': {
            'GENERAL': {
                'CARD-ACCOUNTS': 'Target Card Accounts',
                'ADD-CARD-ACCOUNT': 'Add Target Card Account',
                'SAVE': 'Save',
                'PAYMENT-PROVIDER': 'Payment Provider',
                'MERCHANT-ID': 'Merchant Id',
                'SAMANBANK': 'Saman Bank',
                'TITLE': 'Title',
                'THRESHOLD-AMOUNT':'Threshold Amount',
                'CARD-TO-CARD-ACCOUNT':'Card To Card Account',
                'MERCHANT-PASSWORD':'Merchant Password',
                'TERMINAL-ID':'Terminal Id(Username)'
            },
            'PROVIDERS': {
                '1': 'PardakhtPal',
                '2': 'Saman Bank',
                '3': 'Meli',
                '4':'Zarinpal',
                '5':'Mellat',
                '6':'Novin'
            }
        },
        'MOBILE-TRANSFER-CARD-ACCOUNT-GROUP': {
            'GENERAL': {
                'GROUPS': 'Target Card Groups'
            }
        },
        'BLOCKED-PHONE-NUMBER': {
            'GENERAL': {
                'BLOCKED-PHONE-NUMBERS': 'Blocked Phone Numbers',
                'ADD': 'Add'
            },
            'LIST-COLUMNS': {
                'PHONE-NUMBER': 'Phone Number'
            }
        },
        'REPORTS': {
            'BANK-STATEMENT-BREAKDOWN': 'Bank Statement Breakdown',
            'USER-SEGMENT-REPORT': 'User Segment Daily Report',
            'CURRENT-BALANCE': 'Current Balance',
            'DEPOSIT-WITHDRAWAL-BREAKDOWN': 'Deposit Withdrawal Breakdown',
            'REPORTS': 'Reports',
            'WITHDRAWAL-PAYMENT-BREAKDOWN': 'Withdrawal Payment Breakdown',
            'DEPOSIT-BREAKDOWN-BY-ACCOUNTNUMBER': 'Deposit Breakdown By Account Number'
        },
        'INVOICE-OWNER-SETTINGS': {
            'GENERAL': {
                'INVOICE-OWNER-SETTINGS': 'Owner Invoice Settings',
                'ADD': 'Add',
                'NEW': 'New',
                'EDIT': 'Edit',
                'SAVE': 'Save'
            },
            'LIST-COLUMNS': {
                'INVOICE-PERIOD': 'Invoice Period',
                'PARDAKHTPAY-DEPOSIT-RATE': 'PardakhtPay Deposit Rate',
                'PARDAKHTPAL-DEPOSIT-RATE': 'PardakhtPal Deposit Rate',
                'PARDAKHTPAL-WITHDRAWAL-RATE': 'PardakhtPal Withdrawal Rate',
                'WITHDRAWAL-RATE': 'Withdrawal Rate'
            },
            'PERIOD': {
                'DAILY': 'Daily',
                'WEEKLY': 'Weekly',
                'MONTHLY': 'Monthly',
            }
        },
        'INVOICE': {
            'GENERAL': {
                'INVOICES': 'Invoices',
                'PARDAKHTPAY-DEPOSIT': 'PardakhtPay Deposit',
                'PARDAKHTPAY-DEPOSIT-DETAIL': 'Deposits which are paid by PardakhtPay',
                'CARTIPAL-DEPOSIT': 'PardakhtPal Deposit',
                'CARTIPAL-DEPOSIT-DETAIL': 'Deposits which are paid by PardakhtPal',
                'PARDAKHTPAL-WITHDRAWAL': 'PardakhtPal Withdrawal',
                'PARDAKHTPAL-WITHDRAWAL-DETAIL': 'Withdrawals which are paid by PardakhtPal deposits',
                'WITHDRAWAL': 'Withdrawal',
                'WITHDRAWAL-DETAIL': 'Withdrawals which are paid by Bank Account',
                'INVOICE': 'Invoice',
                'SERVICE': 'Service',
                'UNIT-RATE': 'Rate',
                'TOTAL-AMOUNT': 'Total Amount',
                'TOTAL-COUNT': 'Total Count',
                'TOTAL': 'Total',
                'THANKS': 'Thanks for your business',
                'CLIENT': 'Client',
                'INVOICE-DATE': 'Invoice Date',
                'DUE-DATE': 'Due Date'
            },
            'LIST-COLUMNS': {
                'START-DATE': 'Start Date',
                'END-DATE': 'End Date'
            }
        },
        'INVOICE-PAYMENT': {
            'GENERAL': {
                'ADD': 'Add New Payment',
                'INVOICE-PAYMENTS': 'Invoice Payments'
            },
            'LIST-COLUMNS': {
                'PAYMENT-REFERENCE': 'Payment Reference',
                'CREATE-DATE': 'Created Date',
                'PAYMENT-DATE': 'Payment Date'
            }
        },
        'BLOCKED-CARD-NUMBER': {
            'GENERAL': {
                'BLOCKED-CARD-NUMBERS': 'Blocked Card Numbers',
                'ADD': 'Add'
            },
            'LIST-COLUMNS': {
                'CARD-NUMBER': 'Card Number'
            }
        }
    }
};
