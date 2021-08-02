import * as notification from '../actions/notification';

export interface NotificationState {
  messages: { [id: number]: notification.NotificationMessage };
  messageCount: number;
}

const initialState: NotificationState = {
  messages: {},
  messageCount: 0
};

export function notificationReducer(state: NotificationState = initialState, action: notification.NotificationActions): NotificationState {
  switch (action.type) {
    case notification.ADD_ONE:
      return {
        messages: {
          ...state.messages,
          [state.messageCount]: action.payload
        },
        messageCount: state.messageCount + 1
      };

    case notification.CLEAR_ALL:
      return {
        messages: {},
        messageCount: 0
      };

    default:
      return state;
  }
}
