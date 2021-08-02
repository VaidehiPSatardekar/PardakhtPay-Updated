import { GenericHelper } from "../helpers/generic";

export class CardToCardAccountGroup {
    id: number;
    name: string;
    tenantGuid: string;
    ownerGuid: string;
    accounts: string;

    items: CardToCardAccountGroupItem[];

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class CardToCardAccountGroupItem {

    id: number;

    cardToCardAccountGroupId: number;

    cardToCardAccountId: number;

    status: number;

    loginType: number;

    allowCardToCard: boolean;

    allowWithdrawal: boolean;

    hideCardNumber: boolean;

    userSegmentGroups: number[];
}