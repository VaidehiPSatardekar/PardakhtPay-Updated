import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule, Optional, SkipSelf } from '@angular/core';

import { FuseConfigService, FUSE_CONFIG } from './../@fuse/services/config.service';

@NgModule({
    imports  : [
        CommonModule
    ],
providers: [
    FuseConfigService 
]})
export class FuseModule
{
    constructor(@Optional() @SkipSelf() parentModule: FuseModule)
    {
        if ( parentModule )
        {
            throw new Error('FuseModule is already loaded. Import it in the AppModule only!');
        }
    }

    static forRoot(config): ModuleWithProviders
    {
        return {
            ngModule : FuseModule,
            providers: [
                {
                    provide : FUSE_CONFIG,
                    useValue: config
                }
            ]
        };
    }
}
