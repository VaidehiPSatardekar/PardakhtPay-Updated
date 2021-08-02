import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import { ReplaySubject, Observable } from 'rxjs';
import { MatChipInputEvent } from '@angular/material';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import * as riskyKeywordActions from '../../core/actions/riskyKeyword';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material';
import { takeUntil, filter, take } from 'rxjs/operators';

@Component({
    selector: 'app-risky-keyword',
    templateUrl: './risky-keyword.component.html',
    styleUrls: ['./risky-keyword.component.scss']
})
export class RiskyKeywordComponent implements OnInit, OnDestroy {
    visible = true;
    selectable = true;
    removable = true;
    addOnBlur = true;
    isCreating: boolean;
    readonly separatorKeysCodes: number[] = [ENTER, COMMA];
    keywords: string[] = [];

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    updateError$: Observable<string>;
    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;

    getDetail$: Observable<string[]>;
    updateSuccess$: Observable<boolean>;

    constructor(private store: Store<coreState.State>,
        private translateService: TranslateService,
        public snackBar: MatSnackBar) { }

    ngOnInit() {

        this.updateError$ = this.store.select(coreState.getRiskyKeywordUpdateError);
        this.getDetailError$ = this.store.select(coreState.getRiskyKeywordLoadError);
        this.getDetailLoading$ = this.store.select(coreState.getRiskyKeywordsLoading);
        this.getDetail$ = this.store.select(coreState.getRiskyKeywordsDetail);
        this.updateSuccess$ = this.store.select(coreState.getRiskyKeywordUpdateSuccess);

        this.updateError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.getDetailError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.getDetailLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.getDetailLoading = l;
        });

        this.getDetail$.pipe(takeUntil(this.destroyed$)).subscribe(detail => {
            if (detail) {
                this.keywords = detail;
            }
        });

        this.updateSuccess$.pipe(filter(tnCreated => tnCreated))
            .subscribe(
                () => {
                    this.isCreating = false;
                    this.openSnackBar('Operation completed');
                });

        this.store.dispatch(new riskyKeywordActions.GetDetails());
    }

    onSubmit() {
        this.store.dispatch(new riskyKeywordActions.Edit(this.keywords));
    }

    add(event: MatChipInputEvent): void {
        const input = event.input;
        const value = event.value;

        if ((value || '').trim()) {
            this.keywords.push(value.trim());
        }

        if (input) {
            input.value = '';
        }
    }

    remove(fruit: string): void {
        const index = this.keywords.indexOf(fruit);

        if (index >= 0) {
            this.keywords.splice(index, 1);
        }
    }

    ngOnDestroy(): void {
        this.store.dispatch(new riskyKeywordActions.Clear());
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
