
import { HttpErrorResponse } from '@angular/common/http';

export class EffectBase {

  sanitiseError(responseError: HttpErrorResponse): string {
    let message: string = '';
    if (responseError.status === 400) {
      // replace 'bad request' with user friendly error message
      message = responseError.error;
    } else {
      message = 'Unable to perform requested action at this time - please try again later';
    }

    return message;
  }

}
