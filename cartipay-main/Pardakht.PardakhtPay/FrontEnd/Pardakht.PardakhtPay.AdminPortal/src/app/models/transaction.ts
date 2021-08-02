import { ListSearchArgs, AgGridSearchArgs } from "./types";
import { GenericHelper } from "../helpers/generic";

export class TransactionSearch {

    id: number;

    merchantTitle: string;

    transactionAmount: number;

    transactionDateStr: string;

    transferredDateStr: string;

    accountNumber: string;

    merchantCardNumber: string;

    customerCardNumber: string;

    bankNumber: string;

    status: number;

    description: string;

    userId: string;

    webSiteName: string;

    reference: string;

    userSegmentGroupId: number;

    merchantCustomerId: number;

    paymentType: number;

    externalId: number;

    externalMessage: string;

    withdrawalId: number;

    cardHolderName: string;

    processId: number;
    userSegmentGroupName: string;
}

export class TransactionSearchArgs extends AgGridSearchArgs {

    filterModel: any;

    statuses: string[];

    tenants: string[];

    paymentType: number;

    dateRange: string;

    token: string;

    merchantCustomerId: number;

    withdrawalId: number;

    constructor(data?: any) {
        super();
        GenericHelper.populateData(this, data);
    }
}

export enum PaymentType {
    CardToCard = 1,
    Mobile = 2,
    SamanBank = 3,
    Meli = 4,
    Zarinpal = 5,
    Mellat = 6,
    Novin = 7
}

export enum TransactionStatus {
    Started = 1,
    TokenValidatedFromWebSite = 2,
    WaitingConfirmation = 3,
    Completed = 4,
    Expired = 5,
    Cancelled = 6,
    Fraud = 7,
    Reversed = 8
}

export const PaymentTypes = [
    {
        value: 1,
        key: 'TRANSACTION.PAYMENT-TYPES.CARD-TO-CARD'
    },
    {
        value: 2,
        key: 'TRANSACTION.PAYMENT-TYPES.MOBILE'
    },
    {
        value: 3,
        key: 'TRANSACTION.PAYMENT-TYPES.SAMANBANK'
    },
    {
        value: 4,
        key: 'TRANSACTION.PAYMENT-TYPES.MELI'
    },
    {
        value: 5,
        key: 'TRANSACTION.PAYMENT-TYPES.ZARINPAL'
    },
    {
        value: 6,
        key: 'TRANSACTION.PAYMENT-TYPES.MELLAT'
    },
    {
        value: 7,
        key: 'TRANSACTION.PAYMENT-TYPES.NOVIN'
    }
]