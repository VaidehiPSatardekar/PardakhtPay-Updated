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
import * as blockedCardNumberActions from '../../core/actions/blockedCardNumber';
import { BlockedCardNumber } from '../../models/blocked-card-number';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { BooleanInverseFormatterComponent } from '../formatters/booleaninverseformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { DeleteButtonFormatterComponent } from '../formatters/deletebuttonformatter';
import { CellClickedEvent } from 'ag-grid-community';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';
import { DateFormatterComponent } from '../formatters/dateformatter';

@Component({
  selector: 'app-blocked-card-number-list',
  templateUrl: './blocked-card-number-list.component.html',
  styleUrls: ['./blocked-card-number-list.component.scss'],
  animations: fuseAnimations
})
export class BlockedCardNumberListComponent implements OnInit, OnDestroy {

  items$: Observable<BlockedCardNumber[]>;
  items: BlockedCardNumber[];
  itemsLoading$: Observable<boolean>;
  itemsLoading: boolean = false;
  itemsError$: Observable<string>;

  deleteSuccess$: Observable<boolean>;
  deleteError$: Observable<string>;

  selectedTenant$: Observable<Tenant>;
  selectedTenant: Tenant;

  columnDefs;
  enableRtl: boolean = false;
  frameworkComponents;

  gridApi;

  allowAddBlockedCardNumber: boolean = false;
  allowEditBlockedCardNumber: boolean = false;
  allowDeleteBlockedCardNumber: boolean = false;

  private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

  constructor(private store: Store<coreState.State>,
      public snackBar: MatSnackBar,
      private accountService: AccountService,
      private translateService: TranslateService,
      private fuseConfigService: FuseConfigService,
      private router: Router) {

      this.allowAddBlockedCardNumber = this.accountService.isUserAuthorizedForTask(permissions.AddBlockedCardNumber);
      this.allowEditBlockedCardNumber = this.accountService.isUserAuthorizedForTask(permissions.AddBlockedCardNumber);
      this.allowDeleteBlockedCardNumber = this.accountService.isUserAuthorizedForTask(permissions.AddBlockedCardNumber);
  }

  ngOnInit() {
      this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

          this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
      });

      this.frameworkComponents = {
          deleteButtonFormatterComponent: DeleteButtonFormatterComponent,
          dateFormatterComponent: DateFormatterComponent
      };

      this.columnDefs = [
          {
              headerName: this.translateService.instant('GENERAL.ID'),
              field: "id",
              sortable: true,
              resizable: true,
              width: 150,
              filter: "agNumberColumnFilter",
              filterParams: {
                  applyButton: true,
                  clearButton: true
              }
          },
          {
              headerName: this.translateService.instant('BLOCKED-CARD-NUMBER.LIST-COLUMNS.CARD-NUMBER'),
              field: "cardNumber",
              width: 200,
              resizable: true,
              sortable: true,
              filter: "agTextColumnFilter",
              filterParams: {
                  applyButton: true,
                  clearButton: true
              }
          },
          {
              headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.DATE'),
              field: "blockedDate",
              sortable: true,
              resizable: true,
              cellRenderer: 'dateFormatterComponent',
              width: 150
          },
          {
              headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.DELETE'),
              field: "id",
              resizable: true,
              sortable: false,
              width: 100,
              cellRenderer: 'deleteButtonFormatterComponent',
              cellRendererParams: {
                  onButtonClick: this.delete.bind(this)
              },
              hide: !this.allowDeleteBlockedCardNumber
          }];

      this.store.dispatch(new blockedCardNumberActions.ClearErrors());

      this.items$ = this.store.select(coreState.getAllBlockedCardNumbers);

      this.items$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
          if (items == undefined) {
              this.items = [];
          }
          else {
              this.items = items;
          }
      });

      this.itemsLoading$ = this.store.select(coreState.getBlockedCardNumberLoading);

      this.itemsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
          this.itemsLoading = l;
      });

      this.itemsError$ = this.store.select(coreState.getAllBlockedCardNumberError);

      this.itemsError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
          if (error) {
              this.openSnackBar(error);
          }
      });

      this.deleteSuccess$ = this.store.select(coreState.getBlockedCardNumberDeleteSuccess);
      this.deleteSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {

          if (t == true) {
              this.store.dispatch(new blockedCardNumberActions.GetAll());
          }
      });

      this.deleteError$ = this.store.select(coreState.getBlockedCardNumberDeleteError);

      this.deleteError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
          if (error) {
              this.openSnackBar(error);
          }
      });

      this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

      this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
          this.selectedTenant = t;
          if (t && t.tenantDomainPlatformMapGuid) {
              this.store.dispatch(new blockedCardNumberActions.GetAll());
          }
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

  }

  delete(id: number) {
      if (id) {
          this.store.dispatch(new blockedCardNumberActions.Delete(id));
      }
  }

  onCellClicked(e: CellClickedEvent) {
      if(this.allowEditBlockedCardNumber){
      if (e && e.colDef && e.colDef.field != 'id') {
          var selectedRows = this.gridApi.getSelectedRows();

          if (selectedRows.length > 0) {
              var selected = selectedRows[0];

              this.router.navigate(['/blockedcardnumber/' + selected.id]);
          }
      }
     }
  }

  ngOnDestroy(): void {
      this.store.dispatch(new blockedCardNumberActions.ClearErrors());
      this.destroyed$.next(true);
      this.destroyed$.complete();
  }

  onGridReady(params) {
      this.gridApi = params.api;
  }
}


