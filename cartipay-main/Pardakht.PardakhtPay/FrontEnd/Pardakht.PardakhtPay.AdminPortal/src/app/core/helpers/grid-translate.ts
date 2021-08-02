
import { TranslateService } from '@ngx-translate/core';
import { Injectable } from '@angular/core';



@Injectable({
    providedIn: 'root'
})
export class GridTranslate {


    constructor(private translateService: TranslateService) { }


    translateAgGrid() {

        return {
            page: this.translateService.instant('page'),
            more: this.translateService.instant('more'),
            to: this.translateService.instant('to'),
            of: this.translateService.instant('of'),
            next: this.translateService.instant('next'),
            last: this.translateService.instant('last'),
            first: this.translateService.instant('first'),
            previous: this.translateService.instant('previous'),
            loadingOoo: this.translateService.instant('loadingOoo'),
            selectAll: this.translateService.instant('selectAll'),
            searchOoo: this.translateService.instant('searchOoo'),
            blanks: this.translateService.instant('blanks'),
            filterOoo: this.translateService.instant('filterOoo'),
            applyFilter: this.translateService.instant('applyFilter'),
            equals: this.translateService.instant('equals'),
            notEqual: this.translateService.instant('notEqual'),
            lessThan: this.translateService.instant('lessThan'),
            greaterThan: this.translateService.instant('greaterThan'),
            lessThanOrEqual: this.translateService.instant('lessThanOrEqual'),
            greaterThanOrEqual: this.translateService.instant('greaterThanOrEqual'),
            inRange: this.translateService.instant('inRange'),
            contains: this.translateService.instant('contains'),
            notContains: this.translateService.instant('notContains'),
            startsWith: this.translateService.instant('startsWith'),
            endsWith: this.translateService.instant('endsWith'),
            andCondition: this.translateService.instant('andCondition'),
            orCondition: this.translateService.instant('orCondition'),
            group: this.translateService.instant('group'),
            columns: this.translateService.instant('columns'),
            filters: this.translateService.instant('filters'),
            rowGroupColumns: this.translateService.instant('rowGroupColumns'),
            rowGroupColumnsEmptyMessage: this.translateService.instant('rowGroupColumnsEmptyMessage'),
            valueColumns: this.translateService.instant('valueColumns'),
            pivotMode: this.translateService.instant('pivotMode'),
            groups: this.translateService.instant('groups'),
            values: this.translateService.instant('values'),
            pivots: this.translateService.instant('pivots'),
            valueColumnsEmptyMessage: this.translateService.instant('valueColumnsEmptyMessage'),
            pivotColumnsEmptyMessage: this.translateService.instant('pivotColumnsEmptyMessage'),
            toolPanelButton: this.translateService.instant('toolPanelButton'),
            noRowsToShow: this.translateService.instant('noRowsToShow'),
            enabled: this.translateService.instant('enabled'),
            pinColumn: this.translateService.instant('pinColumn'),
            valueAggregation: this.translateService.instant('valueAggregation'),
            autosizeThiscolumn: this.translateService.instant('autosizeThiscolumn'),
            autosizeAllColumns: this.translateService.instant('autosizeAllColumns'),
            groupBy: this.translateService.instant('groupBy'),
            ungroupBy: this.translateService.instant('ungroupBy'),
            resetColumns: this.translateService.instant('resetColumns'),
            expandAll: this.translateService.instant('expandAll'),
            collapseAll: this.translateService.instant('collapseAll'),
            toolPanel: this.translateService.instant('toolPanel'),
            export: this.translateService.instant('export'),
            csvExport: this.translateService.instant('csvExport'),
            excelExport: this.translateService.instant('excelExport'),
            excelXmlExport: this.translateService.instant('excelXmlExport'),
            pivotChartAndPivotMode: this.translateService.instant('pivotChartAndPivotMode'),
            pivotChart: this.translateService.instant('pivotChart'),
            chartRange: this.translateService.instant('chartRange'),
            columnChart: this.translateService.instant('columnChart'),
            groupedColumn: this.translateService.instant('groupedColumn'),
            stackedColumn: this.translateService.instant('stackedColumn'),
            normalizedColumn: this.translateService.instant('normalizedColumn'),
            barChart: this.translateService.instant('barChart'),
            groupedBar: this.translateService.instant('groupedBar'),
            stackedBar: this.translateService.instant('stackedBar'),
            normalizedBar: this.translateService.instant('normalizedBar'),
            pieChart: this.translateService.instant('pieChart'),
            pie: this.translateService.instant('pie'),
            doughnut: this.translateService.instant('doughnut'),
            line: this.translateService.instant('line'),
            xyChart: this.translateService.instant('xyChart'),
            scatter: this.translateService.instant('scatter'),
            bubble: this.translateService.instant('bubble'),
            areaChart: this.translateService.instant('areaChart'),
            area: this.translateService.instant('area'),
            stackedArea: this.translateService.instant('stackedArea'),
            normalizedArea: this.translateService.instant('normalizedArea'),
            pinLeft: this.translateService.instant('pinLeft'),
            pinRight: this.translateService.instant('pinRight'),
            noPin: this.translateService.instant('noPin'),
            sum: this.translateService.instant('sum'),
            min: this.translateService.instant('min'),
            max: this.translateService.instant('max'),
            none: this.translateService.instant('none'),
            count: this.translateService.instant('count'),
            average: this.translateService.instant('average'),
            filteredRows: this.translateService.instant('filteredRows'),
            selectedRows: this.translateService.instant('selectedRows'),
            totalRows: this.translateService.instant('totalRows'),
            totalAndFilteredRows: this.translateService.instant('totalAndFilteredRows'),
            copy: this.translateService.instant('copy'),
            copyWithHeaders: this.translateService.instant('copyWithHeaders'),
            ctrlC: this.translateService.instant('ctrlC'),
            paste: this.translateService.instant('paste'),
            ctrlV: this.translateService.instant('ctrlV'),
            pivotChartTitle: this.translateService.instant('pivotChartTitle'),
            rangeChartTitle: this.translateService.instant('rangeChartTitle'),
            settings: this.translateService.instant('settings'),
            data: this.translateService.instant('data'),
            format: this.translateService.instant('format'),
            categories: this.translateService.instant('categories'),
            series: this.translateService.instant('series'),
            xyValues: this.translateService.instant('xyValues'),
            paired: this.translateService.instant('paired'),
            axis: this.translateService.instant('axis'),
            color: this.translateService.instant('color'),
            thickness: this.translateService.instant('thickness'),
            xRotation: this.translateService.instant('xRotation'),
            yRotation: this.translateService.instant('yRotation'),
            ticks: this.translateService.instant('ticks'),
            width: this.translateService.instant('width'),
            length: this.translateService.instant('length'),
            padding: this.translateService.instant('padding'),
            chart: this.translateService.instant('chart'),
            title: this.translateService.instant('title'),
            background: this.translateService.instant('background'),
            font: this.translateService.instant('font'),
            top: this.translateService.instant('top'),
            right: this.translateService.instant('right'),
            bottom: this.translateService.instant('bottom'),
            left: this.translateService.instant('left'),
            labels: this.translateService.instant('labels'),
            size: this.translateService.instant('size'),
            minSize: this.translateService.instant('minSize'),
            maxSize: this.translateService.instant('maxSize'),
            legend: this.translateService.instant('legend'),
            position: this.translateService.instant('position'),
            markerSize: this.translateService.instant('markerSize'),
            markerStroke: this.translateService.instant('markerStroke'),
            markerPadding: this.translateService.instant('markerPadding'),
            itemPaddingX: this.translateService.instant('itemPaddingX'),
            itemPaddingY: this.translateService.instant('itemPaddingY'),
            strokeWidth: this.translateService.instant('strokeWidth'),
            offset: this.translateService.instant('offset'),
            offsets: this.translateService.instant('offsets'),
            tooltips: this.translateService.instant('tooltips'),
            callout: this.translateService.instant('callout'),
            markers: this.translateService.instant('markers'),
            shadow: this.translateService.instant('shadow'),
            blur: this.translateService.instant('blur'),
            xOffset: this.translateService.instant('xOffset'),
            yOffset: this.translateService.instant('yOffset'),
            lineWidth: this.translateService.instant('lineWidth'),
            normal: this.translateService.instant('normal'),
            bold: this.translateService.instant('bold'),
            italic: this.translateService.instant('italic'),
            boldItalic: this.translateService.instant('boldItalic'),
            predefined: this.translateService.instant('predefined'),
            fillOpacity: this.translateService.instant('fillOpacity'),
            strokeOpacity: this.translateService.instant('strokeOpacity'),
            columnGroup: this.translateService.instant('columnGroup'),
            barGroup: this.translateService.instant('barGroup'),
            pieGroup: this.translateService.instant('pieGroup'),
            lineGroup: this.translateService.instant('lineGroup'),
            scatterGroup: this.translateService.instant('scatterGroup'),
            areaGroup: this.translateService.instant('areaGroup'),
            groupedColumnTooltip: this.translateService.instant('groupedColumnTooltip'),
            stackedColumnTooltip: this.translateService.instant('stackedColumnTooltip'),
            normalizedColumnTooltip: this.translateService.instant('normalizedColumnTooltip'),
            groupedBarTooltip: this.translateService.instant('groupedBarTooltip'),
            stackedBarTooltip: this.translateService.instant('stackedBarTooltip'),
            normalizedBarTooltip: this.translateService.instant('normalizedBarTooltip'),
            pieTooltip: this.translateService.instant('pieTooltip'),
            doughnutTooltip: this.translateService.instant('doughnutTooltip'),
            lineTooltip: this.translateService.instant('lineTooltip'),
            groupedAreaTooltip: this.translateService.instant('groupedAreaTooltip'),
            stackedAreaTooltip: this.translateService.instant('stackedAreaTooltip'),
            normalizedAreaTooltip: this.translateService.instant('normalizedAreaTooltip'),
            scatterTooltip: this.translateService.instant('scatterTooltip'),
            bubbleTooltip: this.translateService.instant('bubbleTooltip'),
            noDataToChart: this.translateService.instant('noDataToChart'),
            pivotChartRequiresPivotMode: this.translateService.instant('pivotChartRequiresPivotMode')

        };
    }
}