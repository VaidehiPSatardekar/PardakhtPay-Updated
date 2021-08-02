import { ChangeDetectorRef, Component, HostBinding, Input, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { merge, Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { FuseNavigationItem } from './../../../../../@fuse/types';
import { fuseAnimations } from './../../../../../@fuse/animations';
import { FuseNavigationService } from './../../../../../@fuse/components/navigation/navigation.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'fuse-nav-vertical-collapsable',
    templateUrl: './collapsable.component.html',
    styleUrls: ['./collapsable.component.scss'],
    animations: fuseAnimations
})
export class FuseNavVerticalCollapsableComponent implements OnInit, OnDestroy {
    @Input()
    item: FuseNavigationItem;

    @HostBinding('class')
    classes = 'nav-collapsable nav-item';

    @HostBinding('class.open')
    public isOpen = false;

    // Private
    private _unsubscribeAll: Subject<any>;

    /**
     * Constructor
     *
     * @param {ChangeDetectorRef} _changeDetectorRef
     * @param {FuseNavigationService} _fuseNavigationService
     * @param {Router} _router
     */
    constructor(
        private _changeDetectorRef: ChangeDetectorRef,
        private _fuseNavigationService: FuseNavigationService,
        private _router: Router,
        private _translateService: TranslateService
    ) {
        // Set the private defaults
        this._unsubscribeAll = new Subject();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    addNavItemCollapseFunction() {
        var collapMenu = this._fuseNavigationService.getNavigationItem('custom-collapse-item');
        if (collapMenu.id != null) {
            return;
        }
        // Prepare the new nav item
        const newNavItem = {
            id: 'custom-collapse-item',
            title: this._translateService.instant('CONFIG.COLLAPSE'),//'Collapse All',
            order: '005',
            type: 'collapsable',
            children: [],
            function: () => {
            }
        };
        // Add the new nav item at the beginning of the navigation
        this._fuseNavigationService.removeNavigationItem('custom-collapse-item');
        this._fuseNavigationService.addNavigationItem(newNavItem, 'start');
    }

    addNavItemExpandFunction() {

        var expandMenu = this._fuseNavigationService.getNavigationItem('custom-expand-item');
        if (expandMenu.id != null) {
            return;
        }
        // Prepare the new nav item
        const newNavItem = {
            id: 'custom-expand-item',
            title: this._translateService.instant('CONFIG.EXPAND'),
            order: '005',
            type: 'collapsable',
            children: [],
            function: () => {
            }
        };
        // Add the new nav item at the beginning of the navigation
        this._fuseNavigationService.removeNavigationItem('custom-expand-item');
        this._fuseNavigationService.addNavigationItem(newNavItem, 'start');
    }
    /**
     * On init
     */
    ngOnInit(): void {
        // Listen for router events
        this._router.events
            .pipe(
                filter(event => event instanceof NavigationEnd),
                takeUntil(this._unsubscribeAll)
            )
            .subscribe((event: NavigationEnd) => {
                // Check if the url can be found in
                // one of the children of this item
                if (this.isUrlInChildren(this.item, event.urlAfterRedirects)) {
                    this.expand();
                }
                else {
                    if (this.item.id !== "ticket-management") {
                        this.collapse();
                    }
                }
            });


        // Listen for collapsing of any navigation item
        this._fuseNavigationService.onItemCollapsed
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe(
                (clickedItem) => {
                    if (clickedItem && clickedItem.children) {
                        // Check if the clicked item is one
                        // of the children of this item
                        if (this.isChildrenOf(this.item, clickedItem)) {
                            return;
                        }


                        // Check if the url can be found in
                        // one of the children of this item
                        if (this.isUrlInChildren(this.item, this._router.url)) {
                            return;
                        }

                        // If the clicked item is not this item, collapse...
                        if (this.item !== clickedItem) {
                            //this.collapse();
                        }

                        if (clickedItem.id == "custom-expand-item") {
                            if (this.item !== clickedItem) {
                                this.expand();
                            }
                        }
                        if (this.item.id === "ticket-management") {
                            if (clickedItem.id != this.item.id) {
                                this.expandItem();
                            }
                        }
                        if (clickedItem.id == "custom-collapse-item") {
                            if (this.item !== clickedItem) {
                                this.collapse();
                            }
                        }
                    }
                }
            );

        // Check if the url can be found in
        // one of the children of this item
        if (this.isUrlInChildren(this.item, this._router.url)) {
            this.expand();
        }
        else {
            var fuseMenuExpand = localStorage.getItem("FuseMenuExpand");
            if (fuseMenuExpand == "true") {
                this.expand();
            } else {
                this.collapse();
            }
        }

        // Subscribe to navigation item
        merge(
            this._fuseNavigationService.onNavigationItemAdded,
            this._fuseNavigationService.onNavigationItemUpdated,
            this._fuseNavigationService.onNavigationItemRemoved
        ).pipe(takeUntil(this._unsubscribeAll))
            .subscribe(() => {

                // Mark for check
                this._changeDetectorRef.markForCheck();
            });
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
     * Toggle collapse
     *
     * @param ev
     */
    toggleOpen(ev): void {
        ev.preventDefault();
        this.isOpen = !this.isOpen;
        this._fuseNavigationService.onItemCollapsed.next(this.item);
        this._fuseNavigationService.onItemCollapseToggled.next();
    }

    /**
     * Expand the collapsable navigation
     */
    expand(): void {

        this._fuseNavigationService.removeNavigationItem('custom-expand-item');
        this.addNavItemCollapseFunction();
        if (this.isOpen) {
            return;
        }

        this.expandItem();
    }

    /* 
    Expand Item 
    */
    expandItem(): void {
        if (this.isOpen) {
            return;
        }

        this.isOpen = true;

        // Mark for check
        this._changeDetectorRef.markForCheck();

        this._fuseNavigationService.onItemCollapseToggled.next();
    }

    /**
     * Collapse the collapsable navigation
     */
    collapse(): void {
        this._fuseNavigationService.removeNavigationItem('custom-collapse-item');
        this.addNavItemExpandFunction();

        if (!this.isOpen) {
            return;
        }

        this.isOpen = false;
        // Mark for check
        this._changeDetectorRef.markForCheck();
        this._fuseNavigationService.onItemCollapseToggled.next();
    }

    /**
     * Check if the given parent has the
     * given item in one of its children
     *
     * @param parent
     * @param item
     * @returns {boolean}
     */
    isChildrenOf(parent, item): boolean {
        if (!parent.children) {
            return false;
        }

        if (parent.children.indexOf(item) !== -1) {
            return true;
        }

        for (const children of parent.children) {
            if (children.children) {
                return this.isChildrenOf(children, item);
            }
        }
    }

    /**
     * Check if the given url can be found
     * in one of the given parent's children
     *
     * @param parent
     * @param url
     * @returns {boolean}
     */
    isUrlInChildren(parent, url): boolean {
        if (!parent.children) {
            return false;
        }

        for (let i = 0; i < parent.children.length; i++) {
            if (parent.children[i].children) {
                if (this.isUrlInChildren(parent.children[i], url)) {
                    return true;
                }
            }

            if (parent.children[i].url === url || url.includes(parent.children[i].url)) {
                return true;
            }
        }

        return false;
    }

}
