import { GenericHelper } from '../helpers/generic';
import { ISuspension } from './suspension.model';
import { AuditLog } from './types';
import { Role } from './role-management.model';



export class User {
    id: number;
    username: string;
    accountId: string;
    firstName: string;
    lastName: string;
    email: string;
    tenantId: number;
    userType: number;
    tenantBrandName;
    tenantGuid: string;
    roles: Role[];
    permissions: string[];
    isBlocked: boolean;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}
