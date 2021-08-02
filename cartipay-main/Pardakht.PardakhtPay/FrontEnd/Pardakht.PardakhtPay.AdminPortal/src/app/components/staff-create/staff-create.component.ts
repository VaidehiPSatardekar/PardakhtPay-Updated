
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';

import { MatTableDataSource } from '@angular/material';
import * as coreState from './../../core/index';
import * as userActions from './../../core/actions/user';
import { GenericHelper } from './../../core/helpers/generic';
import { Role, StaffUser, StaffUserPlatformRoleContainer, CreateStaffUserResponse } from './../../core/models/user-management.model';
import { UserService } from './../../core/services/user.service';
import { Brand } from '../../core/models/tenant.model';
import { FormHelper } from 'app/helpers/forms/form-helper';

@Component({
    selector: 'tcam-staff-create',
    templateUrl: './staff-create.component.html',
    styleUrls: ['./staff-create.component.scss']
})

export class StaffCreateComponent implements OnChanges, OnInit {

    @Input() tenantPlatformMapBrands: Brand[];
    @Input() roles: Role[];
    @Input() editRolesEnabled = false;
    @Input() tenantGuid: string;
    @Input() users: StaffUser[];
    @Input() parentAccountEnabled = false;
    @Output() cancel: EventEmitter<void> = new EventEmitter();

    loading$: Observable<boolean>;
    errors$: Observable<string>;
    createUserResponse$: Observable<CreateStaffUserResponse>;
    userForm: FormGroup;
    dataSourceRoles: MatTableDataSource<Role> = new MatTableDataSource<Role>([]);
    loginGuid: string = undefined;
    staffUser = 'staff';

    constructor(private store: Store<coreState.State>,
        private userService: UserService) {
        this.createForm();
        this.loginGuid = this.userService.getTenantGuid();
    }

    brandSelectionChanged(event: any): void {
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.roles) {
            this.dataSourceRoles.data = this.roles;
            // reset
            this.dataSourceRoles.data.forEach((value: Role) => value.isSelected = false);
        }
    }

    ngOnInit(): void {
        this.loading$ = this.store.select(coreState.getUserLoading);
        this.errors$ = this.store.select(coreState.getUserCreateError);
        this.store.dispatch(new userActions.ClearErrors());

        this.createUserResponse$ = this.store.select(coreState.getUserCreated);
    }

    onSubmit(): void {
        if (this.userForm.valid) {
            const user: StaffUser = new StaffUser(this.userForm.value);
            user.platformRoleMappings = [];
            const platformContainer = new StaffUserPlatformRoleContainer();
            platformContainer.roles = this.getSelectedListValues();
            user.platformRoleMappings.push(platformContainer);
            if (this.tenantGuid !== undefined) {
                user.tenantGuid = this.tenantGuid;
            }
            if (this.userForm.value.brandId === this.staffUser) {
                user.brandId = null;
            }
            this.store.dispatch(new userActions.CreateStaffUser(user));
        }
        else {
            FormHelper.validateFormGroup(this.userForm);
        }
    }

    onCancel(): void {
        this.cancel.emit(undefined);
    }

    getErrorMessage(control: FormControl): string {
        return FormHelper.getErrorMessage(control);
    }

    createForm(): void {
        debugger;
        const formGroup = new FormGroup({
            brandId: new FormControl('staff'),
            username: new FormControl(undefined, [
                Validators.required, Validators.minLength(5)
            ]),
            firstName: new FormControl(undefined, [
                Validators.required
            ]),
            lastName: new FormControl(undefined, [
                Validators.required
            ]),
            email: new FormControl(undefined, [
                Validators.required, Validators.email
            ]),
            parentAccountId: new FormControl(undefined)
        });

        //if (this.parentAccountEnabled) {
        //    formGroup.controls['parentAccountId'] = new FormControl(undefined);
        //}

        this.userForm = formGroup;
    }

    private getSelectedListValues(): number[] {
        return this.roles.filter((value: Role) => value.isSelected).map((role: Role) => role.id);
    }
}
