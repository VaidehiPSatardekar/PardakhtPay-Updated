import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Role, StaffUser } from '../../core/models/user-management.model';
import { Subject, Observable } from 'rxjs';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import * as coreState from './../../core/index';
import * as roleActions from './../../core/actions/role';
import { Permissions } from './../../models/index';
import { PermissionGuard } from '../../guards/permission.guard';
import { takeUntil } from 'rxjs/operators';
import { FormHelper } from 'app/helpers/forms/form-helper';

@Component({
  selector: 'tcam-role-create',
  templateUrl: './role-create.component.html',
  styleUrls: ['./role-create.component.scss']
})
export class RoleCreateComponent implements OnInit, OnDestroy {

  @Output() cancel: EventEmitter<void> = new EventEmitter();

  errors$: Observable<{ [key: string]: string }>;
  errors: { [key: string]: string };

  roleForm: FormGroup;
  role: Role;
  roleHolderTypeId: string = "P";
  permissions = Permissions;

  currentUser: StaffUser;

  private destroyed$: Subject<boolean> = new Subject();

  constructor(private permissionGuard: PermissionGuard,
    private store: Store<coreState.State>) {
    this.createForm();
  }

  ngOnInit(): void {
    this.errors$ = this.store.select(coreState.getRolesError);

    this.errors$.pipe(takeUntil(this.destroyed$))
      .subscribe((errors: any) => {
        if (errors !== undefined) {
          this.errors = errors;
        }
      });

    this.permissionGuard.getCurrentUser().subscribe(user => {
      this.currentUser = user;
    });
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
    this.store.dispatch(new roleActions.ClearErrors());
  }

  onSubmit(): void {

    if (this.roleForm.valid) {
      const role: Role = new Role(this.roleForm.value);
      role.roleHolderTypeId = this.canDisplay(this.permissions.RoleEditProvider) ? this.roleHolderTypeId : "T";
      role.isFixed = this.canDisplay(this.permissions.RoleEditProvider);
      role.tenantGuid = role.isFixed ? null : this.currentUser.tenantGuid;
      this.store.dispatch(new roleActions.CreateRole(role));
    }
    else {
      FormHelper.validateFormGroup(this.roleForm);
    }
  }

  canDisplay(item: string): boolean {
    return this.permissionGuard.isUserAuthorized(item);
  }

  onCancel(): void {
    this.cancel.emit(undefined);
  }

  getErrorMessage(control: FormControl): string {
    return FormHelper.getErrorMessage(control);
  }

  createForm(): void {
    this.roleForm = new FormGroup({
      name: new FormControl(undefined, [
        Validators.required, Validators.minLength(5)
      ]),
      isFixed: new FormControl(true),
      roleHolderTypeId: new FormControl(undefined),
    });
  }

  changeRoleHolderTypeId(event: any) {
    this.roleHolderTypeId = event.value === "false" ? "T" : "P";
  }
}
