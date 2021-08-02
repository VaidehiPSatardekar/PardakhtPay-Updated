import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Platform } from '@angular/cdk/platform';
import { TranslateService } from '@ngx-translate/core';
import { Subject, Observable } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { FuseConfigService } from '../@fuse/services/config.service';
import { FuseNavigationService } from '../@fuse/components/navigation/navigation.service';
import { FuseSidebarService } from '../@fuse/components/sidebar/sidebar.service';
import { FuseSplashScreenService } from '../@fuse/services/splash-screen.service';
import { FuseTranslationLoaderService } from '../@fuse/services/translation-loader.service';

import { navigation } from 'app/navigation/navigation';
import { locale as navigationEnglish } from 'app/navigation/i18n/en';
import { locale as navigationTurkish } from 'app/navigation/i18n/tr';
import { locale as navigationFarsi } from 'app/navigation/i18n/fa';

import { locale as locale_english_admin } from 'app/navigation/i18n/admin-management-en';
import { locale as locale_farsi_admin } from 'app/navigation/i18n/admin-management-fa';



import * as coreState from './core';
import * as account from './core/actions/account';
import { Store } from '@ngrx/store';
import * as tenantActions from './core/actions/tenant';
import { Tenant } from './models/tenant';
import { TenantService } from './core/services/tenant/tenant.service';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material';
import { MAT_MOMENT_DATE_FORMATS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { getIsLoggedIn } from '../app/core/index';
import { AccountService } from './core/services/account.service';
import { Initialize as InitializeAdminManagement } from './core/actions/user';


@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    providers: [
        // The locale would typically be provided on the root module of your application. We do it at
        // the component level here, due to limitations of our example generation script.
        { provide: MAT_DATE_LOCALE, useValue: 'gb' },

        // `MomentDateAdapter` and `MAT_MOMENT_DATE_FORMATS` can be automatically provided by importing
        // `MatMomentDateModule` in your applications root module. We provide it at the component level
        // here, due to limitations of our example generation script.
        { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
        { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    ],
})
export class AppComponent implements OnInit, OnDestroy {
    fuseConfig: any;
    navigation: any;

    // Private
    private _unsubscribeAll: Subject<any>;

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

    tenantGuid$: Observable<string>;
    tenantGuid: string;

    isLoggedIn$: Observable<boolean>;

    tenantSearched: boolean = false;

    /**
     * Constructor
     *
     * @param {DOCUMENT} document
     * @param {FuseConfigService} _fuseConfigService
     * @param {FuseNavigationService} _fuseNavigationService
     * @param {FuseSidebarService} _fuseSidebarService
     * @param {FuseSplashScreenService} _fuseSplashScreenService
     * @param {FuseTranslationLoaderService} _fuseTranslationLoaderService
     * @param {Platform} _platform
     * @param {TranslateService} _translateService
     */
    constructor(
        @Inject(DOCUMENT) private document: any,
        private _fuseConfigService: FuseConfigService,
        private _fuseNavigationService: FuseNavigationService,
        private _fuseSidebarService: FuseSidebarService,
        private _fuseSplashScreenService: FuseSplashScreenService,
        private _fuseTranslationLoaderService: FuseTranslationLoaderService,
        private _translateService: TranslateService,
        private _tenantService: TenantService,
        private store: Store<coreState.State>,
        private _adapter: DateAdapter<any>,
        private _platform: Platform,
        private accountService: AccountService
    ) {
        // Get default navigation
        //this.navigation = navigation;

        console.log('cons');

        // Register the navigation to the service
        this._fuseNavigationService.register('main', navigation);

        // Set the main navigation as our current navigation
        this._fuseNavigationService.setCurrentNavigation('main');

        console.log('cons 1');
        // Add languages
        this._translateService.addLangs(['en', 'tr', 'fa']);

        console.log('cons 2');
        // Set the default language
        this._translateService.setDefaultLang('fa');

        console.log('cons 3');
        // Set the navigation translations
        this._fuseTranslationLoaderService.loadTranslations(navigationEnglish, navigationTurkish, navigationFarsi, locale_english_admin, locale_farsi_admin);

        console.log('cons 4');
        // Use a language
        // this._translateService.use('en');

        /**
         * ----------------------------------------------------------------------------------------------------
         * ngxTranslate Fix Start
         * ----------------------------------------------------------------------------------------------------
         */

        /**
         * If you are using a language other than the default one, i.e. Turkish in this case,
         * you may encounter an issue where some of the components are not actually being
         * translated when your app first initialized.
         *
         * This is related to ngxTranslate module and below there is a temporary fix while we
         * are moving the multi language implementation over to the Angular's core language
         * service.
         **/

        // Set the default language to 'en' and then back to 'tr'.
        // '.use' cannot be used here as ngxTranslate won't switch to a language that's already
        // been selected and there is no way to force it, so we overcome the issue by switching
        // the default language back and forth.
        /**
         setTimeout(() => {
            this._translateService.setDefaultLang('en');
            this._translateService.setDefaultLang('tr');
         });
         */

        /**
         * ----------------------------------------------------------------------------------------------------
         * ngxTranslate Fix End
         * ----------------------------------------------------------------------------------------------------
         */

        // Add is-mobile class to the body if the platform is mobile
        if (this._platform.ANDROID || this._platform.IOS) {
            this.document.body.classList.add('is-mobile');
        }

        // Set the private defaults
        this._unsubscribeAll = new Subject();

        console.log('cons all');
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void {
        console.log('init started');
        // Subscribe to config changes
        this._fuseConfigService.config
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((config) => {
                this.fuseConfig = config;
                console.log(config.layout.style);
                this._translateService.use(this.fuseConfig.locale.languageId);
                document.title = this._translateService.instant('GENERAL.PARDAKHTPAY');
                document.dir = this.fuseConfig.locale.direction;
                document.documentElement.lang = this.fuseConfig.locale.languageId;
                this._adapter.setLocale(this.fuseConfig.locale.locale);
                // Boxed
                if (this.fuseConfig.layout.width === 'boxed') {
                    this.document.body.classList.add('boxed');
                }
                else {
                    this.document.body.classList.remove('boxed');
                }

                // Color theme - Use normal for loop for IE11 compatibility
                for (let i = 0; i < this.document.body.classList.length; i++) {
                    const className = this.document.body.classList[i];

                    if (className.startsWith('theme-')) {
                        this.document.body.classList.remove(className);
                    }
                }

                this.document.body.classList.add(this.fuseConfig.colorTheme);
            });

        this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

        this.tenantGuid$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.tenantGuid = t;
            var ip = this.accountService.isUserProviderAdmin();
            if (ip != undefined && ip == false && this.tenantGuid != undefined) {
                var tenant = new Tenant();
                tenant.tenantDomainPlatformMapGuid = this.tenantGuid;
                this._tenantService.setCurrentTenant(this.tenantGuid);
                this.store.dispatch(new tenantActions.ChangeSelectedTenant(tenant));
            }
        });
        console.log('after tenant');
        this.isProviderAdmin$ = this.store.select(coreState.getAccountIsProviderAdmin);
        this.isProviderAdmin$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.isProviderAdmin = t;
            //if (t && t == true) {
            //    this.store.dispatch(new tenantActions.Search());
            //} else if (this.tenantGuid) {
            //    var tenant = new Tenant();
            //    tenant.tenantDomainPlatformMapGuid = this.tenantGuid;
            //    this._tenantService.setCurrentTenant(this.tenantGuid);
            //    this.store.dispatch(new tenantActions.ChangeSelectedTenant(tenant));
            //}
        });
        console.log('after provider admin');
        this.isProviderUser$ = this.store.select(coreState.getAccountIsProviderUser);
        this.isProviderUser$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.isProviderUser = t;
            if (t && t == true) {
                if (!this.tenantSearched) {
                    this.store.dispatch(new tenantActions.Search());
                    this.tenantSearched = true;
                }
            } else if (this.tenantGuid) {
                var tenant = new Tenant();
                tenant.tenantDomainPlatformMapGuid = this.tenantGuid;
                this._tenantService.setCurrentTenant(this.tenantGuid);
                this.store.dispatch(new tenantActions.ChangeSelectedTenant(tenant));
            }
        });
        console.log('after is provider user');

        this.isTenantAdmin$ = this.store.select(coreState.getAccountIsTenantAdmin);
        this.isTenantAdmin$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.isTenantAdmin = t;
        });

        this.isStandardUser$ = this.store.select(coreState.getAccountIsStandardUser);
        this.isStandardUser$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.isStandardUser = t;
        });
        console.log('after standard user');
        this.accountGuid$ = this.store.select(coreState.getAccountGuid);
        this.accountGuid$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            this.accountGuid = t;
            //if (this.accountGuid) {
            //    this.store.dispatch(new tenantActions.Search());
            //}
        });
        console.log('after account guid');
        this.isLoggedIn$ = this.store.select(getIsLoggedIn);

        console.log('after is logged in 0');
        this.isLoggedIn$.pipe(takeUntil(this._unsubscribeAll)).subscribe(t => {
            if (t) {
                console.log('after is logged in 1');
                var items = [];
                this.filterNavigation(navigation, items);
                this._fuseNavigationService.unregister('main');
                this._fuseNavigationService.register('main', items);
                this._fuseNavigationService.setCurrentNavigation('main');
                this.store.dispatch(new account.Initialize());
            }
            else {
                console.log('after is logged in 2');
                this._fuseNavigationService.unregister('main');
                this._fuseNavigationService.register('main', []);
                this._fuseNavigationService.setCurrentNavigation('main');
                this.store.dispatch(new account.Initialize());
                this.store.dispatch(new tenantActions.Clear());
                this.tenantSearched = false;
            }
         });
        console.log('after is logged in');
        this.store.dispatch(new InitializeAdminManagement());
        console.log('init ended');
    }

    /**
     * On destroy
     */
    ngOnDestroy(): void {
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
    toggleSidebarOpen(key): void {
        this._fuseSidebarService.getSidebar(key).toggleOpen();
    }

    private filterNavigation(items: any[], menus: any[]) {

        for (var i = 0; i < items.length; i++) {
            if (items[i].permission == '' || items[i].permission == undefined || items[i].permission == null || this.accountService.isUserAuthorizedForTask(items[i].permission)) {
                var item = {
                    id: items[i].id,
                    title: items[i].title,
                    translate: items[i].translate,
                    type: items[i].type,
                    permission: items[i].permission,
                    children: [] = [],
                    icon: items[i].icon,
                    url: items[i].url,
                    order: items[i].order
                };

                menus.push(item);

                if (items[i].children != null && items[i].children != undefined && items[i].children.length > 0) {
                    this.filterNavigation(items[i].children, item.children);
                }
            }
        }
    }
}
