import { GenericHelper } from "../helpers/generic";

export class Tenant {
    id: number;
    tenantGuid: string;
    tenantDomainPlatformMapGuid: string;
    tenantName: string;
    domainName: string;
    domainGuid: string;
    tenantStatus: number;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class TenantCreate {
    name: string;

    tenantAdminUsername: string;
    tenantAdminEmail: string;
    tenantAdminPassword: string;
    tenantAdminFirstName: string;
    tenantAdminLastName: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}