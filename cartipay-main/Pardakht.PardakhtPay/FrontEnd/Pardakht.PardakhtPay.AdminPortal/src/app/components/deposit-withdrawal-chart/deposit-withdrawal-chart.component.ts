import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatSnackBar, DateAdapter } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import * as bankStatementactions from '../../core/actions/bankStatement';
import { fuseAnimations } from '../../core/animations';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { TimeZoneService } from '../../core/services/timeZoneService/time-zone.service';
import { TimeZone } from '../../models/timeZone';
import * as bankLoginActions from '../../core/actions/bankLogin';
import { BankLogin, BankAccount } from '../../models/bank-login';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { IGetRowsParams } from 'ag-grid-community';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { GenericHelper } from '../../helpers/generic';
import { NgModel } from '@angular/forms';
import { BankStatementReportSearchArgs, TransactionReportSearchArgs } from 'app/models/report';
import { DashBoardChartWidget, DashBoardWidget } from 'app/models/dashboard';
import * as reportActions from '../../core/actions/report';

@Component({
  selector: 'app-deposit-withdrawal-chart',
  templateUrl: './deposit-withdrawal-chart.component.html',
    styleUrls: ['./deposit-withdrawal-chart.component.scss'],
    animations: fuseAnimations
})
export class DepositWithdrawalChartComponent implements OnInit, OnDestroy {

    args: TransactionReportSearchArgs;

    loading$: Observable<boolean>;
    widget$: Observable<DashBoardChartWidget>;
    widget: DashBoardChartWidget
    searchError$: Observable<string>;

    tenantGuid$: Observable<string>;
    tenantGuid: string;

    selectedZoneId: string;
    zones: TimeZone[] = [];

    isProviderAdmin$: Observable<boolean>;
    isProviderAdmin: boolean;

    isTenantAdmin$: Observable<boolean>;
    isTenantAdmin: boolean;

    isStandardUser$: Observable<boolean>;
    isStandardUser: boolean;

    accountGuid$: Observable<string>;
    accountGuid: string;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    firstLoaded: boolean = false;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private timeZoneService: TimeZoneService,
        private fuseConfigService: FuseConfigService,
        public translateService: TranslateService) {

        var cachedArgs = localStorage.getItem('depositwithdrawalchartreportargs');

        if (cachedArgs != undefined && cachedArgs != null) {
            this.args = JSON.parse(cachedArgs);

            if (!this.args.endDate || !this.args.startDate) {
                this.args.endDate = new Date().toISOString()
                this.args.startDate = new Date().toISOString();
                this.args.startDate = new Date(new Date().setDate(new Date(this.args.endDate).getDate() - 7)).toISOString();
            }
        }
        else {
            this.args = new TransactionReportSearchArgs();
            this.args.endDate = new Date().toISOString();
            this.args.startDate = new Date().toISOString();
            this.args.startDate = new Date(new Date().setDate(new Date(this.args.endDate).getDate() - 7)).toISOString();
        }
    }

    ngOnInit() {
        this.loading$ = this.store.select(coreState.getReportDepositWithdrawalLoading);
        this.widget$ = this.store.select(coreState.getReportDepositWithdrawalWidget);
        this.searchError$ = this.store.select(coreState.getReportDepositWithdrawalError);

        this.zones = this.timeZoneService.getTimeZones();
        this.selectedZoneId = this.timeZoneService.getTimeZone().timeZoneId;

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.widget$.pipe(takeUntil(this.destroyed$)).subscribe((data: DashBoardChartWidget) => {
            this.widget = data;
        });

        this.isProviderAdmin$ = this.store.select(coreState.getAccountIsProviderAdmin);
        this.isProviderAdmin$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.isProviderAdmin = t;
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

        this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

        this.tenantGuid$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.tenantGuid = t;
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid) {
                this.refresh();
            }
        });
    }

    loadReport() {

        if (this.selectedTenant == undefined) {
            return;
        }
        this.firstLoaded = true;
        this.args.timeZoneInfoId = this.selectedZoneId;

        var newArgs = { ...this.args };

        newArgs.startDate = new Date(new Date(this.args.startDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();
        newArgs.endDate = new Date(new Date(this.args.endDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();

        localStorage.setItem('depositwithdrawalchartreportargs', JSON.stringify(this.args));
        this.store.dispatch(new reportActions.GetDepositWithdrawalWidget(newArgs));
    }

    refresh() {
        this.loadReport();
    }

    equals(objOne, objTwo) {
        if (typeof objOne !== 'undefined' && typeof objTwo !== 'undefined') {
            return objOne === objTwo;
        }
    }

    selectAll(select: NgModel, values) {
        select.update.emit(values.map(t => t.accountGuid));
    }

    deselectAll(select: NgModel) {
        select.update.emit([]);
    }

    ngOnDestroy(): void {
        this.store.dispatch(new reportActions.ClearAll());
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

    setTimeZone() {
        let zone = this.zones.find(t => t.timeZoneId == this.selectedZoneId);
        this.timeZoneService.setTimeZone(zone);
        this.refresh();
    }
}

