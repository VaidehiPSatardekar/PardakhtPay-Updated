import { Action } from '@ngrx/store';
import { Localization } from '../../models/types';

export const LOAD = '[Localization] Load';
export const LOAD_COMPLETE = '[Localization] Load Complete';


export class Load implements Action {
  readonly type = LOAD;

  constructor(public payload: string) { }
}

export class LoadComplete implements Action {
  readonly type = LOAD_COMPLETE;

  constructor(public payload: Localization) { }
}

/**
 * Export a type alias of all actions in this action group
 * so that reducers can easily compose action types
 */
export type Actions = Load | LoadComplete;
