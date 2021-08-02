import { Component } from '@angular/core';

@Component({
  selector: 'app-number-formatter-cell',
  template: `
    <span>{{params.value | number : '1.0-2'}}</span>
  `
})
export class NumberFormatterComponent {
  params: any;

  agInit(params: any): void {
    this.params = params;
  }
}