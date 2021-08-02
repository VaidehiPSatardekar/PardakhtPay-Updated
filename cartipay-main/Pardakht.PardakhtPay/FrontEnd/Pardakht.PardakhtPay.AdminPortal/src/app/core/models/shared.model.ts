export class LookupItem {
  id: number;
  code: string;
  description: string;
  image: string;
}

export class CurrencyLookupItem {
  id: number;
  code: string;
  description: string;
  image: string;
  minDeposit: number;
  maxDeposit: number;
  minWithdrawal: number;
  maxWithdrawal: number;
  withdrawalLimitAmount: number;
  withdrawalLimitDuration: number;
}

export class BrandLookupItem {
  id: number;
  brandName: string;
}

export class MappedLookupItem extends LookupItem {
  isDefault: boolean;
}

export class LookupMapping {
  constructor(public id: number, public isDefault: boolean) {
  }
}

export class CurrencyLookupMapping {
  constructor(public id: number, public isDefault: boolean, public minDeposit: number, public maxDeposit: number, public minWithdrawal: number, public maxWithdrawal: number,
    public withdrawalLimitAmount: number, public withdrawalLimitDuration: number) {
  }
}

export class BrandLookupMapping {
  constructor(public id: number, public isDefault: boolean, public minDeposit: number, public maxDeposit: number, public minWithdrawal: number, public maxWithdrawal: number,
    public withdrawalLimitAmount: number, public withdrawalLimitDuration: number) {
  }
}

export class Metadata {
  sections: Section[];
}

export class Section {
  label: string;
  name: string;
  configType: string;
  items: MetadataItem[];
}

export class MetadataItem {
  label: string;
  name: string;
  type: string;
  required: boolean;
  preferenceOptions: PreferenceOption[];
  value: any[];
  defaultValue: any;
  configCaptureType: string;
  configRegex: string;
}

export class PreferenceOption {
  name: string;
  defaultValue: string;
}

export class TenantPlatformMapDocument {
  id: number;
  documentId: number;
  isActive: boolean;
  isDefault: boolean;
}

export class TenantPlatformMapFinancialDocumentSettings {
  id: number;
  financialActionId: number;
  isActive: boolean;
  isDefault: boolean;
}

