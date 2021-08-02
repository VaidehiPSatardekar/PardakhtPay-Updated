import { GenericHelper } from "../helpers/generic";

export class BlockedPhoneNumber {
    id: number;
    phoneNumber: string;
    blockedDate: Date;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}