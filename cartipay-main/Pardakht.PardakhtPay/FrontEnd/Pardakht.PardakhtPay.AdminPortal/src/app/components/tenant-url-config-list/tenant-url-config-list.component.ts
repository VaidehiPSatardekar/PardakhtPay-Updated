import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import * as coreState from '../../core';
import { Store } from '@ngrx/store';
import { Observable, ReplaySubject } from 'rxjs';
import { TenantUrlConfig } from '../../models/tenant-url-config';
import * as tenantUrlConfigActions from '../../core/actions/tenantUrlConfig';
import { fuseAnimations } from '../../core/animations';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { Merchant } from '../../models/merchant-model';
import * as merchantActions from '../../core/actions/merchant';

@Component({
  selector: 'app-tenant-url-config-list',
  templateUrl: './tenant-url-config-list.component.html',
    styleUrls: ['./tenant-url-config-list.component.scss'],
    animations: fuseAnimations
})
export class TenantUrlConfigListComponent implements OnInit, OnDestroy {

    tenantUrlConfigs$: Observable<TenantUrlConfig[]>;
    searchError$: Observable<string>;
    selected = [];
    loading$: Observable<boolean>;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    merchantsLoading$: Observable<boolean>;
    merchantsLoading: boolean;
    merchants$: Observable<Merchant[]>;
    merchants: Merchant[];
    merchantsSearchError$: Observable<string>;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        public snackBar: MatSnackBar,
        private translateService: TranslateService,
        private router: Router) { }

    ngOnInit() {
        this.tenantUrlConfigs$ = this.store.select(coreState.getAllTenantUrlConfigs);
        this.loading$ = this.store.select(coreState.getTenantUrlConfigLoading);
        this.searchError$ = this.store.select(coreState.getAllTenantUrlConfigError);

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;
            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new tenantUrlConfigActions.GetAll());
                this.store.dispatch(new merchantActions.Search(''));
            }
        });

        this.merchants$ = this.store.select(coreState.getMerchantSearchResults);
        this.merchantsLoading$ = this.store.select(coreState.getMerchantLoading);
        this.merchantsSearchError$ = this.store.select(coreState.getMerchantSearchError);

        this.merchants$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.merchants = items;
        });

        this.merchantsSearchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.merchantsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.merchantsLoading = l;
        });
    }
    openSnackBar(message: string, action: string = undefined) {
        if (!action) {
            action = this.translateService.instant('GENERAL.OK');
        }
        this.snackBar.open(message, action, {
            duration: 10000,
        });
    }
    onSelect() {
        if (this.selected.length > 0) {
            this.router.navigate(['/tenanturlconfig/' + this.selected[0].id]);
        }
    }

    getMerchantName(merchantId): string {
        if (this.merchants == undefined || this.merchants == null) {
            return '';
        }

        var merchant = this.merchants.find(t => t.id == merchantId);

        if (merchant == null) {
            return '';
        }

        return merchant.title;
    }

    ngOnDestroy(): void {
        this.store.dispatch(new tenantUrlConfigActions.ClearErrors());
        this.store.dispatch(new merchantActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }
}

