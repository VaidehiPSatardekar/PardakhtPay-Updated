import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import { ReplaySubject, Observable } from 'rxjs';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { ApplicationSettings } from '../../models/application-settings';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import * as applicationSettingsActions from '../../core/actions/applicationSettings';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material';
import { takeUntil, filter, take } from 'rxjs/operators';
import { FormHelper } from '../../helpers/forms/form-helper';

@Component({
    selector: 'app-application-settings',
    templateUrl: './application-settings.component.html',
    styleUrls: ['./application-settings.component.scss']
})
export class ApplicationSettingsComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    settingsForm: FormGroup;
    formSubmit: boolean = false;
    isCreating: boolean;
    updateError$: Observable<string>;
    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;

    getDetail$: Observable<ApplicationSettings>;
    updateSuccess$: Observable<boolean>;
    @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

    constructor(private store: Store<coreState.State>,
        private translateService: TranslateService,
        private fb: FormBuilder,
        public snackBar: MatSnackBar) { }

    ngOnInit() {

        this.updateError$ = this.store.select(coreState.getApplicationSettingsUpdateError);
        this.getDetailError$ = this.store.select(coreState.getApplicationSettingsLoadError);
        this.getDetailLoading$ = this.store.select(coreState.getApplicationSettingsLoading);
        this.getDetail$ = this.store.select(coreState.getApplicationSettingsDetail);
        this.updateSuccess$ = this.store.select(coreState.getApplicationSettingsUpdateSuccess);

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
                this.createUpdateForm(detail);
            }
        });

        this.updateSuccess$.pipe(filter(tnCreated => tnCreated))
            .subscribe(
                () => {
                    this.isCreating = false;
                    this.openSnackBar('Operation completed');
                });

        this.store.dispatch(new applicationSettingsActions.GetDetails());
    }

    createUpdateForm(data: ApplicationSettings): void {
        var sms = this.fb.group(data.smsConfiguration);
        var malicious = this.fb.group(data.maliciousCustomerSettings);
        var bankAccount = this.fb.group(data.bankAccountConfiguration);
        var mobileApi = this.fb.group(data.mobileApiConfiguration);

        this.settingsForm = this.fb.group({
            smsConfiguration: sms,
            maliciousCustomerSettings: malicious,
            bankAccountConfiguration: bankAccount,
            mobileApiConfiguration: mobileApi
        });

        console.log(this.settingsForm);
    }

    onSubmit() {
        this.formSubmit = false;
        if (this.settingsForm.valid) {
            let form = this.settingsForm.value;
            this.formSubmit = true;
            this.store.dispatch(new applicationSettingsActions.Edit(form));

        }
        else {
            FormHelper.validateFormGroup(this.settingsForm);
        }
    }

    ngOnDestroy(): void {
        this.store.dispatch(new applicationSettingsActions.Clear());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
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
