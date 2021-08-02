import { NgModule } from '@angular/core';

import { LoginModule } from 'app/main/pages/authentication/login/login.module';
import { ResetPasswordModule } from './authentication/reset-password/reset-password.module';

@NgModule({
    imports: [
        // Authentication
        LoginModule,
        ResetPasswordModule
    ],
    declarations: []
})
export class PagesModule
{

}
