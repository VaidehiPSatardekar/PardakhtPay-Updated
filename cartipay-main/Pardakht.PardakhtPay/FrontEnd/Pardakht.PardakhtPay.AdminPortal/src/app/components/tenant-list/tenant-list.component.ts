import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import {  MatSnackBar } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import { Tenant } from '../../models/tenant';
import * as tenantActions from '../../core/actions/tenant';
import { fuseAnimations } from '../../core/animations';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-tenant-list',
  templateUrl: './tenant-list.component.html',
    styleUrls: ['./tenant-list.component.scss'],
    animations: fuseAnimations
})
export class TenantListComponent implements OnInit, OnDestroy {
    loading$: Observable<boolean>;
    items$: Observable<Tenant[]>;
    items: Tenant[];
    searchError$: Observable<string>;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private translateService: TranslateService) {
    }

    ngOnInit() {
        this.loading$ = this.store.select(coreState.getTenantLoading);
        this.items$ = this.store.select(coreState.getTenantSearchResults);
        this.searchError$ = this.store.select(coreState.getTenantSearchError);

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.items$.subscribe(data => {
            this.items = data;
        });

        this.loadTenants();
    }

    loadTenants() {
        //this.store.dispatch(new tenantActions.Search());
    }

    refresh() {
        this.loadTenants();
    }

    ngOnDestroy(): void {
        //this.store.dispatch(new tenantActions.Clear());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    openSnackBar(message: string, action: string = undefined) {
        if (!action) {
            action = this.translateService.instant('GENERAL.OK');
        }
        this.snackBar.open(message, action, {
            duration: 10000,
        });
    }
}

