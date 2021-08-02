import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatButtonModule, MatFormFieldModule, MatIconModule, MatInputModule, MatProgressBarModule } from '@angular/material';

import { FuseSharedModule } from '../../../../../@fuse/shared.module';

import { ResetPasswordComponent } from 'app/main/pages/authentication/reset-password/reset-password.component';
import { TranslateModule } from '@ngx-translate/core';


@NgModule({
    declarations: [
        ResetPasswordComponent
    ],
    imports     : [
        MatButtonModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        FuseSharedModule,
        MatProgressBarModule,
        TranslateModule,
        RouterModule
    ]
})
export class ResetPasswordModule
{
}
