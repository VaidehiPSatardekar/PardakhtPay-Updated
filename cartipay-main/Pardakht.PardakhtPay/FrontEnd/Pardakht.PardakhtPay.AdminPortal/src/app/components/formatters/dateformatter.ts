import { Component } from '@angular/core';

@Component({
  selector: 'app-date-formatter-cell',
  template: `
    <span class="url content-right">{{params.value | localizedDate}}</span>
  `
})
export class DateFormatterComponent {
  params: any;

  agInit(params: any): void {
    this.params = params;
  }
}