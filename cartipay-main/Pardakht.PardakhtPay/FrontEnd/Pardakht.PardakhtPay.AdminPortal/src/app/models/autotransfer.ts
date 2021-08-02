import { ListSearchArgs, AgGridSearchArgs } from "./types";
import { GenericHelper } from "../helpers/generic";

export class AutoTransfer {
    id: number;

    bankLoginId: number;

    bankAccountId: number;

    tenantGuid: string;

    ownerGuid: string;

    amount: number;

    status: number;

    statusDescription: string;

    requestId: number;

    requestGuid: string;

    cardToCardAccountId: number;

    accountGuid: string;

    transferFromAccount: string;

    transferToAccount: string;

    priority: number;

    transferRequestDateStr: string;

    transferredDateStr: string;

    cancelDateStr: string;

    isCancelled: boolean;

    friendlyName: string;
}

export class AutoTransferSearchArgs extends AgGridSearchArgs {

    tenants: string[];

    dateRange: string;

    constructor(data?: any) {
        super();
        GenericHelper.populateData(this, data);
    }
}