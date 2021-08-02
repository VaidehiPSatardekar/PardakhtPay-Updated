import { Action } from '@ngrx/store';
import { ListSearchResponse } from '../../models/types';
import { BankLogin, BankAccount, CreateLoginFromLoginRequestDTO, BankLoginUpdateDTO, QrCodeRegistrationRequest } from '../../models/bank-login';
import { Bank } from '../../models/bank';


export const SHOW_LOGINDEVICESTATUS = '[LoginDeviceStatus] Show Login Device Status';
export const SHOW_LOGINDEVICESTATUS_COMPLETED = '[LoginDeviceStatus] Show Login Device Status Complete';
export const SHOW_LOGINDEVICESTATUS_ERROR = '[LoginDeviceStatus] Show Login Device Status Error';

export const SHOW_LOGINLIST_DEVICESTATUS = '[LoginDeviceStatus] Show Login List Device Status';
export const SHOW_LOGINLIST_DEVICESTATUS_COMPLETED = '[LoginDeviceStatus] Show Login List Device Status Complete';
export const SHOW_LOGINLIST_DEVICESTATUS_ERROR = '[LoginDeviceStatus] Show Login List Device Status Error';

export const GET_OWNER_LOGIN_LIST = '[LoginDeviceStatus] Get Owner Login List';
export const GET_OWNER_LOGIN_LIST_COMPLETE = '[LoginDeviceStatus] Get Owner Login List Complete';
export const GET_OWNER_LOGIN_LIST_ERROR = '[LoginDeviceStatus] Get Owner Login List Error';

export const GET_DETAILS = '[LoginDeviceStatus] Get Details';
export const GET_DETAILS_COMPLETE = '[LoginDeviceStatus] Get Details Complete';
export const GET_DETAILS_ERROR = '[LoginDeviceStatus] Get Details Error';


export const CLEAR_ALL = '[LoginDeviceStatus] Clear All';

export class GetOwnerLoginList implements Action {
    readonly type = GET_OWNER_LOGIN_LIST;

    constructor() {

    }
}

export class GetOwnerLoginListComplete implements Action {
    readonly type = GET_OWNER_LOGIN_LIST_COMPLETE;

    constructor(public payload: BankLogin[]) {

    }
}

export class GetOwnerLoginListError implements Action {
    readonly type = GET_OWNER_LOGIN_LIST_ERROR;

    constructor(public payload: string) {

    }
}

export class ShowLoginDeviceStatus implements Action {
    readonly type = SHOW_LOGINDEVICESTATUS;

    constructor(public payload: string) {

    }
}

export class ShowLoginDeviceStatusCompleted implements Action {
    readonly type = SHOW_LOGINDEVICESTATUS_COMPLETED;


    constructor(public payload: string) {
        console.log(payload);
    }
}

export class ShowLoginDeviceStatusError implements Action {
    readonly type = SHOW_LOGINDEVICESTATUS_ERROR;

    constructor(public payload: string) {

    }
}

export class ShowLoginListDeviceStatus implements Action {
    readonly type = SHOW_LOGINLIST_DEVICESTATUS;

    constructor() {

    }
}

export class ShowLoginListDeviceStatusCompleted implements Action {
    readonly type = SHOW_LOGINLIST_DEVICESTATUS_COMPLETED;


    constructor(public payload: BankLogin[]) {
        console.log(payload);
    }
}

export class ShowLoginListDeviceStatusError implements Action {
    readonly type = SHOW_LOGINLIST_DEVICESTATUS_ERROR;

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

    constructor(public payload: BankLogin) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}


export class ClearAll implements Action {
    readonly type = CLEAR_ALL;

    constructor() {

    }
}



export type Actions =
    GetDetails | GetDetailsComplete | GetDetailsError
    | ShowLoginDeviceStatus | ShowLoginDeviceStatusCompleted | ShowLoginDeviceStatusError
    | ShowLoginListDeviceStatus | ShowLoginListDeviceStatusCompleted | ShowLoginListDeviceStatusError
    | ClearAll;