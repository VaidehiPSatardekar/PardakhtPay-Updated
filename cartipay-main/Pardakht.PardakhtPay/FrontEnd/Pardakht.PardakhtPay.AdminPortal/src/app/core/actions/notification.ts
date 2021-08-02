import { Action } from '@ngrx/store';
import { NotificationType } from 'angular2-notifications';

export const ADD_ONE = '[notification] Add one notification';
export const CLEAR_ALL = '[notification] Clear all notifications';

export class AddOne implements Action {
  readonly type = ADD_ONE;

  constructor(public payload: NotificationMessage) { }
}

export class ClearAll implements Action {
  readonly type = CLEAR_ALL;
}

export type NotificationActions = AddOne | ClearAll;


export class NotificationMessage {
 
  constructor(public message: string | { [key: string]: string[] }, public type: NotificationType = NotificationType.Info)  {}
}
