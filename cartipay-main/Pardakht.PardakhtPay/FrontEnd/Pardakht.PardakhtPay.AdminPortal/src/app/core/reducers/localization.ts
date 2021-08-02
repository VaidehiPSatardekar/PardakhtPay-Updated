import * as localization from '../actions/localization';

export interface State {
  languageCode: string;
  dictionary: { [key: string]: string };
  loading: boolean;
}


const initialState: State = {
  languageCode: "en",
  dictionary: {},
  loading: false
};

export function reducer(state = initialState, action: localization.Actions): State {
  switch (action.type) {
    case localization.LOAD:
      return {
        ...state,
        loading: true,
      };

    case localization.LOAD_COMPLETE:
      return {
        loading: false,
        dictionary: action.payload.dictionary,
        languageCode: action.payload.languageCode,
      };

    default: {
      return state;
    }
  }
}

export const getDictionary = (state: State) => state.dictionary;

export const getDictionaryByKey = (state: State, key: string) => {
  let value = state.dictionary[key];
  return value === undefined ? key : value;
};

export const getDictionaryForEnglish = (state: State) => {
  let value = state.dictionary['EN'];
  return value === undefined ? 'EN' : value;
};