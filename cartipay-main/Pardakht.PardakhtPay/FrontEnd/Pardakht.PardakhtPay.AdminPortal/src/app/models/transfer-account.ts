import { GenericHelper } from "../helpers/generic";

export class TransferAccount{
    id: number;
    tenantGuid: string;
    accountNo: string;
    //accountHolderName: string;
    accountHolderFirstName: string;
    accountHolderLastName: string;
    iban: string;
    friendlyName: string;
    isActive: boolean;
    ownerGuid: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}