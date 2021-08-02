import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { fuseAnimations } from '../../core/animations';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as tenantActions from '../../core/actions/tenant';
import * as accountActions from '../../core/actions/account';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
    styleUrls: ['./users.component.scss'],
    animations: fuseAnimations
})
export class UsersComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    selected = [];
    tenantsLoading$: Observable<boolean>;
    tenantsLoading: boolean;
    tenants$: Observable<Tenant[]>;
    tenants: Tenant[] = [];
    //tenantSearchError$: Observable<string>;

    tenantGuid$: Observable<string>;
    tenantGuid: string;

    isProviderAdmin$: Observable<boolean>;
    isProviderAdmin: boolean;

    isTenantAdmin$: Observable<boolean>;
    isTenantAdmin: boolean;

    isStandardUser$: Observable<boolean>;
    isStandardUser: boolean;

    accountGuid$: Observable<string>;
    accountGuid: string;

    ownersLoading$: Observable<boolean>;
    owners$: Observable<Owner[]>;
    owners: Owner[] = [];

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    constructor(private store: Store<coreState.State>,
        private dateAdapter: DateAdapter<any>,
        private router: Router,
        public snackBar: MatSnackBar,
        private translateService: TranslateService) {
        this.dateAdapter.setLocale('gb');
    }

    openSnackBar(message: string, action: string = undefined) {
        if (!action) {
            action = this.translateService.instant('GENERAL.OK');
        }
        this.snackBar.open(message, action, {
            duration: 10000,
        });
    }

    ngOnInit() {

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);
        //this.tenantSearchError$ = this.store.select(coreState.getTenantSearchError);
        this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

        this.tenantGuid$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.tenantGuid = t;
        });

        //this.tenantSearchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
        //    if (error) {
        //        this.openSnackBar(error);
        //    }
        //});

        this.isProviderAdmin$ = this.store.select(coreState.getAccountIsProviderAdmin);
        this.isProviderAdmin$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.isProviderAdmin = t;
            //if (t == true) {
            //    this.store.dispatch(new tenantActions.Search());
            //}
        });

        this.isTenantAdmin$ = this.store.select(coreState.getAccountIsTenantAdmin);
        this.isTenantAdmin$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.isTenantAdmin = t;
        });

        this.isStandardUser$ = this.store.select(coreState.getAccountIsStandardUser);
        this.isStandardUser$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.isStandardUser = t;
        });

        this.accountGuid$ = this.store.select(coreState.getAccountGuid);
        this.accountGuid$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.accountGuid = t;
        });

        this.tenants$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items != null) {
                this.tenants = items;
            }
            else {
                this.tenants = [];
            }
        });

        this.tenantsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.tenantsLoading = l;
        });

        this.ownersLoading$ = this.store.select(coreState.getAccountLoading);
        this.owners$ = this.store.select(coreState.getOwners);

        this.owners$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items != undefined) {
                this.owners = items;
            } else {
                this.owners = [];
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
            }
        });
    }

    ngOnDestroy(): void {
        this.store.dispatch(new accountActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    getTenantName(tenantGuid: string): string {

        if (this.tenants != null && this.tenants != undefined) {
            var tenant = this.tenants.find(t => t.tenantDomainPlatformMapGuid == tenantGuid);

            if (tenant != null) {
                return tenant.tenantName;
            }
        }

        return '';
    }
    onSelect() {

        //if (this.selected.length > 0) {
        //    this.router.navigate(['/user/' + this.selected[0].id]);
        //}
    }

}
