export class BankStatementReportSearchArgs{
    timeZoneInfoId: string;
    accountGuids: string[];
    startDate: string;
    endDate: string;
}

export class UserSegmentReport {
    userSegmentId: number;
    ownerGuid: string;
    amount: number;
}

export class UserSegmentReportSearchArgs {
    timeZoneInfoId: string;
    startDate: string;
    endDate: string;
}

export class TenantBalance {
    bankName: string;
    amount: number;
    ownerGuid: string;
}

export class TransactionReportSearchArgs {
    timeZoneInfoId: string;
    startDate: string;
    endDate: string;
    startRow: number;
    endRow: number;
    sortColumn: string;
    sortOrder: string;
}

export class WithdrawalPaymentReportArgs {
    timeZoneInfoId: string;
    startDate: string;
    endDate: string;
}

export class TenantBalanceSearchArgs{
    accountGuids: string[];
}