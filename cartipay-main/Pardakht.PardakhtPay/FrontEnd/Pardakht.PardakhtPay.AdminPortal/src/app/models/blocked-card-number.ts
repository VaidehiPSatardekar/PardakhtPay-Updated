import { GenericHelper } from "../helpers/generic";

export class BlockedCardNumber {
    id: number;
    cardNumber: string;
    blockedDate: Date;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}