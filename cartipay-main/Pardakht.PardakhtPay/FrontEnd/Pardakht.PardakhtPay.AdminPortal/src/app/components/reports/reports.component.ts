import { Component, OnInit } from '@angular/core';
import { fuseAnimations } from '../../core/animations';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
    styleUrls: ['./reports.component.scss'],
    animations: fuseAnimations
})
export class ReportsComponent implements OnInit {
    tenantBalance: boolean = false;
    userSegmentReport: boolean = false;
    depositWithdrawal: boolean = false;
    withdrawalPaymentBreakdown: boolean = false;
    blockedCards: boolean = false;
    cardHolderNames: boolean = false;

    constructor(private accountService: AccountService) {
        this.tenantBalance = this.accountService.isUserAuthorizedForTask(permissions.TenantCurrentBalance);
        this.userSegmentReport = this.accountService.isUserAuthorizedForTask(permissions.UserSegmentReport);
        this.depositWithdrawal = this.accountService.isUserAuthorizedForTask(permissions.DepositWithdrawalReport);
        this.withdrawalPaymentBreakdown = this.accountService.isUserAuthorizedForTask(permissions.WithdrawalPayments);
        this.blockedCards = this.accountService.isUserAuthorizedForTask(permissions.ListBlockedPhoneNumbers);
        this.cardHolderNames = this.accountService.isUserAuthorizedForTask(permissions.ListCardHolderNames);
    }

  ngOnInit() {
  }

}
