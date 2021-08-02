import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource, MatSnackBar } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import { CardHolder } from '../../models/card-holder';
import * as cardHolderActions from '../../core/actions/cardHolder';
import { fuseAnimations } from '../../core/animations';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import * as tenantActions from '../../core/actions/tenant';
import { Tenant } from '../../models/tenant';
import { FuseConfigService } from '../../../@fuse/services/config.service';

@Component({
    selector: 'app-card-holder-name',
    templateUrl: './card-holder-name.component.html',
    styleUrls: ['./card-holder-name.component.scss'],
    animations: fuseAnimations
})
export class CardHolderNameComponent implements OnInit, OnDestroy {

    cardNumber: string = undefined;

    loading$: Observable<boolean>;
    cardHolder$: Observable<CardHolder>;
    cardHolder: CardHolder;

    error$: Observable<string>;

    tenants$: Observable<Tenant[]>;
    tenants: Tenant[] = [];

    tenantGuid$: Observable<string>;
    tenantGuid: string;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private translateService: TranslateService) { }

    ngOnInit() {

        this.loading$ = this.store.select(coreState.getCardHolderLoading);

        this.cardHolder$ = this.store.select(coreState.getCardHolderSearchResult);

        this.cardHolder$.pipe(takeUntil(this.destroyed$)).subscribe(item => {
            this.cardHolder = item;
        });

        this.error$ = this.store.select(coreState.getCardHolderSearchError);
        this.error$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    onSearch() {
        if (this.cardNumber) {
            this.store.dispatch(new cardHolderActions.Search(this.cardNumber));
        }
    }

    ngOnDestroy(): void {
        this.store.dispatch(new cardHolderActions.ClearAll());
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
