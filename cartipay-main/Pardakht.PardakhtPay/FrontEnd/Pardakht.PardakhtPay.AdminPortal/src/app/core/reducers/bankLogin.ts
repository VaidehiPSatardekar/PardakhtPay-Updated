import * as bankLogin from '../actions/bankLogin';
import { BankLogin, BankAccount } from '../../models/bank-login';
import { Bank } from '../../models/bank';
import { BlockedCardDetail } from '../../models/blocked-card-detail';

export interface State {
    bankLogins: BankLogin[];
    bankAccounts: BankAccount[];
    ownerLogins: BankLogin[];
    blockedCards: BlockedCardDetail[];
    createSuccess: boolean;
    updateSuccess: boolean;
    created: BankLogin;
    detail: BankLogin;
    banks: Bank[];
    loading: boolean;
    accountsLoading: boolean;
    banksLoading: boolean;
    getDetailLoading: boolean;
    updateLoginSuccess: boolean;
    createLoginFromLoginRequestLoading: boolean;
    createLoginSuccess: boolean;
    ownerBankLoginLoading: boolean;
    deactiveLoginSuccess: boolean;
    deleteLoginSuccess: boolean;
    deactivatingLogin: boolean;
    deletingLogin: boolean;
    activatingLogin: boolean;
    activateLoginSuccess: boolean;
    blockedCardDetailLoading: boolean;
    password: string;
    qrCodeRegisterSuccess: boolean;
    qrCodeRegister: boolean;
    qrCodeRegistrationCompleted: boolean;
    getOTP: string,
    registerLoginRequestLoading: boolean;
    registerLoginRequestSuccess: boolean;
    switchBankConnectionProgram: string;
    error?: {
        searchError: string,
        searchBankAccountError: string;
        searchBanksError: string;
        createError: string;
        getDetailError: string;
        updateLogin: string;
        createLoginError: string;
        ownerBankLoginError: string;
        deactivaLoginError: string;
        deleteLoginError: string;
        activateLoginError: string;
        blockedCardError: string;
        showPasswordError: string;
        qrCodeRegisterError: string;
        getOTPError: string;
        registerLoginRequestError: string;        
        switchBankConnectionProgramError: string;
    };
}

const initialState: State = {
    bankLogins: undefined,
    bankAccounts: undefined,
    ownerLogins: undefined,
    blockedCards: undefined,
    createSuccess: false,
    updateSuccess: false,
    created: undefined,
    detail: undefined,
    banks: undefined,
    loading: false,
    accountsLoading: false,
    banksLoading: false,
    getDetailLoading: false,
    updateLoginSuccess: false,
    createLoginFromLoginRequestLoading: false,
    createLoginSuccess: false,
    ownerBankLoginLoading: false,
    deactivatingLogin: false,
    deletingLogin: false,
    deactiveLoginSuccess: false,
    deleteLoginSuccess: false,
    activateLoginSuccess: false,
    activatingLogin: false,
    blockedCardDetailLoading: false,
    error: undefined,
    password: undefined,
    qrCodeRegisterSuccess: false,
    qrCodeRegister: false,
    qrCodeRegistrationCompleted: false,
    getOTP: undefined,
    registerLoginRequestLoading: false,
    registerLoginRequestSuccess: false,
    switchBankConnectionProgram: undefined
}

export function reducer(state: State = initialState, action: bankLogin.Actions): State {
    switch (action.type) {
        case bankLogin.SEARCH:

            return {
                ...state,
                bankLogins: undefined,
                loading: true,
                error: {
                    ...state.error,
                    searchError: undefined
                }
            }

        case bankLogin.SEARCH_COMPLETE:
            return {
                ...state,
                bankLogins: action.payload,
                loading: false,
                error: {
                    ...state.error,
                    searchError: undefined
                }
            }

        case bankLogin.SEARCH_ERROR:
            return {
                ...state,
                bankLogins: undefined,
                loading: false,
                error: {
                    ...state.error,
                    searchError: action.payload
                }
            }

        case bankLogin.SEARCH_ACCOUNTS:

            return {
                ...state,
                bankAccounts: undefined,
                accountsLoading: true,
                error: {
                    ...state.error,
                    searchBankAccountError: undefined
                }
            }

        case bankLogin.SEARCH_ACCOUNTS_COMPLETE:
            return {
                ...state,
                bankAccounts: action.payload,
                accountsLoading: false,
                error: {
                    ...state.error,
                    searchBankAccountError: undefined
                }
            }

        case bankLogin.SEARCH_ACCOUNTS_ERROR:
            return {
                ...state,
                bankAccounts: undefined,
                accountsLoading: false,
                error: {
                    ...state.error,
                    searchBankAccountError: action.payload
                }
            }

        case bankLogin.SEARCH_BANKS:

            return {
                ...state,
                banks: undefined,
                banksLoading: true,
                error: {
                    ...state.error,
                    searchBankAccountError: undefined
                }
            }

        case bankLogin.SEARCH_BANKS_COMPLETE:
            return {
                ...state,
                banks: action.payload,
                banksLoading: false,
                error: {
                    ...state.error,
                    searchBanksError: undefined
                }
            }

        case bankLogin.SEARCH_BANKS_ERROR:
            return {
                ...state,
                banks: undefined,
                banksLoading: false,
                error: {
                    ...state.error,
                    searchBanksError: action.payload
                }
            }

        case bankLogin.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                created: undefined,
                createSuccess: false
            };

        case bankLogin.CREATE_COMPLETE:

            return {
                ...state,
                loading: false,
                createSuccess: true,
                created: action.payload
            };

        case bankLogin.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                created: undefined,
                error: {
                    ...state.error,
                    createError: action.payload
                },
                createSuccess: false
            };


        case bankLogin.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                qrCodeRegistrationCompleted: false,
                error: undefined,
                detail: undefined,
            };

        case bankLogin.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                qrCodeRegistrationCompleted: false,
                detail: action.payload,
            };

        case bankLogin.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                qrCodeRegistrationCompleted: false,
                error: {
                    ...state.error,
                    getDetailError: action.payload
                },
                detail: undefined,
            };

        case bankLogin.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                updateSuccess: false
            };

        case bankLogin.EDIT_COMPLETE:

            return {
                ...state,
                loading: false,
                updateSuccess: true
            };

        case bankLogin.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createError: action.payload
                },
                updateSuccess: false
            };

        case bankLogin.SEARCH_ACCOUNTS_BY_LOGIN_GUID:

            return {
                ...state,
                bankAccounts: undefined,
                accountsLoading: true,
                error: {
                    ...state.error,
                    searchBankAccountError: undefined
                }
            }

        case bankLogin.SEARCH_ACCOUNTS_BY_LOGIN_GUID_COMPLETED:
            return {
                ...state,
                bankAccounts: action.payload,
                accountsLoading: false,
                error: {
                    ...state.error,
                    searchBankAccountError: undefined
                }
            }

        case bankLogin.SEARCH_ACCOUNTS_BY_LOGIN_GUID_ERROR:
            return {
                ...state,
                bankAccounts: undefined,
                accountsLoading: false,
                error: {
                    ...state.error,
                    searchBankAccountError: action.payload
                }
            }

        case bankLogin.UPDATE_LOGIN_INFORMATION:
            return {
                ...state,
                updateLoginSuccess: false,
                error: {
                    ...state.error,
                    updateLogin: undefined
                }
            };

        case bankLogin.UPDATE_LOGIN_INFORMATION_COMPLETE:
            return {
                ...state,
                updateLoginSuccess: true,
                error: {
                    ...state.error,
                    updateLogin: undefined
                }
            };

        case bankLogin.UPDATE_LOGIN_INFORMATION_ERROR:
            return {
                ...state,
                updateLoginSuccess: false,
                error: {
                    ...state.error,
                    updateLogin: action.payload
                }
            }

        case bankLogin.CREATE_LOGIN_FROM_LOGIN_REQUEST:
            return {
                ...state,
                createLoginSuccess: false,
                createLoginFromLoginRequestLoading: true,
                error: {
                    ...state.error,
                    createLoginError: undefined
                }
            };

        case bankLogin.CREATE_LOGIN_FROM_LOGIN_REQUEST_COMPLETE:
            return {
                ...state,
                createLoginSuccess: true,
                createLoginFromLoginRequestLoading: false,
                error: {
                    ...state.error,
                    createLoginError: undefined
                }
            };

        case bankLogin.CREATE_LOGIN_FROM_LOGIN_REQUEST_ERROR:
            return {
                ...state,
                createLoginSuccess: false,
                createLoginFromLoginRequestLoading: false,
                error: {
                    ...state.error,
                    createLoginError: action.payload
                }
            }

        case bankLogin.GET_OWNER_LOGIN_LIST:

            return {
                ...state,
                ownerLogins: undefined,
                ownerBankLoginLoading: true,
                error: {
                    ...state.error,
                    ownerBankLoginError: undefined
                }
            }

        case bankLogin.GET_OWNER_LOGIN_LIST_COMPLETE:
            return {
                ...state,
                ownerLogins: action.payload,
                ownerBankLoginLoading: false,
                error: {
                    ...state.error,
                    ownerBankLoginError: undefined
                }
            }

        case bankLogin.GET_OWNER_LOGIN_LIST_ERROR:
            return {
                ...state,
                ownerLogins: undefined,
                ownerBankLoginLoading: false,
                error: {
                    ...state.error,
                    ownerBankLoginError: action.payload
                }
            }

        case bankLogin.DEACTIVATE_OWNER_LOGIN:
            return {
                ...state,
                deactivatingLogin: true,
                deactiveLoginSuccess: false,
                error: {
                    ...state.error,
                    deactivaLoginError: undefined
                }
            };

        case bankLogin.DEACTIVATE_OWNER_LOGIN_COMPLETE:
            return {
                ...state,
                deactivatingLogin: false,
                deactiveLoginSuccess: true,
                error: {
                    ...state.error,
                    deactivaLoginError: undefined
                }
            };

        case bankLogin.DEACTIVATE_OWNER_LOGIN_ERROR:
            return {
                ...state,
                deactivatingLogin: false,
                deactiveLoginSuccess: false,
                error: {
                    ...state.error,
                    deactivaLoginError: action.payload
                }
            }

        case bankLogin.DELETE_OWNER_LOGIN:
            return {
                ...state,
                deletingLogin: true,
                deleteLoginSuccess: false,
                error: {
                    ...state.error,
                    deleteLoginError: undefined
                }
            };

        case bankLogin.DELETE_OWNER_LOGIN_COMPLETE:
            return {
                ...state,
                deletingLogin: false,
                deleteLoginSuccess: true,
                error: {
                    ...state.error,
                    deleteLoginError: undefined
                }
            };

        case bankLogin.DELETE_OWNER_LOGIN_ERROR:
            return {
                ...state,
                deleteLoginSuccess: false,
                deletingLogin: false,
                error: {
                    ...state.error,
                    deleteLoginError: action.payload
                }
            }

        case bankLogin.ACTIVATE_OWNER_LOGIN:
            return {
                ...state,
                activatingLogin: true,
                activateLoginSuccess: false,
                error: {
                    ...state.error,
                    activateLoginError: undefined
                }
            };

        case bankLogin.ACTIVATE_OWNER_LOGIN_COMPLETE:
            return {
                ...state,
                activatingLogin: false,
                activateLoginSuccess: true,
                error: {
                    ...state.error,
                    activateLoginError: undefined
                }
            };

        case bankLogin.ACTIVATE_OWNER_LOGIN_ERROR:
            return {
                ...state,
                activatingLogin: false,
                activateLoginSuccess: false,
                error: {
                    ...state.error,
                    activateLoginError: action.payload
                }
            }

        case bankLogin.GET_BLOCKED_CARD_DETAILS:

            return {
                ...state,
                blockedCards: undefined,
                blockedCardDetailLoading: true,
                error: {
                    ...state.error,
                    blockedCardError: undefined
                }
            }

        case bankLogin.GET_BLOCKED_CARD_DETAILS_COMPLETE:
            return {
                ...state,
                blockedCards: action.payload,
                blockedCardDetailLoading: false,
                error: {
                    ...state.error,
                    blockedCardError: undefined
                }
            }

        case bankLogin.GET_BLOCKED_CARD_DETAILS_ERROR:
            return {
                ...state,
                blockedCards: undefined,
                blockedCardDetailLoading: false,
                error: {
                    ...state.error,
                    blockedCardError: action.payload
                }
            }

            case bankLogin.SHOW_PASSWORD:
    
                return {
                    ...state,
                    password: undefined,
                    loading: true,
                    error: {
                        ...state.error,
                        showPasswordError: undefined
                    }
                }
    
            case bankLogin.SHOW_PASSWORD_COMPLETED:
                return {
                    ...state,
                    password: action.payload,
                    loading: false,
                    error: {
                        ...state.error,
                        showPasswordError: undefined
                    }
                }
    
            case bankLogin.SHOW_PASSWORD_ERROR:
                return {
                    ...state,
                    password: undefined,
                    loading: false,
                    error: {
                        ...state.error,
                        showPasswordError: action.payload
                    }
                }

        case bankLogin.QR_REGISTER_LOGIN:
            return {
                ...state,
                qrCodeRegister: true,
                qrCodeRegisterSuccess: false,
                qrCodeRegistrationCompleted: false,
                error: {
                    ...state.error,
                    qrCodeRegisterError: undefined
                }
            };

        case bankLogin.QR_REGISTER_LOGIN_COMPLETE:
            return {
                ...state,
                qrCodeRegister: false,
                qrCodeRegisterSuccess: true,
                qrCodeRegistrationCompleted: false,
                error: {
                    ...state.error,
                    qrCodeRegisterError: undefined
                }
            };

        case bankLogin.QR_REGISTER_LOGIN_ERROR:
            return {
                ...state,
                qrCodeRegister: false,
                qrCodeRegisterSuccess: false,
                qrCodeRegistrationCompleted: false,
                error: {
                    ...state.error,
                    qrCodeRegisterError: action.payload
                }
            }

            case bankLogin.REGISTER_QR_CODE:
                return {
                    ...state,
                    loading: true,
                    qrCodeRegistrationCompleted: false,
                    error: {
                        ...state.error,
                        qrCodeRegisterError: undefined
                    }
                };
    
            case bankLogin.REGISTER_QR_CODE_COMPLETE:
                return {
                    ...state,
                    loading: false,
                    qrCodeRegistrationCompleted: true,
                    error: {
                        ...state.error,
                        qrCodeRegisterError: undefined
                    }
                };
    
            case bankLogin.REGISTER_QR_CODE_ERROR:
                return {
                    ...state,
                    loading: false,
                    qrCodeRegistrationCompleted: false,
                    error: {
                        ...state.error,
                        qrCodeRegisterError: action.payload
                    }
            }
        case bankLogin.GET_OTP:

            return {
                ...state,
                getOTP: undefined,
                loading: true,
                error: {
                    ...state.error,
                    getOTPError: undefined
                }
            }

        case bankLogin.GET_OTP_COMPLETED:
            return {
                ...state,
                getOTP: action.payload,
                loading: false,
                error: {
                    ...state.error,
                    getOTPError: undefined
                }
            }

        case bankLogin.GET_OTP_ERROR:
            return {
                ...state,
                getOTP: undefined,
                loading: false,
                error: {
                    ...state.error,
                    getOTPError: action.payload
                }
            }

        case bankLogin.REGISTER_LOGIN_REQUEST:
            return {
                ...state,
                registerLoginRequestSuccess: false,
                createLoginFromLoginRequestLoading: true,
                error: {
                    ...state.error,
                    registerLoginRequestError: undefined
                }
            };

        case bankLogin.REGISTER_LOGIN_REQUEST_COMPLETE:
            return {
                ...state,
                registerLoginRequestSuccess: true,
                createLoginFromLoginRequestLoading: false,
                error: {
                    ...state.error,
                    registerLoginRequestError: undefined
                }
            };

        case bankLogin.REGISTER_LOGIN_REQUEST_ERROR:
            return {
                ...state,
                registerLoginRequestSuccess: false,
                createLoginFromLoginRequestLoading: false,
                error: {
                    ...state.error,
                    registerLoginRequestError: action.payload
                }
            }
        case bankLogin.SWITCH_BANK_CONNECTION_PROGRAM:

            return {
                ...state,
                switchBankConnectionProgram: undefined,
                loading: true,
                error: {
                    ...state.error,
                    switchBankConnectionProgramError: undefined
                }
            }

        case bankLogin.SWITCH_BANK_CONNECTION_PROGRAM_COMPLETE:
            return {
                ...state,
                switchBankConnectionProgram: action.payload,
                loading: false,
                error: {
                    ...state.error,
                    switchBankConnectionProgramError: undefined
                }
            }

        case bankLogin.SWITCH_BANK_CONNECTION_PROGRAM_ERROR:
            return {
                ...state,
                switchBankConnectionProgram: undefined,
                loading: false,
                error: {
                    ...state.error,
                    switchBankConnectionProgramError: action.payload
                }
            }
        case bankLogin.CLEAR_ALL:
            return initialState;
        default:
            return state;
    }
}