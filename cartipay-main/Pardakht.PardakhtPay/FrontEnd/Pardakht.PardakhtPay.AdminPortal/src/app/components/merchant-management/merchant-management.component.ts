import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { Merchant, MerchantCreate } from '../../models/merchant-model';
import * as merchantActions from '../../core/actions/merchant';
import { Observable, ReplaySubject } from 'rxjs';
import { Store } from '@ngrx/store';
import { MatDialog } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import * as coreState from '../../core';
import { tap, takeUntil, take } from 'rxjs/operators';
import { fuseAnimations } from '../../core/animations';
import { filter } from 'rxjs/operators/filter';
import { AccountService } from '../../core/services/account.service';

@Component({
  selector: 'app-merchant-management',
  templateUrl: './merchant-management.component.html',
    styleUrls: ['./merchant-management.component.scss'],
    animations: fuseAnimations
})
export class MerchantManagementComponent implements OnInit, OnDestroy {


    merchants$: Observable<Merchant[]>;
    loading$: Observable<boolean>;
    editError$: Observable<string>;
    createError$: Observable<string>;
    updateSuccess$: Observable<boolean>;
    showSearch: boolean = true;
    selectedMerchant: Merchant;
    isCreating: boolean;
    formSubmit: boolean;
    formChanges: boolean;

    merchantCreated$: Observable<MerchantCreate>;
    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        private dialogService: MatDialog,
        private translateService: TranslateService) {
    }

    ngOnInit() {

        this.loading$ = this.store.select(coreState.getMerchantLoading);

        this.store.select(coreState.getMerchantSearchResults).subscribe((result: Merchant[]) => {

            
        });

        this.merchants$ = this.store.select(coreState.getMerchantSearchResults).pipe(
            tap((result: Merchant[]) => {

                if (this.formSubmit) {
                    this.formSubmit = undefined;
                }
            })
        );

        this.updateSuccess$ = this.store.select(coreState.getMerchantEditSuccess);
        this.updateSuccess$.pipe(takeUntil(this.destroyed$))
            .subscribe((result: boolean) => {
                if (result) {
                    this.selectedMerchant = undefined;
                }
            });

        this.merchantCreated$ = this.store.select(coreState.getMerchantCreated);
        this.editError$ = this.store.select(coreState.getMerchantEditError);
        this.createError$ = this.store.select(coreState.getMerchantCreateError);

        this.store.dispatch(new merchantActions.Search(''));
    }

    ngOnDestroy(): void {
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    onMerchantSelected(merchant: Merchant): void {
        this.selectedMerchant = merchant;
    }

    onCloseMerchant(): void {
        this.selectedMerchant = undefined;
    }

    onCreateMerchant(form: MerchantCreate) {
        //console.log(form);
        this.formSubmit = true;
        this.store.dispatch(new merchantActions.Create(form));

        this.merchantCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
            .subscribe(
                tnCreated => {
                    this.isCreating = false;
                });
    }

    onFormChange(changes: boolean): void {
        this.formChanges = changes;
    }
}
