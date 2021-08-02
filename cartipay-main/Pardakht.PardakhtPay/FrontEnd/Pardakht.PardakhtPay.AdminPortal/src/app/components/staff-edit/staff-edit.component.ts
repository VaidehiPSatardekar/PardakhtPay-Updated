import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';

import { MatTableDataSource, MatSlideToggleChange, MatDialog, MatDialogRef } from '@angular/material';
import * as coreState from './../../core/index';
import * as userActions from './../../core/actions/user';
import { GenericHelper } from './../../core/helpers/generic';
import { Role, StaffUser, StaffUserPlatformRoleContainer, BlockStaffUserRequest, DeleteStaffUserRequest, UserType, PasswordResetResponse } from './../../core/models/user-management.model';
import { UserService } from './../../core/services/user.service';
import { TranslateService } from '@ngx-translate/core';
import { FormHelper } from 'app/helpers/forms/form-helper';
import { ConfirmationDialogComponent } from '../confirm-dialog/confirm-dialog.component';

@Component({
    selector: 'tcam-staff-edit',
    templateUrl: './staff-edit.component.html',
    styleUrls: ['./staff-edit.component.scss']
})

export class StaffEditComponent implements OnChanges, OnInit {

    @Input() user: StaffUser;
    @Input() roles: Role[];
    @Input() editRolesEnabled = false;
    @Input() resetPasswordEnabled = false;
    @Input() editDetailsEnabled = false;
    @Input() blockUserEnabled = false;
    @Input() platformGuid: string;
    @Input() tenantGuid: string;
    @Input() users: StaffUser[];
    @Input() parentAccountEnabled = false;

    @Output() cancel: EventEmitter<void> = new EventEmitter();
    // @Output() resetPassword: EventEmitter<string> = new EventEmitter();

    loading$: Observable<boolean>;
    errors$: Observable<string>;
    accountErrors$: Observable<{ [key: string]: string }>;
    resetPasswordResult$: Observable<PasswordResetResponse>;
    userForm: FormGroup;
    dataSourceRoles: MatTableDataSource<Role> = new MatTableDataSource<Role>([]);
    loginGuid: string = undefined;

    dialogRef: MatDialogRef<ConfirmationDialogComponent>;

    userTypeArray: any[] = [
        { value: UserType.standard, text: this.translateService.instant('STAFF-USER.STAFFUSER') },
    ];

    constructor(private store: Store<coreState.State>, public dialog: MatDialog, private translateService: TranslateService,
        private userService: UserService) {
        this.loginGuid = this.userService.getTenantGuid();
    }

    ngOnChanges(changes: SimpleChanges): void {
        this.store.dispatch(new userActions.ClearErrors());
        if (changes.user) {
            this.createForm();
        }
        if (changes.roles) {
            this.dataSourceRoles.data = this.roles;
            // console.log(this.dataSourceRoles.data);
        }
        this.updateSelectionList();
    }

    ngOnInit(): void {
        this.loading$ = this.store.select(coreState.getUserLoading);
        this.errors$ = this.store.select(coreState.getUserEditError);
        this.resetPasswordResult$ = this.store.select(coreState.getPasswordResetResult);
        // look into merging the error observables into one
        // this.accountErrors$ = this.store.select(coreState.getAccountErrors);
    }

    onSubmit(): void {
        this.userForm.updateValueAndValidity();
        if (this.userForm.valid) {
            const user: StaffUser = new StaffUser(this.userForm.value);
            user.id = this.user.id;
            user.username = this.user.username;
            user.platformRoleMappings = this.getSelectedListValues();
            if (this.tenantGuid !== undefined) {
                user.tenantGuid = this.tenantGuid;
            }
            this.store.dispatch(new userActions.EditStaffUser(user));
        }
        else {
            FormHelper.validateFormGroup(this.userForm);
        }
    }

    onCancel(): void {
        this.cancel.emit(undefined);
    }

    onResetPassword(): void {
        this.store.dispatch(new userActions.ResetPassword(this.user.accountId));
    }

    onBlockUser(args: MatSlideToggleChange): void {
        this.store.dispatch(new userActions.BlockStaffUser(new BlockStaffUserRequest(this.user.id, args.checked)));
    }

    onDeleteUser(): void {

        this.dialogRef = this.dialog.open(ConfirmationDialogComponent, {
            disableClose: false,
            width: '400px',
            data: { confirmMessage: 'Are you sure you want to delete this user? ' }
        });

        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.store.dispatch(new userActions.DeleteStaffUser(new DeleteStaffUserRequest(this.user.id)));
            }
        });

    }

    getErrorMessage(control: FormControl): string {
        return FormHelper.getErrorMessage(control);
    }

    createForm(): void {
        const formGroup = new FormGroup({
            username: new FormControl({ value: this.user.username, disabled: true }, [
                Validators.required, Validators.minLength(5)
            ]),
            firstName: new FormControl({ value: this.user.firstName, disabled: !this.editDetailsEnabled || this.user.userType === UserType.api },
                this.user.userType === UserType.standard ? [Validators.required] : []),
            lastName: new FormControl({ value: this.user.lastName, disabled: !this.editDetailsEnabled || this.user.userType === UserType.api },
                this.user.userType === UserType.standard ? [Validators.required] : []),
            email: new FormControl({ value: this.user.email, disabled: !this.editDetailsEnabled || this.user.userType === UserType.api },
                this.user.userType === UserType.standard ? [Validators.required, Validators.email] : []),
            isUserBlocked: new FormControl({ value: this.user.isBlocked, disabled: !this.editDetailsEnabled }, [
                Validators.required
            ]),
            userType: new FormControl({ value: this.user.userType, disabled: !this.editRolesEnabled }),
            parentAccountId: new FormControl({ value: this.user.parentAccountId, disabled: !this.editDetailsEnabled }, [
            ])
        });
        //if (this.parentAccountEnabled && !this.loginGuid) {
        //    formGroup.controls['parentAccountId'] = new FormControl({ value: this.user.parentAccountId, disabled: !this.editDetailsEnabled }, []);
        //}

        this.userForm = formGroup;
    }

    private updateSelectionList(): void {
        // flatten out user's current roles into one list
        let allRoles: number[] = [];
        if (this.user.platformRoleMappings) {
            this.user.platformRoleMappings.forEach((mapping: StaffUserPlatformRoleContainer) => {
                allRoles = allRoles.concat(mapping.roles);
            });
        }
        // flag the item as selected if the user already has the role
        this.roles.forEach((value: Role) => {
            value.isSelected = allRoles.findIndex((r: number) => r === value.id) > -1;
        });
    }

    private getSelectedListValues(): StaffUserPlatformRoleContainer[] {
        // we are only editing roles for this platform, so clone user's platform records and update
        const result = this.user.platformRoleMappings;
        const thisPlatform = result.find((mapping: StaffUserPlatformRoleContainer) => mapping.platformGuid === this.platformGuid);
        // return this.roles.filter((value: Role) => value.isSelected);
        thisPlatform.roles = this.roles.filter((value: Role) => value.isSelected).map((role: Role) => role.id);

        return result;
    }
}
