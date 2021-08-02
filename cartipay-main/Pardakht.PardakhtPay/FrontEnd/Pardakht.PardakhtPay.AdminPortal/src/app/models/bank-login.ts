import { GenericHelper } from "../helpers/generic";

export class BankLogin {
    id: number;
    bankLoginId: number;
    tenantGuid: string;
    loginGuid: string;
    username: string;
    mobileusername: string;
    mobilepassword: string;
    bankName: string;
    bankId: number;
    friendlyName: string;
    ownerGuid: string;
    status: number;
    isBlocked: boolean;
    accounts: string[];
    loginRequestId: number;
    loginType: number;
    isSecondPasswordNeeded: boolean;
    isBlockCard: boolean;
    lastPasswordChangeDate: Date;
    qrRegistrationId: number;
    qrRegistrationStatus: string;
    qrRegistrationStatusId: number;
    loginDeviceStatusId: number;
    isMobileLogin: boolean;
    bankConnectionProgram: string;

    //accountNo: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class BankAccount {
    accountNo: string;
    accountGuid: string;
    loginGuid: string;
    friendlyName: string;
}

export enum BankLoginStatus {
    WaitingInformation = 1,
    Success = 2,
    Error = 3,
    WaitingApprovement = 4,
    AwaitingRegistration=5
}

export enum LoginDeviceStatuses {
    Active = 1,
    InActive = 2,
    MobileNotConfigured = 3,
    Error = 4
}

export class CreateLoginFromLoginRequestDTO {    
    loginRequestId: number;
    accountNumber: string;
    loadPreviousStatements: boolean;
    loginType: number;
    isBlockCard: boolean;
    secondPassword: string;
    mobileNumber: string;
    emailAddress: string;
    emailPassword: string;
    otp: number;
    processCountIn24Hrs: number;
    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class BankLoginUpdateDTO{
    username: string;
    password: string;
    mobileusername: string;
    mobilepassword: string;
    secondPassword: string;
    mobileNumber: string;
    emailAddress: string;
    emailPassword: string;
    processCountIn24Hrs: number;
}

export enum LoginTypeEnum {
    CardToCard = 1,
    TransferOnly = 2,
    InfoOnly = 3
}

export enum QRRegisterRequestStatus {
    Incomplete = 0,
    InProgress = 1,
    Complete = 2,
    Pending = 3,
    Failed = 4,
    SessionOut = 5,
    QrImageCaptured = 6
}

export class QrCodeRegistrationRequest{
    bankLoginId: number;
    otp: string;
}

export class RegisterLogin {
    loginRequestId: number;    
    otp: number;
    bankId: number;
    //accountNo: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export const LoginTypes = [
    {
        id: 1,
        translate: 'BANK-LOGIN.GENERAL.CARD-TO-CARD'
    },
    {
        id: 2,
        translate: 'BANK-LOGIN.GENERAL.TRANSFER-ONLY'
    },
    {
        id: 3,
        translate: 'BANK-LOGIN.GENERAL.INFO'
    }];