import { GenericHelper } from "app/helpers/generic";
import { AgGridSearchArgs } from "./types";

export class DailyAccountingDTO {
    
    merchantTitle: string;

    count : number;

    amount: number;

    date: Date;

    accountNumber: string;

    cardNumber: string;

    cardHolderName: string;

    tenantGuid: string;

    tenantName: string;

    depositPercentage: number;

    withdrawalPercentage: number;
}

export class AccountingSearchArgs extends AgGridSearchArgs {

    groupType: number;
    startDate: string;
    endDate: string;

    constructor(data?: any) {
        super();
        GenericHelper.populateData(this, data);
    }
}

export enum AccountingGroupingType {

    Merchant = 1,
    Account = 2
}

export const AccountingGroupingTypes = [
    {
        value: AccountingGroupingType.Merchant,
        key: 'GENERAL.MERCHANT'
    },
    {
        value: AccountingGroupingType.Account,
        key: 'ACCOUNTING.LIST-COLUMNS.ACCOUNT_NUMBER'
    }
];