export interface IEnvironment {
  platformGuid: string;
  serviceUrl: string;
  financialUrl: string;
  customerUrl: string;
  agGridLicense: string;
  platformConfig: IPlatformConfig;
}

export interface IPlatformConfig {
  platformGuid: string;
  parentAccountEnabled: boolean;
}
