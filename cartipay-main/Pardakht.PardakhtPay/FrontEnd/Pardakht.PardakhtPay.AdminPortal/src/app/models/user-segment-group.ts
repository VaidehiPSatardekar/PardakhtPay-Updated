import { GenericHelper } from "../helpers/generic";

export class UserSegmentGroup {

    id: number;

    name: string;

    isActive: boolean;

    isDeleted: boolean;

    isDefault: boolean;

    isMalicious: boolean;

    createDate: Date;

    tenantGuid: string;

    ownerGuid: string;

    order: number;

    items: UserSegment[] = [];

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class UserSegment {

    id: number;

    userSegmentGroupId: number;

    userSegmentCompareTypeId: number;

    userSegmentTypeId: number;

    value: string;
}

export class RegisteredPhoneNumbers {
    phoneNumber: string;
}