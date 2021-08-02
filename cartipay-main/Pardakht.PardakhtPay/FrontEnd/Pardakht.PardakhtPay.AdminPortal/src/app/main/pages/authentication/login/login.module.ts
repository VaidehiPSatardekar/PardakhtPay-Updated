import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatButtonModule, MatCheckboxModule, MatFormFieldModule, MatIconModule, MatInputModule, MatProgressBarModule } from '@angular/material';

import { FuseSharedModule } from '../../../../../@fuse/shared.module';

import { LoginComponent } from 'app/main/pages/authentication/login/login.component';
import { TranslateModule } from '@ngx-translate/core';


@NgModule({
    declarations: [
        LoginComponent
    ],
    imports     : [
        MatButtonModule,
        MatCheckboxModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        MatProgressBarModule,
        FuseSharedModule,
        TranslateModule,
        RouterModule
    ],
    exports     : [
        LoginComponent
    ]
})
export class LoginModule
{
}


