import { ListSearchArgs, AgGridSearchArgs } from "./types";

export class BankStatementItem {
    id: number;
    creationDate: Date;
    accountId: number;
    accountGuid: string;
    transactionNo: string;
    checkNo: string;
    transactionDateTimeStr: string;
    debit: number;
    credit: number;
    balance: number;
    description: string;
    insertDateTime: Date;
    usedDateStr: string;
    transactionId: number;
    isRisky: boolean;
    withdrawalId: number;
}

export class BankStatementItemSearchArgs extends AgGridSearchArgs {
    filterModel: any;
    statementItemType: number;
    accountGuids: string[];
    isRisky: boolean;
}