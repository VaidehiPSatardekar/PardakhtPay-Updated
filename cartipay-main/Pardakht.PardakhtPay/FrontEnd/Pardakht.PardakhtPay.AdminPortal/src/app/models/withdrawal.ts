import { ListSearchArgs, AgGridSearchArgs } from "./types";
import { GenericHelper } from "../helpers/generic";

export class Withdrawal {
    id: number;
    bankLoginId: number;
    bankAccountId: number;
    amount: number;
    fromAccountNumber: string;
    toAccountNumber: string;
    toIbanNumber: string;
    priority: number;
    firstName: string;
    lastName: string;
    transferRequestGuid: string;
    transferNotes: string;
    transferStatus: number;
    transferType: string;
    transferDate: Date;
    transferRequestDate: Date;
    tenantGuid: string;
    transferAccountId: number;
    reference: string;
    trackingNumber: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class WithdrawalCreate {
    amount: number;
    priority: number;
    toIbanNumber: string;
    firstName: string;
    lastName: string;
    transferRequestGuid: string;
    transferNotes: string;
    transferStatus: number;
    transferStatusDescription: string;
    transferType: string;
    tenantGuid: string;
    transferAccountId: number;
    userId: string;
    websiteName: string;
    reference: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class WithdrawalSearch {
    id: number;
    merchantTitle: string;
    amount: number;
    fromAccountNumber: string;
    toAccountNumber: string;
    toIbanNumber: string;
    cardNumber: string;
    priority: number;
    firstName: string;
    lastName: string;
    transferRequestGuid: string;
    transferNotes: string;
    transferStatus: number;
    transferStatusDescription: string;
    transferType: string;
    transferDateStr: string;
    transferRequestDateStr: string;
    expectedTransferDateStr: string;
    cancelDateStr: string;
    tenantGuid: string;
    transferAccountId: number;
    userId: string;
    websiteName: string;
    reference: string;
    friendlyName: string;
    description: string;
    merchantCustomerId: number;
    bankStatementItemId: number;
    transactionId: number;
    withdrawalProcessType: number;
    cardToCardTryCount: number;
    updateDate: Date;
    remainingAmount: number;
    pendingApprovalAmount: number;
    isLoginBlocked: boolean;
    cardHolderName: string;
}

export class WithdrawalTransferHistory {
    id: number;
    withdrawalId: number;
    transferId: number;
    transferNotes: string;
    amount: number;
    transferStatus: number;
    transferStatusDescription: number;
    requestedDate: Date;
    lastCheckDate: Date;
}

export class WithdrawalReceipt {
    data: any;
    contentType: string;
    fileName: string;
}

export class WithdrawalSearchArgs extends AgGridSearchArgs {

    status: number;

    filterModel: any;

    merchantCustomerId: number;

    merchants: number[];

    constructor(data?: any) {
        super();
        GenericHelper.populateData(this, data);
    }
}

export enum WithdrawalProcessType {
    Transfer = 1,
    CardToCard = 2,
    Both = 3
}

export const WithdrawalProcessTypes = [
    {
        value: WithdrawalProcessType.Transfer,
        key: 'WITHDRAWAL.PROCESS_TYPES.TRANSFER'
    },
    {
        value: WithdrawalProcessType.CardToCard,
        key: 'WITHDRAWAL.PROCESS_TYPES.CARD-TO-CARD'
    },
    {
        value: WithdrawalProcessType.Both,
        key: 'WITHDRAWAL.PROCESS_TYPES.BOTH'
    }
];

export const WithdrawalStatuses = [
    {
        status: 1,
        translate: 'WITHDRAWAL.STATUSES.PENDING'
    },
    {
        status: 2,
        translate: 'WITHDRAWAL.STATUSES.CANCELLED-BY-USER'
    },
    {
        status: 3,
        translate: 'WITHDRAWAL.STATUSES.CANCELLED-BY-SYSTEM'
    },
    {
        status: 4,
        translate: 'WITHDRAWAL.STATUSES.CONFIRMED'
    },
    {
        status: 5,
        translate: 'WITHDRAWAL.STATUSES.INSUFFICIENT-BALANCE'
    },
    {
        status: 6,
        translate: 'WITHDRAWAL.STATUSES.SENT'
    },
    {
        status: 7,
        translate: 'WITHDRAWAL.STATUSES.REFUNDED'
    },
    {
        status: 8,
        translate: 'WITHDRAWAL.STATUSES.PENDING-CARD-TO-CARD-CONFIRMATION'
    },
    {
        status: 9,
        translate: 'WITHDRAWAL.STATUSES.PARTIAL-PAID'
    }];

export enum WithdrawalStatusEnum {
    Pending = 1,
    CancelledByUser = 2,
    CancelledBySystem = 3,
    Confirmed = 4,
    PendingBalance = 5,
    Sent = 6,
    Refunded = 7,
    PendingCardToCardConfirmation = 8,
    PartialPaid = 9
}