import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { Platform } from '../models/platform.model';
import { TenantPlatformMap, CreateTenantPlatformMapRequest, Tenant, UpdateTenantPlatformMapRequest } from '../models/tenant.model';
import { PaymentSetting } from '../models/payment-setting.model';
import { DomainAvailabilityCheckRequest } from '../models/domain-management.model';
import { BaseService } from './base.service';
import { IEnvironment } from '../environment/environment.model';
import { LookupItem, TenantPlatformMapFinancialDocumentSettings } from '../models/shared.model';
import { EnvironmentService } from '../environment/environment.service';
import { FuseConfigService } from '@fuse/services/config.service';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TenantService extends BaseService {
  platformGuid: string;
  private baseUrl: string;
  timeZoneId: string;

  loadAndSetEnvironment(): void {
    this.environmentService.subscribe(this, (service: any, es: IEnvironment) => {
      this.baseUrl = es.serviceUrl;
      this.platformGuid = es.platformConfig.platformGuid;
    });
  }

  constructor(private http: HttpClient, private environmentService: EnvironmentService, private fuseConfigService: FuseConfigService) {
    super();
    this.fuseConfigService.config.subscribe(config => {

      if (config.timeZone != null && config.timeZone !== undefined) {
        this.timeZoneId = config.timeZone.timeZoneId;
      }
    });
    this.loadAndSetEnvironment();
  }

  getTenantSelectList(): Observable<TenantPlatformMap[]> {
    debugger;
    var returnValue = new TenantPlatformMap();
    returnValue.id = 1;
    return of([returnValue]);
  }


  getTenantMappingListCurrency(): Observable<TenantPlatformMap[]> {
    return this.http.get<TenantPlatformMap[]>(this.baseUrl + 'api/TenantPlatform/tenant-select-list-with-currency', super.getHttpOptions());
  }

  getTenantsWithLanguages(tenantGuid: string): Observable<TenantPlatformMap> {
    super.addTenantContextToHeaders(tenantGuid);
    return this.http.get<TenantPlatformMap>(this.baseUrl + 'api/TenantPlatform/tenant-select-with-languages', super.getHttpOptions());
  }
  getTenantCurrency(tenantGuid: string): Observable<TenantPlatformMap> {
    super.resetHeaders();
    super.addCustomHeaders("Account-Context", tenantGuid);
    return this.http.get<TenantPlatformMap>(this.baseUrl + 'api/TenantPlatform/tenant-select-with-currency', super.getHttpOptions());
  }

  getTenantMappingListOptions(): Observable<TenantPlatformMap[]> {
    return this.http.get<TenantPlatformMap[]>(this.baseUrl + 'api/TenantPlatform/tenant-select-list-with-options', super.getHttpOptions());
  }

  getTenantMappingList(): Observable<TenantPlatformMap[]> {
    super.addCustomHeaders('TimeZoneId', this.timeZoneId);
    return this.http.get<TenantPlatformMap[]>(this.baseUrl + 'api/TenantPlatform', super.getHttpOptions());
  }

  getTenants(): Observable<TenantPlatformMap[]> {
    super.addCustomHeaders('TimeZoneId', this.timeZoneId);
    return this.http.get<TenantPlatformMap[]>(this.baseUrl + 'api/TenantPlatform/get-tenants', super.getHttpOptions());
  }

  getTenantLookupList(): Observable<Tenant[]> {

//todo
    var returnValue = new Tenant();
    returnValue.id = 1;
    returnValue.businessName = '1';
    returnValue.description = '1';
    returnValue.mainContact = '1';
    returnValue.platformMappings = [];
    returnValue.telephone = '1';
    returnValue.tenancyName = '1';
    return of([returnValue]);
    // return this.http.get<Tenant[]>(this.baseUrl + 'api/Tenant/', super.getHttpOptions());
  }

  createTenant(tenant: CreateTenantPlatformMapRequest): Observable<TenantPlatformMap> {
    return this.http.post<TenantPlatformMap>(this.baseUrl + 'api/TenantPlatform/', tenant, super.getHttpOptions());
  }

  updateTenant(request: UpdateTenantPlatformMapRequest): Observable<TenantPlatformMap> {
    return this.http.put<TenantPlatformMap>(this.baseUrl + 'api/TenantPlatform/', request, super.getHttpOptions());
  }

  getCurrencyList(): Observable<LookupItem[]> {
    return this.http.get<LookupItem[]>(this.baseUrl + 'api/Currency/', super.getHttpOptions());
  }

  getLanguageList(): Observable<LookupItem[]> {
    return this.http.get<LookupItem[]>(this.baseUrl + 'api/Language/', super.getHttpOptions());
  }
  getDocumentList(): Observable<LookupItem[]> {
    return this.http.get<LookupItem[]>(this.baseUrl + 'api/document/', super.getHttpOptions());
  }

  getFinancialActionDocumentSettings(): Observable<TenantPlatformMapFinancialDocumentSettings[]> {
    return this.http.get<TenantPlatformMapFinancialDocumentSettings[]>(this.baseUrl + 'api/document/getfinancialactiondocumentlist', super.getHttpOptions());
  }
  getCountryList(): Observable<LookupItem[]> {
    return this.http.get<LookupItem[]>(this.baseUrl + 'api/Country/', super.getHttpOptions());
  }

  getTimeZoneList(): Observable<LookupItem[]> {
    return this.http.get<LookupItem[]>(this.baseUrl + 'api/TimeZone/', super.getHttpOptions());
  }

  getPlatform(): Observable<Platform> {
    return this.http.get<Platform>(this.baseUrl + 'api/platform/' + this.platformGuid, super.getHttpOptions());
  }

  getPaymentSettingList(): Observable<PaymentSetting[]> {
    return this.http.get<PaymentSetting[]>(this.baseUrl + 'api/PaymentSetting/', super.getHttpOptions());
  }

  checkDomainAvailability(request: DomainAvailabilityCheckRequest): Observable<boolean> {
    return this.http.post<boolean>(this.baseUrl + 'api/TenantPlatform/check-domain', request, super.getHttpOptions());
  }

  getTenantPaymentSettingList(tenantPlatformMapGuid: string): Observable<PaymentSetting[]> {
    return this.http.post<PaymentSetting[]>(this.baseUrl + 'api/TenantPlatform/getTenantPaymentList', { TenantPlatformMapGuid: tenantPlatformMapGuid }, super.getHttpOptions());
  }

  getTenantAvailablePaymentSettings(tenantPlatformMapGuid: string): Observable<PaymentSetting[]> {
    return this.http.post<PaymentSetting[]>
      (this.baseUrl + 'api/TenantPlatform/getTenantAvailablePaymentSettings', { TenantPlatformMapGuid: tenantPlatformMapGuid }, super.getHttpOptions());
  }

}
