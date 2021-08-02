import { Injectable } from '@angular/core';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';
import { AngularCsv } from 'angular7-csv/dist/Angular-csv';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { autoTable as AutoTable } from 'jspdf-autotable';


const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';

@Injectable({
  providedIn: 'root'
})
export class ExportService {
  constructor() { }
  csvOptions = {
    fieldSeparator: ',',
    quoteStrings: '"',
    decimalseparator: '.',
    showLabels: true,
    showTitle: true,
    title: "",
    useBom: true,
    noDownload: false,
    headers: ""
  };
  public exportAsExcelFile(json: any[], excelFileName: string, header?: any[]): void {
    if (header.length > 0)
      json.unshift(header);
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(json);

    const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
    const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });

    this.saveAsExcelFile(excelBuffer, excelFileName);
  }

  private saveAsExcelFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE
    });
    FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
  }

  public exportAsCSVFile(json: any[], csvFileName: string, headers: any, title: string) {
    this.csvOptions.headers = headers;
    this.csvOptions.title = title;
    new AngularCsv(json, csvFileName + '_export_' + new Date().getTime(), this.csvOptions);
  }

  public exportAsPDFFile(arrayList: any[], pdfFileName: string, headers: any) {
    var head = [headers];
    const doc = new jsPDF('l', 'pt');

    ((doc as any).autoTable as AutoTable)({
      head: head,
      body: arrayList,
      didDrawCell: data => {

      },
    })

    doc.save(pdfFileName + '_export_' + new Date().getTime());
  }
}
