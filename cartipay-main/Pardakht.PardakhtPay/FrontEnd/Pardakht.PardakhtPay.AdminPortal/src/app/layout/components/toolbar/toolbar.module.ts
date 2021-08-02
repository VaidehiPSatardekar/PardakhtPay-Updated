import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatButtonModule, MatIconModule, MatMenuModule, MatToolbarModule } from '@angular/material';

import { FuseSearchBarModule } from '../../../../@fuse/components/search-bar/search-bar.module';
import { FuseShortcutsModule } from '../../../../@fuse/components/shortcuts/shortcuts.module';
import { FuseSharedModule } from '../../../../@fuse/shared.module';

import { ToolbarComponent } from 'app/layout/components/toolbar/toolbar.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
    declarations: [
        ToolbarComponent
    ],
    imports     : [
        RouterModule,
        MatButtonModule,
        MatIconModule,
        MatMenuModule,
        MatToolbarModule,

        FuseSharedModule,
        FuseSearchBarModule,
        FuseShortcutsModule,        
        RouterModule,
        TranslateModule
    ],
    exports     : [
        ToolbarComponent
    ]
})
export class ToolbarModule
{
}
