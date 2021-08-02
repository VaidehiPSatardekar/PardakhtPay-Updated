import { CommonModule } from '@angular/common';
import { NgModule, Optional, SkipSelf } from '@angular/core';

import { EnvironmentService } from './environment.service';
import { throwIfAlreadyLoaded } from './module-import-guard';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [],
  providers: [
    EnvironmentService
  ]
})
export class CoreEnvironmentModule {
  constructor( @Optional() @SkipSelf() parentModule: CoreEnvironmentModule) {
    throwIfAlreadyLoaded(parentModule, 'CoreModule');
  }
}
