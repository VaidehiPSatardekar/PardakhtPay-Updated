import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { Subject, ReplaySubject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import * as _ from 'lodash';

import { FuseConfigService } from '../../../../@fuse/services/config.service';
import { FuseSidebarService } from '../../../../@fuse/components/sidebar/sidebar.service';

import { navigation } from 'app/navigation/navigation';
import * as coreState from '../../../core';
import { Store } from '@ngrx/store';

import * as account from '../../../core/actions/account';
import * as tenantActions from '../../../core/actions/tenant';
import { Observable } from 'rxjs/internal/Observable';
import { User } from '../../../models/user-management.model';
import { Router } from '@angular/router';
import { Tenant } from '../../../models/tenant';
import { TenantService } from '../../../core/services/tenant/tenant.service';
import { ChangePasswordComponent } from '../../../components/change-password/change-password.component';
import { MatDialog } from '@angular/material';
import * as merchantActions from '../../../core/actions/merchant';


@Component({
    selector     : 'toolbar',
    templateUrl  : './toolbar.component.html',
    styleUrls    : ['./toolbar.component.scss'],
    encapsulation: ViewEncapsulation.None
})

export class ToolbarComponent implements OnInit, OnDestroy
{
    horizontalNavbar: boolean;
    rightNavbar: boolean;
    hiddenNavbar: boolean;
    languages: any;
    navigation: any;
    selectedLanguage: any;
    userStatusOptions: any[];

    tenants$: Observable<Tenant[]>;
    tenants: Tenant[] = [];
    getAllTenantError$: Observable<string>;

    isProviderAdmin$: Observable<boolean>;
    isProviderAdmin: boolean;

    isProviderUser$: Observable<boolean>;
    isProviderUser: boolean;

    isTenantAdmin$: Observable<boolean>;
    isTenantAdmin: boolean;

    isStandardUser$: Observable<boolean>;
    isStandardUser: boolean;

    accountGuid$: Observable<string>;
    accountGuid: string;

    username$: Observable<string>;
    username: string;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    tenantGuid$: Observable<string>;
    tenantGuid: string;

    //account$: Observable<User>;
    //account: User;
    name: string;

    //private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    fuseSettings: any;


    // Private
    private _unsubscribeAll: Subject<any>;

    /**
     * Constructor
     *
     * @param {FuseConfigService} _fuseConfigService
     * @param {FuseSidebarService} _fuseSidebarService
     * @param {TranslateService} _translateService
     */
    constructor(
        private _fuseConfigService: FuseConfigService,
        private _fuseSidebarService: FuseSidebarService,
        private _translateService: TranslateService,
        private tenantService: TenantService,
        private router: Router,
        private store: Store<coreState.State>,
        public dialog: MatDialog
    )
    {
        // Set the defaults
        this.userStatusOptions = [
            {
                'title': 'Online',
                'icon' : 'icon-checkbox-marked-circle',
                'color': '#4CAF50'
            },
            {
                'title': 'Away',
                'icon' : 'icon-clock',
                'color': '#FFC107'
            },
            {
                'title': 'Do not Disturb',
                'icon' : 'icon-minus-circle',
                'color': '#F44336'
            },
            {
                'title': 'Invisible',
                'icon' : 'icon-checkbox-blank-circle-outline',
                'color': '#BDBDBD'
            },
            {
                'title': 'Offline',
                'icon' : 'icon-checkbox-blank-circle-outline',
                'color': '#616161'
            }
        ];

        this.languages = [
            {
                id   : 'en',
                title: 'English',
                flag: 'gb',
                locale: 'en-GB',
                'direction': 'ltr'
            },
            //{
            //    id   : 'tr',
            //    title: 'Turkish',
            //    flag: 'tr',
            //    'direction': 'ltr'
            //},
            {
                id: 'fa',
                title: 'فارسی',
                flag: 'ir',
                locale: 'fa',
                'direction': 'rtl'
            }
        ];

        this.navigation = navigation;

        // Set the private defaults
        this._unsubscribeAll = new Subject();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void
    {
        // Subscribe to the config changes
        this._fuseConfigService.config
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((settings) => {
                this.fuseSettings = settings;
                this.horizontalNavbar = settings.layout.navbar.position === 'top';
                this.rightNavbar = settings.layout.navbar.position === 'right';
                this.hiddenNavbar = settings.layout.navbar.hidden === true;
            });

        // Set the selected language from default languages
        this.selectedLanguage = _.find(this.languages, {'id': this._translateService.currentLang});

        //this.account$ = this.store.select(coreState.getCurrentAccount);
        //this.account$.pipe(takeUntil(this.destroyed$))
        //  .subscribe((result: User) => {
        //    if (result) {
        //      this.account = result;
        //      this.name = this.account.username;
        //    }
        //  });

        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.isProviderAdmin$ = this.store.select(coreState.getAccountIsProviderAdmin);
        this.isProviderAdmin$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.isProviderAdmin = t;
        });

        this.isProviderUser$ = this.store.select(coreState.getAccountIsProviderUser);
        this.isProviderUser$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.isProviderUser = t;
        });

        this.isTenantAdmin$ = this.store.select(coreState.getAccountIsTenantAdmin);
        this.isTenantAdmin$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.isTenantAdmin = t;
        });

        this.isStandardUser$ = this.store.select(coreState.getAccountIsStandardUser);
        this.isStandardUser$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.isStandardUser = t;
        });

        this.accountGuid$ = this.store.select(coreState.getAccountGuid);
        this.accountGuid$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.accountGuid = t;
        });

        this.username$ = this.store.select(coreState.getUsername);

        this.username$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.username = t;
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            //console.log('start');
            if (t && this.tenants && this.selectedTenant == undefined) {
                var tenant = this.tenants.find(p => p.tenantDomainPlatformMapGuid == t.tenantDomainPlatformMapGuid);
                //console.log('tenant');
                //console.log(t);
                //console.log(tenant);
                //console.log(this.tenants);
                if (tenant != null && tenant != undefined) {
                    this.selectedTenant = tenant;
                } else {
                    this.selectedTenant = t;
                }
            }
            else {
                this.selectedTenant = t;
            }
        });

        this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

        this.tenantGuid$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.tenantGuid = t;
        });

        this.tenants$.pipe(takeUntil(this._unsubscribeAll)).subscribe(items => {
            this.tenants = items;

            if (items != undefined) {
                var tenant = undefined;

                var selectedTenantGuid = this.tenantService.getCurrentTenant();

                if (selectedTenantGuid != undefined && selectedTenantGuid != '' && this.isProviderAdmin) {
                    tenant = items.find(t => t.tenantDomainPlatformMapGuid == selectedTenantGuid);
                }

                if (this.tenantGuid && items != undefined && tenant == undefined) {
                    tenant = items.find(t => t.tenantDomainPlatformMapGuid == this.tenantGuid);
                }

                if (tenant == undefined && items != undefined && items.length > 0 && this.isProviderUser) {
                    tenant = items[0];
                }

                if (tenant != undefined && tenant != null) {
                    this.store.dispatch(new tenantActions.ChangeSelectedTenant(tenant));
                    this.tenantService.setCurrentTenant(tenant.tenantDomainPlatformMapGuid);
                }
            }
        });
    }



    onLogout(): void {
        this.store.dispatch(new merchantActions.Clear());
        this.store.dispatch(new account.Logout());
    }

    onChangePassword(): void {
        //this.router.navigate(['/changepassword']);
        const dialogRef = this.dialog.open(ChangePasswordComponent, {
            width: '300px',
            data: { name: this.username }
        });
    }

    /**
     * On destroy
     */
    ngOnDestroy(): void
    {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next();
        this._unsubscribeAll.complete();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Toggle sidebar open
     *
     * @param key
     */
    toggleSidebarOpen(key): void
    {
        this._fuseSidebarService.getSidebar(key).toggleOpen();
    }

    /**
     * Search
     *
     * @param value
     */
    search(value): void
    {
        // Do your search here...
    }

    /**
     * Set the language
     *
     * @param lang
     */
    setLanguage(lang): void
    {
        // Set the selected language for the toolbar
        //this.selectedLanguage = lang;

        //// Use the selected language for translations
        //this._translateService.use(lang.id);

        this.selectedLanguage = lang;

        // Use the selected language for translations
        //this._translateService.use(lang.id);
        this.fuseSettings.locale.direction = lang.direction;
        this.fuseSettings.locale.languageId = lang.id;
        this.fuseSettings.locale.locale = lang.locale;
        this._fuseConfigService.setConfig(this.fuseSettings);

        window.location.reload();
    }

    setTenant(tenant) {
        this.tenantService.setCurrentTenant(tenant.tenantDomainPlatformMapGuid);
        window.location.reload();
        //this.store.dispatch(new tenantActions.ChangeSelectedTenant(tenant));
    }
}
