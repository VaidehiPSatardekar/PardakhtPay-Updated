import { Action } from '@ngrx/store';
import { ApplicationSettings, TransferStatusDescription } from '../../models/application-settings';
import { OwnerSetting } from 'app/models/owner-setting';

export const GET_DETAILS = '[ApplicationSettings] Get Details';
export const GET_DETAILS_COMPLETE = '[ApplicationSettings] Get Details Complete';
export const GET_DETAILS_ERROR = '[ApplicationSettings] Get Details Error';

export const GET_TRANSFER_STATUS = '[ApplicationSettings] Get Transfer Status';
export const GET_TRANSFER_STATUS_COMPLETE = '[ApplicationSettings] Get Transfer Status Complete';
export const GET_TRANSFER_STATUS_ERROR = '[ApplicationSettings] Get Transfer Status Error';

export const EDIT = '[ApplicationSettings] Edit';
export const EDIT_COMPLETE = '[ApplicationSettings] Edit Complete';
export const EDIT_ERROR = '[ApplicationSettings] Edit Error';

export const GET_OWNER_SETTING = '[OwnerSetting] Get Owner Setting';
export const GET_OWNER_SETTING_COMPLETED = '[OwnerSetting] Get Owner Setting Completed';
export const GET_OWNER_SETTING_ERROR = '[OwnerSetting] Get Owner Setting Error';

export const SAVE_OWNER_SETTING = '[OwnerSetting] Save Owner Setting';
export const SAVE_OWNER_SETTING_COMPLETED = '[OwnerSetting] Save Owner Setting Completed';
export const SAVE_OWNER_SETTING_ERROR = '[OwnerSetting] Save Owner Setting Error';

export const CLEAR = '[ApplicationSettings] Clear';
export const CLEAR_ERRORS = '[ApplicationSettings] Clear Errors';

export class GetDetails implements Action {
    readonly type = GET_DETAILS;

    constructor() {

    }
}

export class GetDetailsComplete implements Action {
    readonly type = GET_DETAILS_COMPLETE;

    constructor(public payload: ApplicationSettings) {

    }
}

export class GetDetailsError implements Action {
    readonly type = GET_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public payload: ApplicationSettings) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: ApplicationSettings) {

    }
}

export class EditError implements Action {
    readonly type = EDIT_ERROR;

    constructor(public payload: string) {

    }
}

export class GetTransferStatus implements Action {
    readonly type = GET_TRANSFER_STATUS;

    constructor() {

    }
}

export class GetTransferStatusComplete {
    readonly type = GET_TRANSFER_STATUS_COMPLETE;

    constructor(public payload: TransferStatusDescription[]) {

    }
}

export class GetTransferStatusError implements Action {
    readonly type = GET_TRANSFER_STATUS_ERROR;

    constructor(public payload: string) {

    }
}

export class GetOwnerSetting implements Action{
    readonly type = GET_OWNER_SETTING;

    constructor(){

    }
}

export class GetOwnerSettingCompleted implements Action{
    readonly type = GET_OWNER_SETTING_COMPLETED;

    constructor(public payload: OwnerSetting){

    }
}

export class GetOwnerSettingError implements Action{
    readonly type = GET_OWNER_SETTING_ERROR;

    constructor(public payload: string){

    }
}

export class SaveOwnerSetting implements Action{
    readonly type = SAVE_OWNER_SETTING;

    constructor(public payload: OwnerSetting){

    }
}

export class SaveOwnerSettingCompleted implements Action{
    readonly type = SAVE_OWNER_SETTING_COMPLETED;

    constructor(public payload: OwnerSetting){

    }
}

export class SaveOwnerSettingError implements Action{
    readonly type = SAVE_OWNER_SETTING_ERROR;

    constructor(public payload: string){

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
    GetDetails | GetDetailsComplete | GetDetailsError
    | Edit | EditComplete | EditError
    | GetTransferStatus | GetTransferStatusComplete | GetTransferStatusError
    | GetOwnerSetting | GetOwnerSettingCompleted | GetOwnerSettingError
    | SaveOwnerSetting | SaveOwnerSettingCompleted | SaveOwnerSettingError
    | Clear | ClearErrors;
