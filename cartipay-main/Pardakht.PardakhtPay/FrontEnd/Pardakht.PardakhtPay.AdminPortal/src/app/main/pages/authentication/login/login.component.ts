import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { FuseConfigService } from '../../../../../@fuse/services/config.service';
import { fuseAnimations } from '../../../../../@fuse/animations'
import { Store } from '@ngrx/store';
import * as coreState from 'app/core';
import * as accountActions from 'app/core/actions/account';
//import { LoginForm } from 'app/models/account.model';
import { Observable } from 'rxjs/internal/Observable';
import { ActivatedRoute, Router } from '@angular/router';
import { getUserLoading } from '../../../../core/index';
import { getLoginError } from '../../../../core/index';
import { LoginForm } from '../../../../core/models/user-management.model';
import { Login , ForgotPassword } from '../../../../core/actions/user';


@Component({
    selector     : 'login',
    templateUrl  : './login.component.html',
    styleUrls    : ['./login.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations   : fuseAnimations
})
export class LoginComponent implements OnInit
{
    loginForm: FormGroup;
    loading$: Observable<boolean>;
    error$: Observable<string>;
    loginAs: boolean = false;

    /**
     * Constructor
     *
     * @param {FuseConfigService} _fuseConfigService
     * @param {FormBuilder} _formBuilder
     */
    constructor(
        private _fuseConfigService: FuseConfigService,
        private _formBuilder: FormBuilder,
        private route: ActivatedRoute,
        private store: Store<coreState.State>, 
    )
    {
        // Configure the layout
        this._fuseConfigService.config = {
            layout: {
                navbar   : {
                    hidden: true
                },
                toolbar  : {
                    hidden: true
                },
                footer   : {
                    hidden: true
                },
                sidepanel: {
                    hidden: true
                }
            }
        };
    }

    login(){
        this.store.dispatch(new Login(new LoginForm('', this.loginForm.value)));
    }
    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void
    {
        this.route.queryParams.subscribe(params => {
            if(params.as){
                if(params.as == 1){
                    this.loginAs = true;
                }
            }
        });

        this.loading$ = this.store.select(getUserLoading);
        this.error$ = this.store.select(getLoginError);


        this.loginForm = this._formBuilder.group({
            username   : ['', [Validators.required]],
            password: ['', Validators.required],
            loginAsUsername: ['']
        });
    }
}
