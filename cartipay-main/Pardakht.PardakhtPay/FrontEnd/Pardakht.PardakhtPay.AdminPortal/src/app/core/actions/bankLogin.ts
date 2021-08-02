import { Action } from '@ngrx/store';
import { ListSearchResponse } from '../../models/types';
import { BankLogin, BankAccount, CreateLoginFromLoginRequestDTO, BankLoginUpdateDTO, QrCodeRegistrationRequest, RegisterLogin } from '../../models/bank-login';
import { Bank } from '../../models/bank';
import { BlockedCardDetail } from '../../models/blocked-card-detail';

export const SEARCH = '[BankLogin] Search';
export const SEARCH_COMPLETE = '[BankLogin] Search Complete';
export const SEARCH_ERROR = '[BankLogin] Search Error';

export const SEARCH_ACCOUNTS = '[BankLogin] Search Accounts';
export const SEARCH_ACCOUNTS_COMPLETE = '[BankLogin] Search Accounts Complete';
export const SEARCH_ACCOUNTS_ERROR = '[BankLogin] Search Accounts Error';

export const SEARCH_BANKS = '[BankLogin] Search Banks';
export const SEARCH_BANKS_COMPLETE = '[BankLogin] Search Banks Complete';
export const SEARCH_BANKS_ERROR = '[BankLogin] Search Banks Error';

export const CREATE = '[BankLogin] Create';
export const CREATE_COMPLETE = '[BankLogin] Create Complete';
export const CREATE_ERROR = '[BankLogin] Craete Error';

export const GET_DETAILS = '[BankLogin] Get Details';
export const GET_DETAILS_COMPLETE = '[BankLogin] Get Details Complete';
export const GET_DETAILS_ERROR = '[BankLogin] Get Details Error';

export const EDIT = '[BankLogin] Edit';
export const EDIT_COMPLETE = '[BankLogin] Edit Complete';
export const EDIT_ERROR = '[BankLogin] Edit Error';

export const SEARCH_ACCOUNTS_BY_LOGIN_GUID = '[BankLogin] Search Accounts By Login Guid';
export const SEARCH_ACCOUNTS_BY_LOGIN_GUID_COMPLETED = '[BankLogin] Search Accounts By Login Guid Completed';
export const SEARCH_ACCOUNTS_BY_LOGIN_GUID_ERROR = '[BankLogin] Search Accounts By Login Guid Error';

export const UPDATE_LOGIN_INFORMATION = '[BankLogin] Update Login Information';
export const UPDATE_LOGIN_INFORMATION_COMPLETE = '[BankLogin] Update Login Information Success';
export const UPDATE_LOGIN_INFORMATION_ERROR = '[BankLogin] Update Login Information Error';

export const CREATE_LOGIN_FROM_LOGIN_REQUEST = '[BankLogin] Create Login From Login Request';
export const CREATE_LOGIN_FROM_LOGIN_REQUEST_COMPLETE = '[BankLogin] Create Login From Login Request Complete';
export const CREATE_LOGIN_FROM_LOGIN_REQUEST_ERROR = '[BankLogin] Create Login From Login Request Error';

export const GET_OWNER_LOGIN_LIST = '[BankLogin] Get Owner Login List';
export const GET_OWNER_LOGIN_LIST_COMPLETE = '[BankLogin] Get Owner Login List Complete';
export const GET_OWNER_LOGIN_LIST_ERROR = '[BankLogin] Get Owner Login List Error';

export const DEACTIVATE_OWNER_LOGIN = '[BankLogin] Deactivate Owner Login';
export const DEACTIVATE_OWNER_LOGIN_COMPLETE = '[BankLogin]  Deactivate Owner Login Complete';
export const DEACTIVATE_OWNER_LOGIN_ERROR = '[BankLogin]  Deactivate Owner Login Error';

export const DELETE_OWNER_LOGIN = '[BankLogin] Delete Owner Login';
export const DELETE_OWNER_LOGIN_COMPLETE = '[BankLogin]  Delete Owner Login Complete';
export const DELETE_OWNER_LOGIN_ERROR = '[BankLogin]  Delete Owner Login Error';

export const ACTIVATE_OWNER_LOGIN = '[BankLogin] Activate Owner Login';
export const ACTIVATE_OWNER_LOGIN_COMPLETE = '[BankLogin]  Activate Owner Login Complete';
export const ACTIVATE_OWNER_LOGIN_ERROR = '[BankLogin]  Activate Owner Login Error';

export const GET_BLOCKED_CARD_DETAILS = '[BankLogin] Get Blocked Card Details';
export const GET_BLOCKED_CARD_DETAILS_COMPLETE = '[BankLogin] Get Blocked Card Details Complete';
export const GET_BLOCKED_CARD_DETAILS_ERROR = '[BankLogin] Get Blocked Card Details Error';

export const SHOW_PASSWORD = '[BankLogin] Show Password';
export const SHOW_PASSWORD_COMPLETED = '[BankLogin] Show Password Completed';
export const SHOW_PASSWORD_ERROR = '[BankLogin] Show Password Error';

export const QR_REGISTER_LOGIN = '[BankLogin] QR Register Login';
export const QR_REGISTER_LOGIN_COMPLETE = '[BankLogin]  QR Register Login Complete';
export const QR_REGISTER_LOGIN_ERROR = '[BankLogin]  QR Register Login Error';

export const GET_QR_REGISTRATION_DETAILS = '[BankLogin] Get QR Registration details';
export const GET_QR_REGISTRATION_DETAILS_COMPLETE = '[BankLogin]  Get QR Registration details Complete';
export const GET_QR_REGISTRATION_DETAILS_ERROR = '[BankLogin]  Get QR Registration details Error';

export const REGISTER_QR_CODE = '[BankLogin] Complete Qr Code Registration';
export const REGISTER_QR_CODE_COMPLETE = '[BankLogin] Complete Qr Code Registration Complete';
export const REGISTER_QR_CODE_ERROR = '[BankLogin] Complete Qr Code Registration Error';

export const GET_OTP = '[BankLogin] Get OTP';
export const GET_OTP_COMPLETED = '[BankLogin] Get OTP Completed';
export const GET_OTP_ERROR = '[BankLogin] Get OTP Error';

export const REGISTER_LOGIN_REQUEST = '[BankLogin] Register Login Request';
export const REGISTER_LOGIN_REQUEST_COMPLETE = '[BankLogin] Register Login Request Complete';
export const REGISTER_LOGIN_REQUEST_ERROR = '[BankLogin] Register Login Request Error';

export const SWITCH_BANK_CONNECTION_PROGRAM = '[BankLogin] Switch Bank Connection Program';
export const SWITCH_BANK_CONNECTION_PROGRAM_COMPLETE = '[BankLogin] Switch Bank Connection Program Complete';
export const SWITCH_BANK_CONNECTION_PROGRAM_ERROR = '[BankLogin] Switch Bank Connection Program Error';

export const CLEAR_ALL = '[BankLogin] Clear All';

export class Search implements Action {
    readonly type = SEARCH;

    constructor(public payload: boolean) {

    }
}

export class SearchComplete implements Action {
    readonly type = SEARCH_COMPLETE;

    constructor(public payload: BankLogin[]) {
    }
}

export class SearchError implements Action {
    readonly type = SEARCH_ERROR;

    constructor(public payload: string) {

    }
}

export class SearchAccounts implements Action {
    readonly type = SEARCH_ACCOUNTS;

    constructor(public payload: boolean) {

    }
}

export class SearchAccountsComplete implements Action {
    readonly type = SEARCH_ACCOUNTS_COMPLETE;

    constructor(public payload: BankAccount[]) {
        //console.log(payload);
    }
}

export class SearchAccountsError implements Action {
    readonly type = SEARCH_ACCOUNTS_ERROR;

    constructor(public payload: string) {

    }
}

export class SearchBanks implements Action {
    readonly type = SEARCH_BANKS;

    constructor() {

    }
}

export class SearchBanksComplete implements Action {
    readonly type = SEARCH_BANKS_COMPLETE;

    constructor(public payload: Bank[]) {

    }
}

export class SearchBanksError implements Action {
    readonly type = SEARCH_BANKS_ERROR;

    constructor(public payload: string) {

    }
}

export class Create implements Action {
    readonly type = CREATE;

    constructor(public payload: BankLogin) {

    }
}

export class CreateComplete implements Action {
    readonly type = CREATE_COMPLETE;

    constructor(public payload: BankLogin) {

    }
}

export class CreateError implements Action {
    readonly type = CREATE_ERROR;

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

export class Edit implements Action {
    readonly type = EDIT;

    constructor(public id: number, public payload: BankLogin) {

    }
}

export class EditComplete implements Action {
    readonly type = EDIT_COMPLETE;

    constructor(public payload: BankLogin) {

    }
}

export class EditError implements Action {
    readonly type = EDIT_ERROR;

    constructor(public payload: string) {

    }
}

export class ClearAll implements Action {
    readonly type = CLEAR_ALL;

    constructor() {

    }
}

export class SearchAccountsByLoginGuid implements Action {
    readonly type = SEARCH_ACCOUNTS_BY_LOGIN_GUID;

    constructor(public payload: string) {

    }
}

export class SearchAccountsBYLoginGuidCompleted implements Action {
    readonly type = SEARCH_ACCOUNTS_BY_LOGIN_GUID_COMPLETED;

    constructor(public payload: BankAccount[]) {
    }
}

export class SearchAccountsByLoginGuidError implements Action {
    readonly type = SEARCH_ACCOUNTS_BY_LOGIN_GUID_ERROR;

    constructor(public payload: string) {

    }
}

export class UpdateLoginInformation implements Action {
    readonly type = UPDATE_LOGIN_INFORMATION;

    constructor(public id: number, public payload: BankLoginUpdateDTO) {

    }
}

export class UpdateLoginInformationComplete implements Action {
    readonly type = UPDATE_LOGIN_INFORMATION_COMPLETE;

    constructor() {

    }
}

export class UpdateLoginInformationError implements Action {
    readonly type = UPDATE_LOGIN_INFORMATION_ERROR;

    constructor(public payload: string) {

    }
}

export class CreateLoginFromLoginRequest implements Action {
    readonly type = CREATE_LOGIN_FROM_LOGIN_REQUEST;

    constructor(public payload: CreateLoginFromLoginRequestDTO) {

    }
}

export class CreateLoginFromLoginRequestComplete implements Action {
    readonly type = CREATE_LOGIN_FROM_LOGIN_REQUEST_COMPLETE;

    constructor() {

    }
}

export class CreateLoginFromLoginRequestError implements Action {
    readonly type = CREATE_LOGIN_FROM_LOGIN_REQUEST_ERROR;

    constructor(public payload: string) {

    }
}

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

export class DeactivateLoginInformation implements Action {
    readonly type = DEACTIVATE_OWNER_LOGIN;

    constructor(public id: number) {

    }
}

export class DeactivateLoginInformationComplete implements Action {
    readonly type = DEACTIVATE_OWNER_LOGIN_COMPLETE;

    constructor() {

    }
}

export class DeactivateLoginInformationError implements Action {
    readonly type = DEACTIVATE_OWNER_LOGIN_ERROR;

    constructor(public payload: string) {

    }
}

export class DeleteLoginInformation implements Action {
    readonly type = DELETE_OWNER_LOGIN;

    constructor(public id: number) {

    }
}

export class DeleteLoginInformationComplete implements Action {
    readonly type = DELETE_OWNER_LOGIN_COMPLETE;

    constructor() {

    }
}

export class DeleteLoginInformationError implements Action {
    readonly type = DELETE_OWNER_LOGIN_ERROR;

    constructor(public payload: string) {

    }
}

export class ActivateLoginInformation implements Action {
    readonly type = ACTIVATE_OWNER_LOGIN;

    constructor(public id: number) {

    }
}

export class ActivateLoginInformationComplete implements Action {
    readonly type = ACTIVATE_OWNER_LOGIN_COMPLETE;

    constructor() {

    }
}

export class ActivateLoginInformationError implements Action {
    readonly type = ACTIVATE_OWNER_LOGIN_ERROR;

    constructor(public payload: string) {

    }
}

export class GetBlockedCardDetails implements Action {
    readonly type = GET_BLOCKED_CARD_DETAILS;

    constructor(public payload: string) {

    }
}

export class GetBlockedCardDetailsComplete implements Action {
    readonly type = GET_BLOCKED_CARD_DETAILS_COMPLETE;

    constructor(public payload: BlockedCardDetail[]) {

    }
}

export class GetBlockedCardDetailsError implements Action {
    readonly type = GET_BLOCKED_CARD_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class ShowPassword implements Action{
    readonly type = SHOW_PASSWORD;

    constructor(public payload: number){

    }
}

export class ShowPasswordCompleted implements Action{
    readonly type = SHOW_PASSWORD_COMPLETED;

    constructor(public payload: string){

    }
}

export class ShowPasswordError implements Action{
    readonly type = SHOW_PASSWORD_ERROR;

    constructor(public payload: string){

    }
}

export class QrCodeRegister implements Action {
    readonly type = QR_REGISTER_LOGIN;

    constructor(public id: number) {

    }
}

export class QrCodeRegisterComplete implements Action {
    readonly type = QR_REGISTER_LOGIN_COMPLETE;

    constructor() {

    }
}

export class QrCodeRegisterError implements Action {
    readonly type = QR_REGISTER_LOGIN_ERROR;

    constructor(public payload: string) {

    }
}

export class GetQRRegistrationDetails implements Action {
    readonly type = GET_QR_REGISTRATION_DETAILS;

    constructor(public id: number) {

    }
}

export class GetQRRegistrationDetailsComplete implements Action {
    readonly type = GET_QR_REGISTRATION_DETAILS_COMPLETE;
    constructor(public payload: BankLogin) {

    }
}

export class GetQRRegistrationDetailsError implements Action {
    readonly type = GET_QR_REGISTRATION_DETAILS_ERROR;

    constructor(public payload: string) {

    }
}

export class RegisterQrCode implements Action {
    readonly type = REGISTER_QR_CODE;

    constructor(public payload: QrCodeRegistrationRequest) {

    }
}

export class RegisterQrCodeComplete implements Action {
    readonly type = REGISTER_QR_CODE_COMPLETE;
    constructor() {

    }
}

export class RegisterQrCodeError implements Action {
    readonly type = REGISTER_QR_CODE_ERROR;

    constructor(public payload: string) {

    }
}

export class GetOTP implements Action {
    readonly type = GET_OTP;

    constructor(public payload: number) {

    }
}

export class GetOTPCompleted implements Action {
    readonly type = GET_OTP_COMPLETED;

    constructor(public payload: string) {
    }
}

export class GetOTPError implements Action {
    readonly type = GET_OTP_ERROR;

    constructor(public payload: string) {

    }
}

export class RegisterLoginRequest implements Action {
    readonly type = REGISTER_LOGIN_REQUEST;

    constructor(public payload: RegisterLogin) {

    }
}

export class RegisterLoginRequestComplete implements Action {
    readonly type = REGISTER_LOGIN_REQUEST_COMPLETE;

    constructor() {

    }
}

export class RegisterLoginRequestError implements Action {
    readonly type = REGISTER_LOGIN_REQUEST_ERROR;

    constructor(public payload: string) {

    }
}

export class SwitchBankConnectionProgram implements Action {
    readonly type = SWITCH_BANK_CONNECTION_PROGRAM;

    constructor(public payload: number) {

    }
}

export class SwitchBankConnectionProgramCompleted implements Action {
    readonly type = SWITCH_BANK_CONNECTION_PROGRAM_COMPLETE;

    constructor(public payload: string) {
    }
}

export class SwitchBankConnectionProgramError implements Action {
    readonly type = SWITCH_BANK_CONNECTION_PROGRAM_ERROR;

    constructor(public payload: string) {

    }
}

export type Actions =
    Search | SearchComplete | SearchError
    | SearchAccounts | SearchAccountsComplete | SearchAccountsError
    | SearchBanks | SearchBanksComplete | SearchBanksError
    | Create | CreateComplete | CreateError
    | GetDetails | GetDetailsComplete | GetDetailsError
    | Edit | EditComplete | EditError
    | SearchAccountsByLoginGuid | SearchAccountsBYLoginGuidCompleted | SearchAccountsByLoginGuidError
    | UpdateLoginInformation | UpdateLoginInformationComplete | UpdateLoginInformationError
    | CreateLoginFromLoginRequest | CreateLoginFromLoginRequestComplete | CreateLoginFromLoginRequestError
    | GetOwnerLoginList | GetOwnerLoginListComplete | GetOwnerLoginListError
    | DeactivateLoginInformation | DeactivateLoginInformationComplete | DeactivateLoginInformationError
    | DeleteLoginInformation | DeleteLoginInformationComplete | DeleteLoginInformationError
    | ActivateLoginInformation | ActivateLoginInformationComplete | ActivateLoginInformationError
    | GetBlockedCardDetails | GetBlockedCardDetailsComplete | GetBlockedCardDetailsError
    | ShowPassword | ShowPasswordCompleted | ShowPasswordError
    | QrCodeRegister | QrCodeRegisterComplete | QrCodeRegisterError
    | GetQRRegistrationDetails | GetQRRegistrationDetailsComplete | GetQRRegistrationDetailsError
    | RegisterQrCode | RegisterQrCodeComplete | RegisterQrCodeError
    | GetOTP | GetOTPCompleted | GetOTPError
    | RegisterLoginRequest | RegisterLoginRequestComplete | RegisterLoginRequestError
    | SwitchBankConnectionProgram | SwitchBankConnectionProgramCompleted | SwitchBankConnectionProgramError
    | ClearAll;