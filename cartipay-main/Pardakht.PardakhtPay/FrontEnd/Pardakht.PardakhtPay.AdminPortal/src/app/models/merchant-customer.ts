import { AgGridSearchArgs } from "./types";

export class MerchantCustomer{

    id: number;

    tenantGuid: string;

    ownerGuid: string;

    merchantId: number;

    websiteName: string;

    userId: string;

    registerDate: Date;

    totalDeposit: number;

    totalWithdraw: number;

    depositNumber: string;

    withdrawNumber: number;

    activityScore: number;

    groupName: string;

    lastActivity: Date;

    cardToCardAccountId: number;

    totalTransactionCount: number;

    totalCompletedTransactionCount: number;

    totalDepositAmount: number;

    totalWithdrawalAmount: number;

    totalWithdrawalCount: number;

    totalCompletedWithdrawalCount: number;

    userSegmentGroupId: number;

    userTotalSportbook: number;

    userSportbookNumber: number;

    userTotalCasino: number;

    userCasinoNumber: number;

    phoneNumber: string;

    phoneNumberRelatedCustomers: string;

    differentCardNumberCount: number;

    deviceRelatedCustomers: string;

    cardNumberRelatedCustomers: string;

    phoneNumberIsBlocked: boolean;
}

export class MerchantRelation {
    id: number;
    websiteName: string;
    userId: string;
    merchantTitle: string;
    relationKey: string;
    value: string;
}

export class MerchantCustomerCardNumbers {
    cardNumber: string;
    cardHolderName: string;
    count: number;
}

export class MerchantCustomerSearchArgs extends AgGridSearchArgs {
    filterModel: any;
    websiteName: string;
    userId: string;
    phoneNumber: string;
    isDownload: boolean;
}

export class CustomerPhoneNumbers {
    data: any;
    contentType: string;
    fileName: string;
}