import { Action } from '@ngrx/store';
import { MobileTransferDevice } from '../../models/mobile-transfer';

export const GET_ALL = '[MobileTransferDevice] Get All';
export const GET_ALL_COMPLETE = '[MobileTransferDevice] Get All Complete';
export const GET_ALL_ERROR = '[MobileTransferDevice] Get All Error';

export const GET_DETAILS = '[MobileTransferDevice] Get Details';
export const GET_DETAILS_COMPLETE = '[MobileTransferDevice] Get Details Complete';
export const GET_DETAILS_ERROR = '[MobileTransferDevice] Get Details Error';

export const CREATE = '[MobileTransferDevice] Create';
export const CREATE_COMPLETE = '[MobileTransferDevice] Create Complete';
export const CREATE_ERROR = '[MobileTransferDevice] Create Error';

export const EDIT = '[MobileTransferDevice] Edit';
export const EDIT_COMPLETE = '[MobileTransferDevice] Edit Complete';
export const EDIT_ERROR = '[MobileTransferDevice] Edit Error';

export const SEND_SMS = '[MobileTransferDevice] Send Sms';
export const SEND_SMS_COMPLETE = '[MobileTransferDevice] Send Sms Complete';
export const SEND_SMS_ERROR = '[MobileTransferDevice] Send Sms Error';

export const ACTIVATE = '[MobileTransferDevice] Activate';
export const ACTIVATE_COMPLETE = '[MobileTransferDevice] Activate Complete';
export const ACTIVATE_ERROR = '[MobileTransferDevice] Activate Error';

export const CHECK_STATUS = '[MobileTransferDevice] Check Status';
export const CHECK_STATUS_COMPLETE = '[MobileTransferDevice] Check Status Complete';
export const CHECK_STATUS_ERROR = '[MobileTransferDevice] Check Status Error';

export const REMOVE = '[MobileTransferDevice] Remove';
export const REMOVE_COMPLETE = '[MobileTransferDevice] Remove Complete';
export const REMOVE_ERROR = '[MobileTransferDevice] Remove Error';

export const CLEAR = '[MobileTransferDevice] Clear';
export const CLEAR_ERRORS = '[MobileTransferDevice] Clear Errors';

export class GetAll implements Action {
    readonly type = GET_ALL;

    constructor() {

    }
}

export class GetAllComplete implements Action {
    readonly type = GET_ALL_COMPLETE;

    constructor(public payload: MobileTransferDevice[]) {

    }
}

export class GetAllError implements Action {
    readonly type = GET_ALL_ERROR;

    constructor(public payload: string) {

    }
}

export class GetDetails implements Action {
    readonly type = GET_DETAILS;

    constructor(public payload: number) {

    }
}

export class GetDetailsComplete implements Action {
    readonly type = GET_DETAILS_COMPLETE;

    constructor(public payload: MobileTransferDevice) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: MobileTransferDevice) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: MobileTransferDevice) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: MobileTransferDevice) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: MobileTransferDevice) {

    }
}

export class EditError implements Action {
    readonly type = EDIT_ERROR;

    constructor(public payload: string) {

    }
}

export class SendSms implements Action {
    readonly type = SEND_SMS;

    constructor(public id: number) {

    }
}

export class SendSmsComplete implements Action {
    readonly type = SEND_SMS_COMPLETE;

    constructor(public payload: MobileTransferDevice) {

    }
}

export class SendSmsError implements Action {
    readonly type = SEND_SMS_ERROR;

    constructor(public payload: string) {

    }
}

export class Activate implements Action {
    readonly type = ACTIVATE;

    constructor(public id: number, public code: string) {

    }
}

export class ActivateComplete implements Action {
    readonly type = ACTIVATE_COMPLETE;

    constructor(public payload: MobileTransferDevice) {

    }
}

export class ActivateError implements Action {
    readonly type = ACTIVATE_ERROR;

    constructor(public payload: string) {

    }
}

export class CheckStatus implements Action {
    readonly type = CHECK_STATUS;

    constructor(public id: number) {

    }
}

export class CheckStatusComplete implements Action {
    readonly type = CHECK_STATUS_COMPLETE;

    constructor(public payload: MobileTransferDevice) {

    }
}

export class CheckStatusError implements Action {
    readonly type = CHECK_STATUS_ERROR;

    constructor(public payload: string) {

    }
}

export class Remove implements Action {
    readonly type = REMOVE;

    constructor(public id: number) {

    }
}

export class RemoveComplete implements Action {
    readonly type = REMOVE_COMPLETE;

    constructor(public payload: MobileTransferDevice) {

    }
}

export class RemoveError implements Action {
    readonly type = REMOVE_ERROR;

    constructor(public payload: string) {

    }
}

export class Clear implements Action {
    readonly type = CLEAR;

    constructor() {

    }
}

export class ClearErrors implements Action {
    readonly type = CLEAR_ERRORS;

    constructor() {

    }
}

export type Actions =
    GetAll | GetAllComplete | GetAllError
    | GetDetails | GetDetailsComplete | GetDetailsError
    | Create | CreateComplete | CreateError
    | Edit | EditComplete | EditError
    | SendSms | SendSmsComplete | SendSmsError
    | Activate | ActivateComplete | ActivateError
    | CheckStatus | CheckStatusComplete | CheckStatusError
    | Remove | RemoveComplete | RemoveError
    | Clear | ClearErrors;
