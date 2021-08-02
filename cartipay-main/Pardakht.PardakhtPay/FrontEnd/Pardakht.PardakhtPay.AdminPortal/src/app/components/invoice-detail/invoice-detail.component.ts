import { Component, OnInit, Input, EventEmitter, Output, OnDestroy } from '@angular/core';
import { Invoice, InvoiceDetailitemTypeValues, InvoiceDetail } from 'app/models/invoice';
import { Router, ActivatedRoute } from '@angular/router';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { TranslateService } from '@ngx-translate/core';
import { fuseAnimations } from '../../core/animations';
import { distinctUntilChanged, debounceTime, take, filter, takeUntil } from 'rxjs/operators';
import { Observable, ReplaySubject } from 'rxjs';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import * as invoiceActions from '../../core/actions/invoice';
import { MatSnackBar } from '@angular/material';
import { Owner } from 'app/models/account.model';
import { Merchant } from 'app/models/merchant-model';

@Component({
  selector: 'app-invoice-detail',
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.scss'],
  animations: fuseAnimations
})
export class InvoiceDetailComponent implements OnInit, OnDestroy {

  @Input() selected: Invoice;
  @Input() owners: Owner[];
  @Input() merchants: Merchant[];
  @Input() summary: boolean;
  @Output() closed: EventEmitter<any> = new EventEmitter<any>();

  id: number;

  detailLoading$: Observable<boolean>;

  detailLoading: boolean;

  detail$: Observable<Invoice>;
  detailError$: Observable<string>;
  invoice: Invoice;
  owner: Owner = undefined;

  private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

  constructor(private router: Router,
    private fuseConfigService: FuseConfigService,
    private translateService: TranslateService,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private store: Store<coreState.State>) {
  }

  ngOnInit() {
    this.detailLoading$ = this.store.select(coreState.getInvoiceDetailLoading);

    this.detailLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
      this.detailLoading = l;
    });

    this.detail$ = this.store.select(coreState.getInvoiceDetail);
    this.detail$.pipe(takeUntil(this.destroyed$)).subscribe(d => {
      this.invoice = d;

      if (this.invoice && this.summary == true) {
        console.log(this.summary);
        var items: InvoiceDetail[] = [];

          this.invoice.items.forEach(item => {
            var innerItem = items.find(t => t.itemTypeId == item.itemTypeId);

            if (innerItem == null) {
              innerItem = new InvoiceDetail();
              innerItem.itemTypeId = item.itemTypeId;
              innerItem.amount = item.amount;
              innerItem.totalAmount = item.totalAmount;
              innerItem.rate = item.rate;
              innerItem.totalCount = item.totalCount;

              items.push(innerItem);
            }
            else {
              innerItem.amount += item.amount;
              innerItem.totalAmount += item.totalAmount;
              innerItem.totalCount += item.totalCount;
            }
          });

          this.invoice.items = items;
      }

      if (this.invoice && this.owners) {
        var o = this.owners.find(t => t.accountId == this.invoice.ownerGuid);

        if (o != null) {
          this.owner = o;
        }
        else {
          this.owner = undefined;
        }
      }
      else {
        this.owner = undefined;
      }
    });

    this.detailError$ = this.store.select(coreState.getMerchantDetailError);

    this.detailError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
      if (error) {
        this.openSnackBar(error);
      }
    });

    if (this.selected != null && this.selected != undefined) {
      this.id = this.selected.id;

      this.store.dispatch(new invoiceActions.GetDetails(this.id));
    }
    else {
      this.route.params.subscribe(params => {

        if (params.id) {
          this.id = params.id;

          this.store.dispatch(new invoiceActions.GetDetails(this.id));
        }
      });
    }
  }

  getOwnerName(ownerGuid): string {
    if (ownerGuid) {
      if (this.owners) {
        var owner = this.owners.find(t => t.accountId == ownerGuid);

        if (owner != null) {
          return owner.username;
        }
      }
    }
    return '';
  }

  getDetailItemTypeText(id: number): string {
    if (id) {
      var item = InvoiceDetailitemTypeValues.find(t => t.value == id);

      if (item != null) {
        return this.translateService.instant(item.key);
      }
    }

    return '';
  }

  getDetailItemTypeDetailText(id: number): string {
    if (id) {
      var item = InvoiceDetailitemTypeValues.find(t => t.value == id);

      if (item != null) {
        return this.translateService.instant(item.detail);
      }
    }

    return '';
  }

  getMerchantName(merchantId: number): string {
    if (this.merchants) {
      var merchant = this.merchants.find(t => t.id == merchantId);

      if (merchant != null) {
        return merchant.title;
      }
    }

    return '';
  }

  ngOnDestroy(): void {
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

  onBack() {
    if (this.selected == null || this.selected == undefined) {
      this.router.navigate(['/invoices']);
    }
    else {
      this.closed.emit(this.selected);
    }

  }

}
