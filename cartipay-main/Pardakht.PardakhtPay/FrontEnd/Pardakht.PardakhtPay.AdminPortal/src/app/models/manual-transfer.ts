import { AgGridSearchArgs } from "./types";
import { GenericHelper } from "../helpers/generic";

export class ManualTransfer {

    id: number;

    bankLoginId: number;

    bankAccountId: number;

    creationDateStr: string;

    tenantGuid: string;

    ownerGuid: string;

    cardToCardAccountId: number;

    cardToCardAccountIds: number[];

    accountGuid: string;

    transferType: number;

    amount: number;

    transferAccountId: number;

    toAccountNo: string;

    iban: string;

    firstName: string;

    lastName: string;

    status: number;

    priority: number;

    processedDateStr: string;

    cancelledDateStr: string;

    immediateTransfer: boolean;

    expectedTransferDate: Date;

    expectedTransferDateStr: string;

    comment: string;

    creatorId: string;

    updaterId: string;

    cancellerId: string;

    transferWholeAmount: boolean;

    details: ManualTransferDetail[];

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class ManualTransferDetail {

    id: number;

    amount: number;

    transferGuid: string;

    transferId: number;

    transferNotes: string;

    trackingNumber: string;

    transferStatus: number;

    transferDate: Date;

    transferRequestDate: Date;

    bankStatementId: number;
}

export class ManualTransferSearchArgs extends AgGridSearchArgs {

    fromDate: Date;

    toDate: Date;

    toAccountNo: string;

    transferType: number;

    status: number;

    accountGuids: string[];
    
    filterModel: any;
}

export const TransferTypes = [{
    value: 1,
    key: 'MANUAL-TRANSFER.TRANSFER-TYPES.NORMAL'
},
{
    value: 2,
    key: 'MANUAL-TRANSFER.TRANSFER-TYPES.PAYA'
}, {
    value: 3,
    key: 'MANUAL-TRANSFER.TRANSFER-TYPES.SATNA'
}];

export const ManualTransferStatuses = [{
    value: 1,
    key: 'MANUAL-TRANSFER.STATUS.PENDING'
}, {
    value: 2,
    key: 'MANUAL-TRANSFER.STATUS.PROCESSING'
}, {
    value: 3,
    key: 'MANUAL-TRANSFER.STATUS.PARTIAL-SENT'
}, {
    value: 4,
    key: 'MANUAL-TRANSFER.STATUS.SENT'
}, {
    value: 5,
    key: 'MANUAL-TRANSFER.STATUS.PARTIAL-COMPLETED'
}, {
    value: 6,
    key: 'MANUAL-TRANSFER.STATUS.COMPLETED'
}, {
    value: 7,
    key: 'MANUAL-TRANSFER.STATUS.CANCELLED'
}, {
    value: 8,
    key: 'MANUAL-TRANSFER.STATUS.BLOCKED-ACCOUNT'
}, {
    value: 9,
    key: 'MANUAL-TRANSFER.STATUS.DELETED-ACCOUNT'
    }, {
        value: 10,
        key: 'MANUAL-TRANSFER.STATUS.INSUFFICIENT-BALANCE-OR-DAILY-LIMIT'
    }
];

export const TransferPriorities = [
    {
        value: 1,
        key: 'MANUAL-TRANSFER.PRIORITIES.LOW'
    }, {
        value: 2,
        key: 'MANUAL-TRANSFER.PRIORITIES.MEDIUM'
    }, {
        value: 3,
        key: 'MANUAL-TRANSFER.PRIORITIES.HIGH'
    }];

export enum TransferTypeEnum {
    Normal = 1,
    Paya = 2,
    Satna = 3
}