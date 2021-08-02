import { Component, OnInit, Input, Output, EventEmitter, ViewChild, OnChanges, OnDestroy, SimpleChanges } from '@angular/core';
import { Role, PermissionGroup, CloneRoleRequest, Permission } from '../../core/models/user-management.model';
import { MatAccordion, EXPANSION_PANEL_ANIMATION_TIMING } from '@angular/material';
import { Subject } from 'rxjs';
import { Store } from '@ngrx/store';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import * as coreState from './../../core/index';
import * as roleActions from './../../core/actions/role';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { FormHelper } from 'app/helpers/forms/form-helper';
import { Tenant } from 'app/core/models/tenant.model';

@Component({
  selector: 'tcam-role-edit',
  templateUrl: './role-edit.component.html',
  styleUrls: ['./role-edit.component.scss'],
  animations: [
    trigger('indicatorRotate', [
      state('collapsed', style({ transform: 'rotate(0deg)' })),
      state('expanded', style({ transform: 'rotate(180deg)' })),
      transition('expanded <=> collapsed', animate(EXPANSION_PANEL_ANIMATION_TIMING))
    ])
  ]
})
export class RoleEditComponent implements OnInit, OnChanges, OnDestroy {
  @Input() permissionGroups: PermissionGroup[];
  @Input() role: Role;
  @Input() loading: boolean;
  @Input() errors: { [key: string]: string };
  @Input() heading: string;
  @Input() readOnly = true;
  @Input() tenants: Tenant[];
  @Input() scopeRestricted = true;
  @Input() cloneEnabled = false;
  @Input() showRestrictedPermissions = false;
  // @Input() tenantId: number;

  @Output() cancel: EventEmitter<void> = new EventEmitter();
  @Output() save: EventEmitter<Role> = new EventEmitter();
  @Output() clone: EventEmitter<CloneRoleRequest> = new EventEmitter();
  @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

  @ViewChild('groupAccordian') groupAccordian: MatAccordion;

  roleForm: FormGroup;
  cloneForm: FormGroup;
  openedGroup: PermissionGroup;
  otherTenants: Tenant[];

  private destroyed$: Subject<boolean> = new Subject();

  constructor(private store: Store<coreState.State>) { }

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    // console.log(changes);
    // this.store.dispatch(new roleActions.ClearErrors());
    if (changes.role) {
      // for a new role (not editing existing one)
      if (changes.role.currentValue === undefined && changes.role.firstChange) {
        this.role = new Role();
        this.role.permissions = [];
      }
      this.createForm();
      this.createCloneForm();
    }
    if (changes.permissions) {
      // this.dataSourcePermissions.data = this.permissions;
      // console.log(this.dataSourcePermissions.data);
    }
    // if (changes.tenants && this.tenants) {
    //   this.otherTenants = this.tenants.filter((tenant: Tenant) => tenant.id !== this.role.tenantId);
    //   // console.log(this.otherTenants);
    // }
    this.updateSelectionList();
  }

  ngOnDestroy(): void {
    // this.formChanges.emit(false);
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  onSubmit(): void {
    if (this.roleForm.valid && !this.readOnly) {
      const editRole: Role = new Role(this.roleForm.value);
      editRole.id = this.role.id;
      // console.log(this.getSelectedListValues());
      editRole.permissions = this.getSelectedListValues();
      editRole.roleHolderTypeId = this.role.roleHolderTypeId;
      this.save.emit(editRole);
    }
    else {
      FormHelper.validateFormGroup(this.roleForm);
    }
  }

  onTenantSelection(args: any): void {
    if (args.value && args.value > 0) {
      // if a tenant is selected then it can't be a fixed role
      this.roleForm.controls['isFixed'].setValue(false);
    }
  }

  onGlobalChanged(args: any): void {
    if (args.checked === true) {
      // if a tenant is selected then it can't be a fixed role
      this.roleForm.controls['tenantGuid'].setValue(undefined);
    }
  }

  canShowPermission(permission: Permission): boolean {
    if (permission.isRestricted) {
      return this.showRestrictedPermissions;
    }

    return true;
  }

  onCloneRole(): void {
    if (this.cloneForm.valid && this.cloneEnabled) {
      const request: CloneRoleRequest = new CloneRoleRequest(this.cloneForm.value);
      request.role = this.role;
      this.clone.emit(request);
    }
    else {
      FormHelper.validateFormGroup(this.cloneForm);
    }
  }

  onCancel(): void {
    this.cancel.emit(undefined);
  }

  onFormChanges(changes: boolean): void {
    this.formChanges.emit(changes);
  }

  getErrorMessage(control: FormControl): string {
    return FormHelper.getErrorMessage(control);
  }

  onToggleGroupDisplay(group: PermissionGroup): void {
    // this.store.dispatch(new roleActions.ClearErrors());
    if (this.openedGroup === undefined) {
      this.toggleExpandedGroup(group);
    }
    // closing current panel - check for changes
    else if (this.openedGroup === group) {
      // if (this.formChanges) {
      //   this.confirmRoleClose(this.toggleExpandedRole);
      // }
      // else {
      this.toggleExpandedGroup(undefined);
      // }
    }
    else {
      // if (this.formChanges) {
      //   this.confirmRoleClose(this.toggleExpandedRole, role);
      // }
      // else {
      this.toggleExpandedGroup(group);
      // }
    }
  }

  toggleExpandedGroup(group: PermissionGroup | undefined): void {
    this.openedGroup = group;
    // this.formChanges = undefined;
  }

  createForm(): void {
    this.roleForm = new FormGroup({
      name: new FormControl({ value: this.role.name, disabled: this.readOnly }, [
        Validators.required
      ]),
      isFixed: new FormControl({ value: this.role.isFixed, disabled: this.readOnly }),
      tenantGuid: new FormControl({ value: this.role.tenantGuid, disabled: this.readOnly })
    });

    // this.roleForm.valueChanges.pipe(
    //   debounceTime(300),
    //   distinctUntilChanged(),
    //   takeUntil(this.ngUnsubscribe)
    // ).subscribe(() => {
    //   const changes: boolean = GenericHelper.detectChanges(this.roleForm.value, this.role);
    //   this.formChanges.emit(changes);
    // });
  }

  createCloneForm(): void {
    this.cloneForm = new FormGroup({
      moveUsersToNewRole: new FormControl({ value: true, disabled: !this.cloneEnabled }),
      tenantGuid: new FormControl({ value: this.role.tenantGuid, disabled: !this.cloneEnabled })
    });
  }

  private updateSelectionList(): void {
    this.permissionGroups.forEach((group: PermissionGroup) => {
      group.permissions.forEach((permission: Permission) => {
        if (this.role.permissions) {
          permission.isSelected = this.role.permissions.findIndex((f: Permission) => f.id === permission.id) > -1;
        }
      });
    });
  }

  private getSelectedListValues(): Permission[] {
    const list: Permission[] = [];
    this.permissionGroups.forEach((group: PermissionGroup) => {
      const selected: Permission[] = group.permissions.filter((value: Permission) => value.isSelected);
      list.push(...selected);
    });

    return list;
  }
}
