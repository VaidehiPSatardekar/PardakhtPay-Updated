import { GenericHelper } from "../helpers/generic";

export class MobileTransferDevice {

    id: number;

    phoneNumber: string;

    verificationCode: string;

    status: number;

    verifyCodeSendDate: Date;

    verifiedDate: Date;

    externalId: number;

    externalStatus: string;

    tenantGuid: string;

    isActive: boolean;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class MobileTransferCardAccount {

    id: number;

    paymentProviderType: number;

    cardNumber: string;

    cardHolderName: string;

    merchantId: string;

    terminalId: string;

    merchantPassword: string;

    title: string;

    tenantGuid: string;

    ownerGuid: string;

    isDeleted: boolean;

    isActive: boolean;

    thresholdAmount: number;

    cardToCardAccountGuid: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class MobileTransferCardAccountGroup {

    id: number;

    tenantGuid: string;

    ownerGuid: string;

    isDeleted: boolean;

    name: string;

    isActive: boolean;

    items: MobileTransferCardAccountGroupItem[];

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class MobileTransferCardAccountGroupItem {

    id: number;

    groupId: number;

    itemId: number;

    status: number;

    userSegmentGroups: number[];

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export enum MobileTransferCardAccountGroupItemStatus {
    Active = 1,
    Dormant = 2,
    Blocked = 3
}

export enum MobileTransferDeviceStatus {
    Created = 1,
    VerifyCodeSended = 2,
    PhoneNumberVerified = 3,
    Removed = 4,
    Error = 5,
    Unkown = 6
}

export enum PaymentProviderTypes {
    Cartipay = 1,
    Cartipal = 2,
    Saman = 3,
    Meli = 4,
    Zarinpal = 5,
    Mellat = 6,
    Novin = 7
}

export const MobileTransferDeviceStatuses = [
    {
        value: MobileTransferDeviceStatus.Created,
        key : 'MOBILE-TRANSFER-DEVICE.STATUS.CREATED'
    },
    {
        value: MobileTransferDeviceStatus.VerifyCodeSended,
        key: 'MOBILE-TRANSFER-DEVICE.STATUS.VERIFICATION-CODE-SENDED'
    },
    {
        value: MobileTransferDeviceStatus.PhoneNumberVerified,
        key: 'MOBILE-TRANSFER-DEVICE.STATUS.PHONE-NUMBER-VERIFIED'
    },
    {
        value: MobileTransferDeviceStatus.Removed,
        key: 'MOBILE-TRANSFER-DEVICE.STATUS.REMOVED'
    },
    {
        value: MobileTransferDeviceStatus.Error,
        key: 'MOBILE-TRANSFER-DEVICE.STATUS.ERROR'
    },
    {
        value: MobileTransferDeviceStatus.Unkown,
        key: 'MOBILE-TRANSFER-DEVICE.STATUS.UNKNOWN'
    }
];

export const PaymentProviders = [
    {
        value: PaymentProviderTypes.Cartipal,
        key : 'MOBILE-TRANSFER-CARD-ACCOUNT.PROVIDERS.1'
    },
    {
        value:PaymentProviderTypes.Saman,
        key: 'MOBILE-TRANSFER-CARD-ACCOUNT.PROVIDERS.2'
    },
    {
        value: PaymentProviderTypes.Meli,
        key: 'MOBILE-TRANSFER-CARD-ACCOUNT.PROVIDERS.3'
    },
    {
        value: PaymentProviderTypes.Zarinpal,
        key: 'MOBILE-TRANSFER-CARD-ACCOUNT.PROVIDERS.4'
    },
    {
        value: PaymentProviderTypes.Mellat,
        key: 'MOBILE-TRANSFER-CARD-ACCOUNT.PROVIDERS.5'
    },
    {
        value: PaymentProviderTypes.Novin,
        key: 'MOBILE-TRANSFER-CARD-ACCOUNT.PROVIDERS.6'
    }
];