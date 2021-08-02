import { GenericHelper } from './../../helpers/generic';

export class PaymentSetting {
  id: number;
  name: string;
  description: string;
  currency: string[];
  paymentSettingStatus: PaymentSettingStatus;
  imageUrl: string;
  isDefault: boolean;
  gatewayKey: string;
  customFields: CustomField[];
  depositCustomFields: CustomField[];
  withdrawCustomFields: CustomField[];
  paymentIdentifier: string;
  tenantPaymentSettingType: TenantPaymentSettingType;
  paymentSettingCustomerSegmentationRelations: PaymentSettingCustomerSegmentationRelation[];
  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }

}


export class ProxyIpAddress {
  id: number;
  name: string;
  ipAddress: string;
  
  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }

}


export class DefaultOrder {
  id: number;
  priority: number;
  isDefault: boolean;
  tenantPaymentSettingType: TenantPaymentSettingType;
  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }

}




export class CustomField {
  type: string;
  name: string;
  label: string;
  value: string;
  required: boolean;
  isReadonly: boolean;
  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }

  
}


export declare class UpdateTenantPaymentSetting {
  paymentSettingId: number;
  defaultpaymentSettingId: number;
  gatewayKey: string;
  tenantPlatformMapId: number;
  customFields: CustomField[];
  constructor(data?: any);
  paymentSettingCustomerSegmentationRelations: PaymentSettingCustomerSegmentationRelation[];
}

export class PaymentSettingCustomerSegmentationRelation{
  id: number;
  customerSegmenationGroupId: number;
  tenantPlatformMapPaymentSettingId: number;
}

export enum PaymentSettingStatus
{
    active = 1,
    passive = 2,
    deleted = 3
}

export enum TenantPaymentSettingType
{
  withdraw = 1,
  deposit = 2,
  both = 3
}


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

export class PaymentSettingHelper {

  static dataSourceStatus = EnumData.getNamesAndValues(PaymentSettingStatus);
  static dataTypeSourceStatus = EnumData.getNamesAndValues(TenantPaymentSettingType);


  static getStatusDescription(value: number): string {
    const _value = PaymentSettingHelper.dataSourceStatus.filter((s: { value: any; }) => s.value === value);
    if (_value == null || _value == undefined || _value.length == 0) {
      return "";
    }
    const filter = PaymentSettingHelper.dataSourceStatus.filter((s: { value: any; }) => s.value === value)[0].name;
    return filter;
  }

  static getTypeDescription(value: number): string {

    const _value = PaymentSettingHelper.dataTypeSourceStatus.filter((s: { value: any; }) => s.value === value);
    if (_value == null || _value == undefined || _value.length == 0) {
      return "";
    }
    const filter = PaymentSettingHelper.dataTypeSourceStatus.filter((s: { value: any; }) => s.value === value)[0].name;
    return filter;
  }

  static getStatusValue(name: string): number {
    const filter = this.dataSourceStatus.filter((s: { name: any; }) => s.name === name)[0].value;
    return filter;
  }


}