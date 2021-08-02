import { GenericHelper } from "../helpers/generic";
import { AgGridSearchArgs } from "./types";

export class InvoiceOwnerSetting {
        id: number;
        isActive: boolean;
        ownerGuid: string;
        startDate: Date;
        invoicePeriod: number;
        cartipayDepositRate: number;
        cartipalDepositRate: number;
        cartipalWithdrawalRate: number;
        withdrawalRate: number;
        tenantGuid: string;

        constructor(data?: any) {
                GenericHelper.populateData(this, data);
        }
}

export class Invoice {
        id: number;
        createDate: Date;
        dueDate: Date;
        ownerGuid: string;
        startDate: Date;
        endDate: Date;
        amount: number;
        tenantGuid: string;

        items: InvoiceDetail[];
}

export class InvoiceDetail {
        createDate: Date;
        invoiceId: number;
        merchantId: number;
        itemTypeId: number;
        totalCount: number;
        totalAmount: number;
        rate: number;
        amount: number;
}

export class InvoicePayment {
        id: number;
        amount: number;
        paymentDate: Date;
        paymentDateStr: string;
        paymentReference: string;
        createDate: Date;
        createDateStr: string;
        tenantGuid: string;
        ownerGuid: string;

        constructor(data?: any) {
                GenericHelper.populateData(this, data);
        }
}

export class InvoiceSearchArgs extends AgGridSearchArgs {
        ownerGuid: string;
        filterModel: any;
}

export class InvoicePaymentSearchArgs extends AgGridSearchArgs {
        ownerGuid: string;
        filterModel: any;
}

export enum InvoicePeriods {
        Daily = 1,
        Weekly = 2,
        Monthly = 3
}

export enum InvoiceDetailItemTypes {
        CartipayDeposit = 1,
        CartipalDeposit = 2,
        CartipalWithdrawal = 3,
        Withdrawal = 4
}

export const InvoicePeriodValues = [
        {
                value: InvoicePeriods.Daily,
                key: 'INVOICE-OWNER-SETTINGS.PERIOD.DAILY'
        },
        {
                value: InvoicePeriods.Weekly,
                key: 'INVOICE-OWNER-SETTINGS.PERIOD.WEEKLY'
        },
        {
                value: InvoicePeriods.Monthly,
                key: 'INVOICE-OWNER-SETTINGS.PERIOD.MONTHLY'
        }];

export const InvoiceDetailitemTypeValues = [
        {
                value: InvoiceDetailItemTypes.CartipayDeposit,
                key: 'INVOICE.GENERAL.PARDAKHTPAY-DEPOSIT',
                detail: 'INVOICE.GENERAL.PARDAKHTPAY-DEPOSIT-DETAIL'
        },
        {
                value: InvoiceDetailItemTypes.CartipalDeposit,
                key: 'INVOICE.GENERAL.PARDAKHTPAL-DEPOSIT',
                detail: 'INVOICE.GENERAL.PARDAKHTPAL-DEPOSIT-DETAIL'
        },
        {
                value: InvoiceDetailItemTypes.CartipalWithdrawal,
                key: 'INVOICE.GENERAL.PARDAKHTPAL-WITHDRAWAL',
                detail: 'INVOICE.GENERAL.PARDAKHTPAL-WITHDRAWAL-DETAIL'
        },
        {
                value: InvoiceDetailItemTypes.Withdrawal,
                key: 'INVOICE.GENERAL.WITHDRAWAL',
                detail: 'INVOICE.GENERAL.WITHDRAWAL-DETAIL'
        }
];