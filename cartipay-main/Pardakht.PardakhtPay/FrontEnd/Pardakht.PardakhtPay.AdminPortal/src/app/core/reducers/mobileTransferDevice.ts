import { MobileTransferDevice } from "../../models/mobile-transfer";
import * as mobileTransferDevice from '../actions/mobile-transfer-device';

export interface State {
    allMobileTransferDevices: MobileTransferDevice[];
    mobileTransferDeviceDetails: MobileTransferDevice;
    loading: boolean;
    getDetailLoading: boolean;
    error?: {
        searchError: string,
        createMobileTransferDevice: string,
        editMobileTransferDevice: string,
        getDetails: string,
        getAll: string,
        sendSmsError: string,
        activateError: string,
        checkStatusError: string,
        removeError: string
    };
    query: string;
    mobileTransferDeviceCreated: MobileTransferDevice;
    mobileTransferDeviceUpdateSuccess: boolean;
    sendSmsCompleted: boolean;
    activateCompleted: boolean;
    checkStatusCompleted: boolean;
    removeCompleted: boolean;
}

const initialState: State = {
    allMobileTransferDevices: [],
    mobileTransferDeviceDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    mobileTransferDeviceCreated: undefined,
    mobileTransferDeviceUpdateSuccess: false,
    sendSmsCompleted: false,
    activateCompleted: false,
    checkStatusCompleted: false,
    removeCompleted: false
};

export function reducer(state: State = initialState, action: mobileTransferDevice.Actions): State {
    switch (action.type) {
        case mobileTransferDevice.GET_ALL:

            return {
                ...state,
                allMobileTransferDevices : [],
                loading: true,
                error: undefined
            }

        case mobileTransferDevice.GET_ALL_COMPLETE:
            return {
                ...state,
                allMobileTransferDevices: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case mobileTransferDevice.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAll: action.payload
                }
            }

        case mobileTransferDevice.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                mobileTransferDeviceCreated: undefined,
            };

        case mobileTransferDevice.CREATE_COMPLETE:


            const newMobileTransferDevices: MobileTransferDevice[] = [
                ...state.allMobileTransferDevices
            ];
            newMobileTransferDevices.push(action.payload);
            return {
                ...state,
                allMobileTransferDevices: newMobileTransferDevices,
                loading: false,
                mobileTransferDeviceCreated: action.payload,
            };

        case mobileTransferDevice.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createMobileTransferDevice: action.payload
                },
                mobileTransferDeviceCreated: undefined,
            };

        case mobileTransferDevice.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                mobileTransferDeviceDetails: undefined,
            };

        case mobileTransferDevice.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                mobileTransferDeviceDetails: action.payload,
            };

        case mobileTransferDevice.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                mobileTransferDeviceCreated: undefined,
            };

        case mobileTransferDevice.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                mobileTransferDeviceCreated: undefined,
                mobileTransferDeviceUpdateSuccess: false
            };

        case mobileTransferDevice.EDIT_COMPLETE:
            {
                const newMobileTransferDevices: MobileTransferDevice[] = [
                    ...state.allMobileTransferDevices
                ];
                newMobileTransferDevices.splice(newMobileTransferDevices.findIndex(q => q.id === action.payload.id), 1, action.payload);
                return {
                    ...state,
                    allMobileTransferDevices: newMobileTransferDevices,
                    loading: false,
                    mobileTransferDeviceCreated: undefined,
                    mobileTransferDeviceUpdateSuccess: true
                };
            }

        case mobileTransferDevice.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editMobileTransferDevice: action.payload
                },
                mobileTransferDeviceCreated: undefined,
            };

        case mobileTransferDevice.SEND_SMS:
            return {
                ...state,
                loading: true,
                error: undefined,
                sendSmsCompleted: false
            };

        case mobileTransferDevice.SEND_SMS_COMPLETE:
            {
                const newMobileTransferDevices: MobileTransferDevice[] = [
                    ...state.allMobileTransferDevices
                ];
                newMobileTransferDevices.splice(newMobileTransferDevices.findIndex(q => q.id === action.payload.id), 1, action.payload);
                return {
                    ...state,
                    allMobileTransferDevices: newMobileTransferDevices,
                    loading: false,
                    sendSmsCompleted: true
                };
            }

        case mobileTransferDevice.SEND_SMS_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    sendSmsError: action.payload
                },
                sendSmsCompleted: false,
            };

        case mobileTransferDevice.ACTIVATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                activateCompleted: false
            };

        case mobileTransferDevice.ACTIVATE_COMPLETE:
            {
                const newMobileTransferDevices: MobileTransferDevice[] = [
                    ...state.allMobileTransferDevices
                ];
                newMobileTransferDevices.splice(newMobileTransferDevices.findIndex(q => q.id === action.payload.id), 1, action.payload);
                return {
                    ...state,
                    allMobileTransferDevices: newMobileTransferDevices,
                    loading: false,
                    activateCompleted: true
                };
            }

        case mobileTransferDevice.ACTIVATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    activateError: action.payload
                },
                activateCompleted: false,
            };

        case mobileTransferDevice.CHECK_STATUS:
            return {
                ...state,
                loading: true,
                error: undefined,
                checkStatusCompleted: false
            };

        case mobileTransferDevice.CHECK_STATUS_COMPLETE:
            {
                const newMobileTransferDevices: MobileTransferDevice[] = [
                    ...state.allMobileTransferDevices
                ];
                newMobileTransferDevices.splice(newMobileTransferDevices.findIndex(q => q.id === action.payload.id), 1, action.payload);
                return {
                    ...state,
                    allMobileTransferDevices: newMobileTransferDevices,
                    loading: false,
                    checkStatusCompleted: true
                };
            }

        case mobileTransferDevice.CHECK_STATUS_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    sendSmsError: action.payload
                },
                checkStatusCompleted: false,
            };

        case mobileTransferDevice.REMOVE:
            return {
                ...state,
                loading: true,
                error: undefined,
                removeCompleted: false
            };

        case mobileTransferDevice.REMOVE_COMPLETE:
            {
                const newMobileTransferDevices: MobileTransferDevice[] = [
                    ...state.allMobileTransferDevices
                ];
                newMobileTransferDevices.splice(newMobileTransferDevices.findIndex(q => q.id === action.payload.id), 1, action.payload);
                return {
                    ...state,
                    allMobileTransferDevices: newMobileTransferDevices,
                    loading: false,
                    removeCompleted: true
                };
            }

        case mobileTransferDevice.REMOVE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    sendSmsError: action.payload
                },
                removeCompleted: false,
            };

        case mobileTransferDevice.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case mobileTransferDevice.CLEAR:
            return initialState;
        default:
            return state;
    }
}