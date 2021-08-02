import * as applicationSettings from '../actions/applicationSettings';
import { ApplicationSettings, TransferStatusDescription } from '../../models/application-settings';
import { OwnerSetting } from 'app/models/owner-setting';

export interface State {
    applicationSettingsDetail: ApplicationSettings;
    loading: boolean;
    transferStatuses: TransferStatusDescription[];
    ownerSetting: OwnerSetting,
    error?: {
        editApplicationSettings: string,
        getDetails: string,
        transferStatuses: string,
        ownerSetting: string
    };
    applicationSettingsCreated: ApplicationSettings;
    applicationSettingsUpdateSuccess: boolean;
    ownerSettingUpdated: boolean;
}

const initialState: State = {
    applicationSettingsDetail: undefined,
    loading: false,
    transferStatuses: undefined,
    ownerSetting: undefined,
    error: undefined,
    applicationSettingsCreated: undefined,
    applicationSettingsUpdateSuccess: false,
    ownerSettingUpdated: false
};

export function reducer(state: State = initialState, action: applicationSettings.Actions): State {
    switch (action.type) {

        case applicationSettings.GET_DETAILS:
            return {
                ...state,
                loading: true,
                error: undefined,
                applicationSettingsDetail: undefined,
            };

        case applicationSettings.GET_DETAILS_COMPLETE:
            return {
                ...state,
                loading: false,
                applicationSettingsDetail: action.payload,
            };

        case applicationSettings.GET_DETAILS_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                applicationSettingsCreated: undefined,
            };

        case applicationSettings.GET_TRANSFER_STATUS:
            return {
                ...state,
                error: undefined
            };

        case applicationSettings.GET_TRANSFER_STATUS_COMPLETE:
            return {
                ...state,
                error: undefined,
                transferStatuses: action.payload
            }

        case applicationSettings.GET_TRANSFER_STATUS_ERROR:
            return {
                ...state,
                error: {
                    ...state.error,
                    transferStatuses: action.payload
                },
                transferStatuses: undefined
            }

        case applicationSettings.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                applicationSettingsCreated: undefined,
                applicationSettingsUpdateSuccess: false
            };

        case applicationSettings.EDIT_COMPLETE:
            return {
                ...state,
                loading: false,
                applicationSettingsCreated: undefined,
                applicationSettingsUpdateSuccess: true
            };
        case applicationSettings.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editApplicationSettings: action.payload
                },
                applicationSettingsCreated: undefined,
            };
        case applicationSettings.GET_OWNER_SETTING:
            return {
                ...state,
                loading: true,
                ownerSetting: undefined,
                error: undefined
            };
        case applicationSettings.GET_OWNER_SETTING_COMPLETED:
            return {
                ...state,
                ownerSetting: action.payload,
                loading: false,
                error: undefined
            }
        case applicationSettings.GET_OWNER_SETTING_ERROR: {
            return {
                ...state,
                loading: false,
                ownerSetting: undefined,
                error: {
                    ...state.error,
                    ownerSetting: action.payload
                }
            }
        }
        case applicationSettings.SAVE_OWNER_SETTING:
            {
            return {
                ...state,
                loading: true,
                ownerSettingUpdated: false,
                error: undefined
            }
        }

        case applicationSettings.SAVE_OWNER_SETTING_COMPLETED:{
            return {
                ...state,
                loading: false,
                ownerSettingUpdated: true,
                error: undefined,
                ownerSetting: action.payload
            }
        }

        case applicationSettings.SAVE_OWNER_SETTING_ERROR:{
            return{
                ...state,
                loading: false,
                ownerSettingUpdated: false,
                error: {
                    ...state.error,
                    ownerSetting: action.payload
                }
            }
        }

        case applicationSettings.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case applicationSettings.CLEAR:
            return initialState;
        default:
            return state;
    }
}