import { GenericHelper } from "../helpers/generic";

export class TenantUrlConfig {

    id: number;

    tenantGuid: string;

    merchantId: number;

    apiUrl: number;

    isServiceUrl: boolean;

    isPaymentUrl: boolean;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}