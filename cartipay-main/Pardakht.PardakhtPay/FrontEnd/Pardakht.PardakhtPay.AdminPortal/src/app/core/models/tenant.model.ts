import { GenericHelper } from './../../helpers/generic';
import { TenantDomain } from './domain-management.model';
import { CurrencyLookupItem, LookupItem, LookupMapping, CurrencyLookupMapping } from './shared.model';

export class TenantPlatformMap {
  id: number;
  tenantPlatformMapGuid: string;
  tenantId: number;
  tenant: Tenant;
  platformId: number;
  platformGuid: string;
  brandName: string;
  primaryDomainName: string;
  subDomain: string;
  connectionString: string;
  domainNames: string[];
  currency: any;
  currencies: CurrencyLookupItem[];
  languages: LookupItem[];
  countries: LookupMapping[];
  timeZones: LookupMapping[];
  preferenceConfig: string;
  products: Product[];
  paymentSettings: TenantPaymentSetting[];
  tenantPlatformMapBrands: Brand[];
  domain: TenantDomain;
  domains: TenantDomain[];

  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }
}

export class CreateTenantPlatformMapRequest {
  existingTenantId: number | null;
  newTenant: Tenant;
  platformDetails: PlatformMappingDetails;
  tenantToClone: string;
}

export class UpdateTenantPlatformMapRequest {
  tenantPlatformMapGuid: string;
  tenant: Tenant;
  platformDetails: PlatformMappingDetails;
}

export class Tenant {
  id: number;
  businessName: string;
  description: string;
  tenancyName: string;
  mainContact: string;
  telephone: string;
  email: string;
  platformMappings: TenantPlatformMap[];

  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }
}

export class PlatformMappingDetails {
  brandName: string;
  primaryDomainName: string;
  currencyMappings: CurrencyLookupMapping[];
  languageMappings: LookupMapping[];
  countryMappings: LookupMapping[];
  timeZoneMappings: LookupMapping[];
  paymentSettingMappings: TenantPaymentSetting[];
  brandMappings: Brand[];
  products: number[];
  preferenceConfig: string;
  adminUserEmail: string;
  domain: TenantDomain;

  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }
}

export class Platform {
  id: number;
  platformGuid: string;
  name: string;
  preferenceMetadata: string;
  products: Product[];
  tenantPlatformMapBrands: Brand[];
  domain: TenantDomain;
}

export class Brand {
  id: number;
  brandName: string;
  tenantUid: string;
}


export class Product {
  id: number;
  name: string;
}

export class TenantPaymentSetting {
  customFields: string;
  gatewayKey: string;
  paymentSettingId: number;
  paymentSettingStatus: number;
  currency: string;
  isDefault: boolean;
  paymentIdentifier: string;
  paymentSettingCustomerSegmentationRelations: number[];
}
export enum TenantBoolStatus {
  true,
  false
}
export enum TenantStatus {
  Active,
  InActive,
  Disabled,
  Blocked,
  Pending
}

export enum TenantCreateStatus {
  InitialState = 0,
  UseExistingDomainState = 2,
  BuyNewDomainState = 8,
  CreateCustomerManagementState = 10,
  CreateFinancialManagementState = 20,
  CreateAffiliateManagementState = 30,
  CreateInvoiceManagementState = 40,
  CreateSportsBookState = 42,
  RegisterDomainInfoState = 45,
  CreateCmsState = 50,
  TerminateState = -1,
  Completed = 100
}

// tslint:disable-next-line:max-classes-per-file
// @dynamic
export class EnumData {
  static getNamesAndValues<T extends number>(e: any) {
    // tslint:disable-next-line:prefer-type-cast
    return EnumData.getNames(e).map(n => ({ name: n, value: e[n] as T }));
  }

  static getNames(e: any) {
    // tslint:disable-next-line:prefer-type-cast
    return EnumData.getObjValues(e).filter(v => typeof v === 'string') as string[];
  }

  static getValues<T extends number>(e: any) {
    // tslint:disable-next-line:prefer-type-cast
    return EnumData.getObjValues(e).filter(v => typeof v === 'number') as T[];
  }

  public static getObjValues(e: any): (number | string)[] {
    return Object.keys(e).map(k => e[k]);
  }
}

export class TenantBoolHelper {

  static dataSourcePriority = EnumData.getNamesAndValues(TenantBoolStatus);


  static getPriorityDescription(value: number): string {
    const filter = this.dataSourcePriority.filter(v => v.value === value)[0].name;
    return filter;
  }

  static getPriorityValue(name: string): number {
    const filter = this.dataSourcePriority.filter((s: { name: any; }) => s.name === name)[0].value;
    return filter;
  }
}
export class TenantStatusHelper {

  static dataSourcePriority = EnumData.getNamesAndValues(TenantStatus);


  static getPriorityDescription(value: number): string {
    const filter = this.dataSourcePriority.filter(v => v.value === value)[0].name;
    return filter;
  }

  static getPriorityValue(name: string): number {
    const filter = this.dataSourcePriority.filter((s: { name: any; }) => s.name === name)[0].value;
    return filter;
  }
}


export class TenanCreateStatusHelper {

  static dataSourcePriority = EnumData.getNamesAndValues(TenantCreateStatus);


  static getPriorityDescription(value: number): string {
    if (value === null) {
      return 'NotStarted';
    }
    const filter = this.dataSourcePriority.filter(v => v.value === value)[0];
    if (filter && filter.name) {
      return filter.name;
    }
    return '';
  }

  static getPriorityValue(name: string): number {
    const filter = this.dataSourcePriority.filter((s: { name: any; }) => s.name === name)[0].value;
    return filter;
  }
}
