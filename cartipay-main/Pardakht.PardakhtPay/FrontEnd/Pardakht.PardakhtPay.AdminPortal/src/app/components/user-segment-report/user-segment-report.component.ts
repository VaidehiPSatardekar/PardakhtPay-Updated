import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import * as coreState from '../../core';
import { Store } from '@ngrx/store';
import { Observable, ReplaySubject } from 'rxjs';
import { fuseAnimations } from '../../core/animations';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as accountActions from '../../core/actions/account';
import * as userSegmentGroupActions from '../../core/actions/userSegmentGroup';
import * as reportActions from '../../core/actions/report';
import { UserSegmentGroup } from '../../models/user-segment-group';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { BooleanInverseFormatterComponent } from '../formatters/booleaninverseformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { DeleteButtonFormatterComponent } from '../formatters/deletebuttonformatter';
import { CellClickedEvent } from 'ag-grid-community';
import { UserSegmentReport, UserSegmentReportSearchArgs } from 'app/models/report';
import { TimeZone } from 'app/models/timeZone';
import { TimeZoneService } from 'app/core/services/timeZoneService/time-zone.service';

@Component({
  selector: 'app-user-segment-report',
    templateUrl: './user-segment-report.component.html',
    styleUrls: ['./user-segment-report.component.scss'],
    animations: fuseAnimations
})
export class UserSegmentReportComponent implements OnInit, OnDestroy {
    args: UserSegmentReportSearchArgs;

    groups$: Observable<UserSegmentGroup[]>;
    groups: UserSegmentGroup[];
    groupsLoading$: Observable<boolean>;
    groupsLoading: boolean = false;
    groupsError$: Observable<string>;

    deleteSuccess$: Observable<boolean>;
    deleteError$: Observable<string>;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    owners$: Observable<Owner[]>;
    owners: Owner[] = undefined;
    ownersLoading$: Observable<boolean>;
    ownersLoading: boolean = false;

    reportItems$: Observable<UserSegmentReport[]>;
    reportItems: UserSegmentReport[];
    reportLoading$: Observable<boolean>;
    reportLoading: boolean = false;
    reportError$: Observable<string>;

    selectedZoneId: string;
    zones: TimeZone[] = [];

    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    gridApi;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    allowAddUserSegmentGroup: boolean = false;

    constructor(private store: Store<coreState.State>,
        public snackBar: MatSnackBar,
        private translateService: TranslateService,
        private fuseConfigService: FuseConfigService,
        private router: Router,
        private timeZoneService: TimeZoneService) {

        var cachedArgs = localStorage.getItem('usersegmentreportargs');

        if (cachedArgs != undefined && cachedArgs != null) {
            this.args = JSON.parse(cachedArgs);

            if (!this.args.endDate || !this.args.startDate) {
                this.args.endDate = new Date().toISOString()
                this.args.startDate = new Date().toISOString();
                this.args.startDate = new Date(new Date().setDate(new Date(this.args.endDate).getDate() - 7)).toISOString();
            }
        }
        else {
            this.args = new UserSegmentReportSearchArgs();
            this.args.endDate = new Date().toISOString();
            this.args.startDate = new Date().toISOString();
            this.args.startDate = new Date(new Date().setDate(new Date(this.args.endDate).getDate() - 7)).toISOString();
        }
    }

    ngOnInit() {
        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            booleanInverseFormatterComponent: BooleanInverseFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent,
            deleteButtonFormatterComponent: DeleteButtonFormatterComponent
        };

        this.columnDefs = [
            {
                headerName: this.translateService.instant('USER-SEGMENT.LIST-COLUMNS.NAME'),
                field: "userSegmentId",
                sortable: true,
                resizable: true,
                width: 300,
                valueGetter: params => this.getGroupName(params.data.userSegmentId),
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.AMOUNT'),
                field: "amount",
                resizable: true,
                sortable: true,
                width: 300,
                cellRenderer: 'numberFormatterComponent',
                filter: "agNumberColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: '%',
                field: "percentage",
                width: 200,
                resizable: true,
                sortable: true,
                cellRenderer: 'numberFormatterComponent',
                valueGetter: params => this.getPercentage(params.data.amount)
            },
            {
                headerName: this.translateService.instant('USER-SEGMENT.LIST-COLUMNS.OWNER'),
                field: "ownerGuid",
                width: 200,
                resizable: true,
                sortable: true,
                valueGetter: params => this.getOwnerName(params.data.ownerGuid),
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }];

        this.zones = this.timeZoneService.getTimeZones();
        this.selectedZoneId = this.timeZoneService.getTimeZone().timeZoneId;

        this.store.dispatch(new userSegmentGroupActions.ClearErrors());

        this.owners$ = this.store.select(coreState.getOwners);
        this.ownersLoading$ = this.store.select(coreState.getAccountLoading);

        this.ownersLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.ownersLoading = l;
        });

        this.owners$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items != undefined) {
                this.owners = items;
            }
        });

        this.groups$ = this.store.select(coreState.getAllUserSegmentGroups);

        this.groups$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.groups = items;
        });

        this.groupsLoading$ = this.store.select(coreState.getUserSegmentGroupLoading);

        this.groupsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.groupsLoading = l;
        });

        this.groupsError$ = this.store.select(coreState.getAllUserSegmentGroupError);

        this.groupsError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;
            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new userSegmentGroupActions.GetAll());
                this.refresh();
            }
        });

        this.deleteSuccess$ = this.store.select(coreState.getUserSegmentGroupDeleteSuccess);
        this.deleteSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t == true) {
                this.store.dispatch(new userSegmentGroupActions.GetAll());
            }
        });

        this.deleteError$ = this.store.select(coreState.getUserSegmentGroupDeleteError);

        this.deleteError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.reportItems$ = this.store.select(coreState.getReportUserSegmentReport);
        this.reportItems$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items) {
                this.reportItems = items;
            }
        });

        this.reportLoading$ = this.store.select(coreState.getReportUserSegmentReportLoading);

        this.reportLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.reportLoading = l;
        });

        this.reportError$ = this.store.select(coreState.getReportUserSegmentReportError);
        this.reportError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    delete(id: number) {
        if (id) {
            this.store.dispatch(new userSegmentGroupActions.Delete(id));
        }
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

    }

    onCellClicked(e: CellClickedEvent) {

        if (!this.allowAddUserSegmentGroup) {
            return;
        }

        if (e && e.colDef && e.colDef.field != 'id') {
            var selectedRows = this.gridApi.getSelectedRows();

            if (selectedRows.length > 0) {
                var selected = selectedRows[0];

                this.router.navigate(['/usersegmentgroup/' + selected.id]);
            }
        }
    }

    refresh() {
        if (this.args == undefined || this.args == null) {
            return;
        }

        this.args.timeZoneInfoId = this.selectedZoneId;

        var newArgs = { ...this.args };

        newArgs.startDate = new Date(new Date(this.args.startDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();
        newArgs.endDate = new Date(new Date(this.args.endDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();

        localStorage.setItem('usersegmentreportargs', JSON.stringify(this.args));
        this.store.dispatch(new reportActions.GetUserSegmentReport(newArgs));
    }

    ngOnDestroy(): void {
        this.store.dispatch(new userSegmentGroupActions.ClearErrors());
        this.store.dispatch(new reportActions.ClearAll());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    getOwnerName(ownerGuid: string): string {
        if (this.owners == undefined || this.owners == null) {
            return '';
        }

        var owner = this.owners.find(t => t.accountId == ownerGuid);

        if (owner == null || owner == undefined) {
            return '';
        }

        return owner.firstName + ' ' + owner.lastName;
    }

    getGroupName(userSegmentId: number): string {
        if (this.groups == undefined || this.groups == null) {
            return '';
        }

        var group = this.groups.find(t => t.id == userSegmentId);

        if (group != null) {
            return group.name;
        }

        return '';
    }

    getPercentage(value: number): number {
        if (this.reportItems == undefined) {
            return 0;
        }

        var sum = 0;

        for (var i = 0; i < this.reportItems.length; i++) {
            sum += this.reportItems[i].amount;
        }

        if (sum == 0) {
            return 0;
        }

        return Number((value / sum * 100).toFixed(2));
    }

    onGridReady(params) {
        this.gridApi = params.api;
    }

    setTimeZone() {
        let zone = this.zones.find(t => t.timeZoneId == this.selectedZoneId);
        this.timeZoneService.setTimeZone(zone);
        this.refresh();
    }
}


