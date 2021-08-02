export interface Platform {
    id: number;
    platformGuid: string;
    name: string;
}

export class PlatformConfig {
  platformGuid: string;
  parentAccountEnabled: boolean;
}
