import { GenericHelper } from './../../helpers/generic';
import { EnumData } from './tenant.model';

export class DomainAvailabeResult {
    domain: string;
    available: boolean;
    definitive: boolean;
    price: number;
    currency: string;
    period: number;
    useExistingDomain: boolean;
    message: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class SearchAvailableDomainSearch {
    address: string;
    isExistingDomain: boolean;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class DomainView {
    domainAddress: string;
    isPrimary: boolean;
    status: string;
    cloudFlareSwitch: string;
    cloudFlareRateLimit: number;
    registrationDate: Date;
    creator: string;
    tenantName: string;
    RebrandlyDomainId: string;
    zoneId: string;
    tenantGuid: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class Links {
    id: string;
    title: string;
    slashtag: string;
    destination: string;
    createdAt: Date;
    updatedAt: Date;
    shortUrl: string;
    isSelected: boolean;
    domain: Domain;
}

export class DomainEdit {
    tenancyName: string;
    isPrimary: string;
    urlTag: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class DomainAvailable {
    domains: string[];
    tenancyName: string;
    privateDomain: boolean;
    autoRenew: boolean;
    period: number;
    useExistingDomain: boolean;
    registrarName: string;
    isPrimary: boolean;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class CheckAvailabilty {
    address: string;
    isExistingDomain: boolean;
}

export class RebrandlyURLs {
    url: string[];
}

export class DomainPurchaseTerms {
    title: string;
    agreementKey: string;
    content: string;
}

export class DomainTermsRequest {
    topleveldomains: string;
    privacy: boolean;
}

export class Consent {
    agreementKeys: string[];
    agreedBy: string;
    agreedAt: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class DomainPurchase {
    domain: string;
    tenancyName: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class DomainPurchaseResponse {
    orderId: number;
    itemCount: number;
    total: number;
    currency: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class DomainAvailabilityCheckRequest {
    subDomain: string;
    domain: string;
    isTenantDomain: boolean;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class Domain {
    subDomain: string;
    domainAddress: string;
    domainGuid: string;
    isPrimary: boolean;
    domainStatus: DomainStatus;
    domainStatusStr: string;
    securityLevel: SecurityLevel;
    id: number;
    zoneId: string;
    createdAt: Date;
    updatedAt: Date;
    registrationDate: Date;
    registrationDateStr: Date;
    tenancyName: string;
    isPrivateDomain: boolean;
    isAutoRenew: boolean;
    purchasePeriod: number;
    isProviderDomain: boolean;
    isTenantDomain: boolean;
    isAffiliationDomain: boolean;
    isTenantContactInfo: boolean;
    tenantPlatformMapGuid: string;
    platformGuid: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class TenantDomain extends Domain {
    domainAddress: string;
    isPrimary: boolean;
    domainStatus: DomainStatus;
    id: number;
    tenancyName: string;
    privateDomain: boolean;
    autoRenew: boolean;
    period: number;
    useExistingDomain: boolean;
    registrarName: string;
    isProviderDomain: boolean;
    isTenantContactInfo: boolean;
    hasDomainChangedOnEdit: boolean;

    constructor(data?: any) {
        super();
        GenericHelper.populateData(this, data);
    }
}

export class CloudFlarePurgeRequest {
    tenantPlatformMapGuid: string;
    domainGuids: string[];

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class ChangeSecurityLevelRequest {
    tenantPlatformMapGuid: string;
    domainGuids: string[];
    securityLevel: SecurityLevel;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class AddUpdateDNSRecordRequest {
    domain: string;
    dnsRecord: DNSRecord;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class DeleteDNSRecordRequest extends AddUpdateDNSRecordRequest {
}

export class DNSRecord {
    id: string;
    type: string;
    name: string;
    content: string;
}

export enum DomainStatus {
    InActive = 0,
    Active = 1,
    Blocked = 2,
    Pending = 3,
    Disabled = 4
}

export enum SecurityLevel {
    essentially_off = 0,
    low = 1,
    medium = 2,
    high = 3,
    under_attack = 4
}

export enum CFRecordType {
    MX,
    SRT,
    TXT,
    CNAME
}

export class DomainHelper {

    static securityLevels = EnumData.getNamesAndValues(SecurityLevel);
    static cfRecordTypes = EnumData.getNamesAndValues(CFRecordType);
    static statuses = EnumData.getNamesAndValues(DomainStatus);

    static getDomainStatusDescription(value: number): string {
        const filter = DomainHelper.statuses.filter((s: { value: any; }) => s.value === value)[0].name;
        return filter;
    }

    static getDomainStatusValue(name: string): number {
        const filter = this.statuses.filter((s: { name: any; }) => s.name === name)[0].value;
        return filter;
    }

    static getLevelDescription(value: number): string {
        if (value == null || value == undefined) {
            return "";
        }

        const _filter = DomainHelper.securityLevels.filter(v => v.value === value);

        if (_filter == null || _filter == undefined) {
            return "";
        }
        return DomainHelper.securityLevels.filter(v => v.value === value)[0].name;

    }

    static getLevelValue(name: string): number {
        if (name == undefined) {
            return 0;
        }
        const _value = this.securityLevels.filter((s: { name: any; }) => s.name === name);
        if (_value == null || _value == undefined) {
            return 0;
        }
        const filter = this.securityLevels.filter((s: { name: any; }) => s.name === name)[0].value;

        return filter;
    }

    static getCFRecordTypeDescription(value: number): string {
        if (value == null || value == undefined) {
            return "";
        }

        const _filter = DomainHelper.cfRecordTypes.filter(v => v.value === value);

        if (_filter == null || _filter == undefined) {
            return "";
        }
        return DomainHelper.cfRecordTypes.filter(v => v.value === value)[0].name;

    }

    static getCFRecordTypeValue(name: string): number {
        if (name == undefined) {
            return 0;
        }
        const _value = this.cfRecordTypes.filter((s: { name: any; }) => s.name === name);
        if (_value == null || _value == undefined) {
            return 0;
        }
        const filter = this.cfRecordTypes.filter((s: { name: any; }) => s.name === name)[0].value;

        return filter;
    }
}


