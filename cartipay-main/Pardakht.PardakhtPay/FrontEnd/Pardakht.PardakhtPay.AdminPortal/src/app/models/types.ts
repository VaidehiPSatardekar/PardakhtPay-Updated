import { HttpHeaders, HttpParams } from '@angular/common/http';

export class ServiceResponse<T> {
    payload: T;
    success: boolean;
    message: string;
}

export enum PageDirection {
    LeftToRight = 'ltr',
    RightToLeft = 'rtl'
}

export interface Localization {
    pageDirection: PageDirection;
    languageCode: string;
    dictionary: { [key: string]: string };
}

export interface AuditLog {
    id: number;
    message: string;
    user: string;
    dateTime: Date;
    tenantId: number;
}

export interface IAuditLogForm {
    message: string;
}

export interface IHttpOptions {
    headers?: HttpHeaders | {
        [header: string]: string | string[];
    };
    observe?: 'body';
    params?: HttpParams | {
        [param: string]: string | string[];
    };
    reportProgress?: boolean;
    responseType?: 'json';
    withCredentials?: boolean;
}

export class ListSearchResponse<T> {
    items: T;
    links: LinkInfo;
    paging: PagingHeader;
}

// not yet implemented
export class LinkInfo {
    href: string;
    rel: string;
    method: string;
}

// tslint:disable-next-line:max-classes-per-file
export class PagingHeader {
    totalItems: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
}

// tslint:disable-next-line:max-classes-per-file
export class ListSearchArgs {
    filter: string;
    pageSize: number;
    pageNumber: number;
    sortColumn: string;
    sortOrder: string;
    timeZoneInfoId: string;
}

export class AgGridSearchArgs {

    filter: string;
    pageSize: number;
    startRow: number;
    endRow: number;
    sortColumn: string;
    sortOrder: string;
    timeZoneInfoId: string;
}

export const defaultColumnDefs = {
    filterParams: {
        suppressAndOrCondition: true,
        applyButton: true,
        clearButton: true
    }
}

export const TransferStatuses = [{
    value: -1,
    key: 'TRANSFER-STATUS.NOT-SENT'
}, {
    value: 0,
    key: 'TRANSFER-STATUS.INCOMPLETE'
},
{
    value: 1,
    key: 'TRANSFER-STATUS.COMPLETE'
},
{
    value: 2,
    key: 'TRANSFER-STATUS.ACCOUNT-BALANCE-LOW'
},
{
    value: 3,
    key: 'TRANSFER-STATUS.LOWER-THAN-MINIMUM-LIMIT'
},
{
    value: 4,
    key: 'TRANSFER-STATUS.HIGHER-THAN-MAXIMUM-LIMIT'
},
{
    value: 5,
    key: 'TRANSFER-STATUS.INSUFFICIENT-DAILY-LIMIT'
},
{
    value: 6,
    key: 'TRANSFER-STATUS.INSUFFICIENT-MONTHLY-LIMIT'
},
{
    value: 7,
    key: 'TRANSFER-STATUS.INVALID'
},
{
    value: 8,
    key: 'TRANSFER-STATUS.PENDING'
},
{
    value: 9,
    key: 'TRANSFER-STATUS.INSUFFICIENT-TIME'
},
{
    value: 10,
    key: 'TRANSFER-STATUS.DETAIL-RECORDED'
},
{
    value: 11,
    key: 'TRANSFER-STATUS.REJECTED-DUE-TO-BLOCKED-ACCOUNT'
},
{
    value: 12,
    key: 'TRANSFER-STATUS.CANCELLED'
},
{
    value: 13,
    key: 'TRANSFER-STATUS.AWAITING-BANK-INFORMATION'
},
{
    value: 14,
    key: 'TRANSFER-STATUS.INSUFFICIENT-BALANCE'
},
{
    value: 15,
    key: 'TRANSFER-STATUS.FAILED-FROM-BANK'
},
{
    value: 16,
    key: 'TRANSFER-STATUS.REFUND-FROM-BANK'
},
{
    value: 17,
    key: 'TRANSFER-STATUS.SUBMITTED'
},
{
    value: 18,
    key: 'TRANSFER-STATUS.INVALID-IBAN-NUMBER'
},
{
    value: 19,
    key: 'TRANSFER-STATUS.COMPLETED-WITH-NO-RECEIPT'
},
{
    value: 20,
    key: 'TRANSFER-STATUS.FAILED-BECAUSE-OF-CREDENTIAL'
},
{
    value: 21,
    key: 'TRANSFER-STATUS.DOWNLOADING-RECEIPT'
},
{
    value: 22,
    key: 'TRANSFER-STATUS.TARGET-PASSWORD-REQUIRED'
},
{
    value: 23,
    key: 'TRANSFER-STATUS.ACCOUNT-NUMBER-INVALID'
    },
    {
        value: 24,
        key: 'TRANSFER-STATUS.ONE-TIME-PASSWORD-REQUIRED'
    },
    {
        value: 25,
        key: 'TRANSFER-STATUS.SECOND-PASSWORD-REQUIRED'
    }
];

export enum TransferStatusEnum {
    NotSent = -1,
    Incomplete = 0,
    Complete = 1,
    AccountBalanceLow = 2,
    AccountBalanceLowerThanAccountTransferMinimumLimit = 3,
    AccountBalanceHigerThanAccountTransferMaximumLimit = 4,
    AccountBalanceHigherThanAccountTransferWithdrawalLimitForDay = 5,
    AccountBalanceHigherThanAccountTransferWithdrawalLimitForMonth = 6,
    Invalid = 7,
    Pending = 8,
    InSufficientTime = 9,
    DetailRecorded = 10,
    RejectedDueToBlokedAccount = 11,
    Cancelled = 12,
    AwaitingConfirmation = 13,
    InsufficientBalance = 14,
    FailedFromBank = 15,
    RefundFromBank = 16,
    BankSubmitted = 17,
    InvalidIbanNumber = 18,
    CompletedWithNoReceipt = 19
}