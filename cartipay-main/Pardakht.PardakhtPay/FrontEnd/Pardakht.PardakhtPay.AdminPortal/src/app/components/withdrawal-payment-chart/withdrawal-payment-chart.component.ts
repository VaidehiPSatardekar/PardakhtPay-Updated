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
import { BankStatementReportSearchArgs, WithdrawalPaymentReportArgs } from 'app/models/report';
import { DashBoardChartWidget, DashBoardWidget } from 'app/models/dashboard';
import * as reportActions from '../../core/actions/report';

@Component({
    selector: 'app-withdrawal-payment-chart',
    templateUrl: './withdrawal-payment-chart.component.html',
    styleUrls: ['./withdrawal-payment-chart.component.scss'],
    animations: fuseAnimations
})
export class WithdrawalPaymentChartComponent implements OnInit, OnDestroy {
    args: WithdrawalPaymentReportArgs;

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
    includeDeletedLogins: boolean = false;

    filter: string = undefined;

    filteredBankAccounts: BankAccount[] = undefined;

    cartipalRate: string = '0';
    transferRate: string = '0';

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private timeZoneService: TimeZoneService,
        private fuseConfigService: FuseConfigService,
        public translateService: TranslateService) {

        var cachedArgs = localStorage.getItem('withdrawalpaymentchartreportargs');

        if (cachedArgs != undefined && cachedArgs != null) {
            this.args = JSON.parse(cachedArgs);

            if (!this.args.endDate || !this.args.startDate) {
                this.args.endDate = new Date().toISOString()
                this.args.startDate = new Date().toISOString();
                this.args.startDate = new Date(new Date().setDate(new Date(this.args.endDate).getDate() - 7)).toISOString();
            }
        }
        else {
            this.args = new WithdrawalPaymentReportArgs();
            this.args.endDate = new Date().toISOString();
            this.args.startDate = new Date().toISOString();
            this.args.startDate = new Date(new Date().setDate(new Date(this.args.endDate).getDate() - 7)).toISOString();
        }
    }

    ngOnInit() {

        this.loading$ = this.store.select(coreState.getReportWithdrawalPaymentLoading);
        this.widget$ = this.store.select(coreState.getReportWithdrawalPaymentWidget);
        this.searchError$ = this.store.select(coreState.getReportWithdrwalPaymentError);

        this.zones = this.timeZoneService.getTimeZones();
        this.selectedZoneId = this.timeZoneService.getTimeZone().timeZoneId;

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.widget$.pipe(takeUntil(this.destroyed$)).subscribe((data: DashBoardChartWidget) => {
            this.widget = data;
            this.cartipalRate = '0';
            this.transferRate = '0';

            if(this.widget){
                var d = this.widget.mainChart['data'];

                var cartipal = 0.0;
                var transfers = 0.0;

                var cartipalItems = d.find(t => t.name == 'Cartipal Payments');

                for(let i= 0; i < cartipalItems.series.length; i++){
                    cartipal += cartipalItems.series[i].value;
                }

                var transferItems = d.find(t => t.name == 'Transfers');

                for(let i= 0; i < transferItems.series.length; i++){
                    transfers += transferItems.series[i].value;
                }

                var total = cartipal + transfers;

                if(total > 0){
                    this.cartipalRate = ((cartipal * 100) / total).toFixed(2);
                    this.transferRate = ((transfers * 100) / total).toFixed(2);
                }
            }
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
                this.loadBankStatements()
            }
        });
    }


    loadBankStatements() {

        if (this.selectedTenant == undefined) {
            return;
        }
        this.firstLoaded = true;
        this.args.timeZoneInfoId = this.selectedZoneId;

        var newArgs = { ...this.args };

        newArgs.startDate = new Date(new Date(this.args.startDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();
        newArgs.endDate = new Date(new Date(this.args.endDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();

        localStorage.setItem('withdrawalpaymentchartreportargs', JSON.stringify(this.args));
        this.store.dispatch(new reportActions.GetWithdrawalPaymentWidget(newArgs));
    }

    refresh() {
        this.loadBankStatements();
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

    getPercentage(value: number, total: number){
        if(total == 0){
            return '0%';
        }

        var percentage = (value / total * 100).toFixed(2);
        return percentage + '%';

    }
}

