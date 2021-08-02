import { GenericHelper } from '../helpers/generic';

export class DepositBreakDownReport {
    BreakDownDate: string;
    PaymentType: string;
    Amount: number;
    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }

}

export class DashBoardWidget {
    ranges: any[];
    currentRange: string;
    detail: string;
    title: string;
    data: WidgetData;
    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class WidgetData {
    coreData: WidgetDataCore[];
    extra: WidgetDataCore[];
    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class WidgetDataCore {
    label: string;
    count: any[];
    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class DashBoardChartWidget {
    ranges: any[];
    title: string;
    mainChart: any[];
    currentRange: string;
    xAxis: boolean;
    yAxis: boolean;
    gradient: boolean;
    legend: boolean;
    showXAxisLabel: boolean;
    xAxisLabel: string;
    showYAxisLabel: boolean;
    yAxisLabel: string;
    scheme: Scheme;
    explodeSlices: boolean;
    labels: boolean;
    doughnut: boolean;
    footerLeft: FooterForPie;
    footerRight: FooterForPie;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class WidgetChartSeries {
    name: string;
    series: WidgetChartSeriesData[];
    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class WidgetChartSeriesData {
    name: string;
    value: number;
    extra: number;
    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class Scheme {
    domain: string[];
    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class FooterForPie {
    count: string[];
    title: string;
    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class DashboardQuery {
    constructor(public tenantGuid: string, public dateType: string, public argument = '', public timeZoneInfoId = 'Iran Standard Time') { }
}

export class DashboardAccountStatusDTO {

    bankLoginId: number;

    bankAccountId: number;

    friendlyName: string;

    accountNo: string;

    bankName: string;

    isBlocked: boolean;

    status: string;

    cardHolderName: string;

    cardNumber: string;

    accountBalance: number;

    blockedBalance: number;

    normalWithdrawable: number;

    payaWithdrawable: number;

    satnaWithdrawable: number;

    totalDepositToday: number;

    totalWithdrawalToday: number;

    pendingWithdrawalAmount: number;
}

export class DashboardMerchantTransactionReportDTO {
    title: string;
    transactionSum: number;
    transactionCount: number;
    withdrawalSum: number;
    withdrawalCount: number;
}

export class DashboardTransactionPaymentTypeReportDTO {
    title: string;
    transactionSum: number;
    transactionCount: number;
    withdrawalSum: number;
    withdrawalCount: number;
}

