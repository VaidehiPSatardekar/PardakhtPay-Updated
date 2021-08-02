export class ApplicationSettings {
    smsConfiguration: SmsServiceConfiguration;
    maliciousCustomerSettings: MaliciousCustomerSettings;
    bankAccountConfiguration: BankAccountConfiguration;
    mobileApiConfiguration: MobileApiConfiguration;
}

export class SmsServiceConfiguration {

    id: number;

    url: string;

    apiKey: string;

    smsApiKey: string;

    smsSecretKey: string;

    templateId: string;

    useSmsConfirmation: boolean;

    seconds: number;

    saveDevices: boolean;

    maximumTryCountForRegisteringDevice: number;
    
}

export class MaliciousCustomerSettings {
    id: number;
    fakeCardNumber: string;
    fakeCardHolderName: string;
}

export class BankAccountConfiguration {
    blockAccountLimit: number;
    testingAccounts: string;
    useSameWithdrawalAccountForCustomer: boolean;    
}

export class TransferStatusDescription {
    id: number;
    code: string;
    descriptionInEnglish: string;
    descriptionInFarsi: string;
}

export class MobileApiConfiguration {

    useAsanPardakhtApi: boolean;

    asandPardakhtApiOrder: number;

    asanpardakhtWithdrawalOrder: number;

    useAsanpardahkhtForWithdrawals: boolean;

    useHamrahCardApi: boolean;

    hamrahCardApiOrder: number;

    hamrahCardWithdrawalOrder: number;

    hamrahMaximumTryCount: number;

    useHamrahCardForWithdrawals:boolean;

    useSekeh: boolean;

    sekehApiOrder : boolean;

    sekehWithdrawalOrder: number;

    sekehMaximumTryCount : boolean;

    useSekehForWithdrawals : boolean;

    useSes: boolean;

    sesApiOrder: number;

    sesWithdrawalOrder: number;

    sesMaximumTryCount: number;

    useSesForWithdrawals: boolean;

    sesLimit: number;

    useSadadPsp: boolean;

    sadadPspOrder: number;

    useSadadPspForWithdrawals: boolean;

    sadadPspWithdrawalOrder: number;

    sadadPspMaxTryCount: number;

    deviceRegistrationApi: number;

    useMydigi: boolean;

    mydigiOrder: number;

    useMydigiForWithdrawals: boolean;

    mydigiWithdrawalOrder: number;

    mydigiMaxTryCount: number;

    usePayment780: boolean;

    payment780ApiOrder : boolean;

    payment780WithdrawalOrder: number;

    payment780MaximumTryCount : boolean;

    usePayment780ForWithdrawals : boolean;
}