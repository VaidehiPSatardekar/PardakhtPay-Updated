import { GenericHelper } from "../helpers/generic";

export class CardToCardAccount {
    id: number;
    bankLoginId: number;
    bankAccountId: number;
    loginGuid: string;
    accountGuid: string;
    cardNumber: string;
    cardHolderName: string;
    safeAccountNumber: string;
    transferThreshold: number;
    isActive: boolean;
    isTransferThresholdActive: boolean;
    transferThresholdLimit: number;
    tenantGuid: string;
    ownerGuid: string;
    friendlyName: string;
    loginType: number;
    accountNo: string;
    switchOnLimit: boolean;
    switchIfHasReserveAccount: boolean;
    switchLimitAmount: number;
    accountType: number;
    switchCreditDailyLimit: number;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export enum AccountType{
    None = 0,
    Withdrawal = 1,
    Deposit = 2,
    Both = 3
}