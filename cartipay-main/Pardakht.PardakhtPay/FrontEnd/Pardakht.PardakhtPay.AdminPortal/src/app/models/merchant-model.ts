import { GenericHelper } from "../helpers/generic";

export class MerchantSummary {
    id: number;
    title: string;
    domainAddress: string;
    email: string;
    isActive: boolean;
    minimumTransactionAmount: number;
}

export class Merchant {
    id: number;
    title: string;
    domainAddress: string;
    email: string;
    isActive: boolean;
    apiKey: string;
    cardToCardAccountGroupId: number;
    mobileTransferAccountGroupId: number;
    tenantGuid: string;
    ownerGuid: string;
    minimumTransactionAmount: number;
    useCardtoCardPaymentForWithdrawal: boolean;
    allowPartialPaymentForWithdrawals: boolean;
}

export class MerchantCreate {
    id: number;
    title: string;
    domainAddress: string;
    email: string;
    isActive: boolean;
    apiKey: string;
    cardToCardAccountGroupId: number;
    mobileTransferAccountGroupId: number;
    tenantGuid: string;
    ownerGuid: string;
    minimumTransactionAmount: number;
    useCardtoCardPaymentForWithdrawal: boolean;
    allowPartialPaymentForWithdrawals: boolean;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class MerchantEdit {
    id: number;
    title: string;
    domainAddress: string;
    email: string;
    isActive: boolean;
    apiKey: string;
    cardToCardAccountGroupId: number;
    mobileTransferAccountGroupId: number;
    tenantGuid: string;
    ownerGuid: string;
    minimumTransactionAmount: number;
    useCardtoCardPaymentForWithdrawal: boolean;
    allowPartialPaymentForWithdrawals: boolean;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class MerchantBankAccount {
    id: number;
    merchantId: number;
    cardNumber: string;
    accountNumber: string;
    isActive: boolean;
    transferTreshold: number;
    businessAccountNumber: string;
    apiKey: string
}