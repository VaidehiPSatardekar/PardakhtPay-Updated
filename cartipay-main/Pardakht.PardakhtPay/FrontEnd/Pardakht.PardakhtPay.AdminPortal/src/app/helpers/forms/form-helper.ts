import { FormControl, FormGroup } from '@angular/forms';
// tslint:disable:function-name

import { AbstractControl, ValidatorFn } from '@angular/forms';

export class FormHelper {

    static validateFormGroup(formGroup: FormGroup): void {
        Object.keys(formGroup.controls).forEach((field: string) => {
            const control: AbstractControl = formGroup.get(field);
            if (control instanceof FormControl) {
                control.markAsTouched({ onlySelf: true });
            } else if (control instanceof FormGroup) {
                this.validateFormGroup(control);
            }
        });
    }

    static getErrorMessage(control: FormControl): string {
        let error: string = '';

        if (control.hasError('required')) {
            error = 'You must enter a value';
        }
        else if (control.hasError('email')) {
            error = 'Not a valid email';
        }
        else if (control.hasError('minlength')) {
            error = 'Value entered must be at least ' + control.getError('minlength').requiredLength + ' characters in length';
        }
        else if (control.hasError('max')) {
            error = 'Value entered must be less than ' + control.getError('max').max;
        }
        else if (control.hasError('min')) {
            error = 'Value entered must be more than ' + control.getError('min').min;
        }
        else if (control.hasError('nomatch')) {
            // error = 'Passwords do not match';
            error = control.getError('nomatch');
        }
        else if (control.hasError('pattern')) {
            error = control.getError('pattern');
        }

        return error;
    }
}

export class CustomValidators {

  /**
   * Validator that compares the control value to another control value.
   */

  static match(compareControlName: string, errorMessage?: string): ValidatorFn {
    return (c: AbstractControl): any => {
      if (c === undefined || c === null) {
        return null;
      }

      const otherControl: AbstractControl = c.root.get(compareControlName);
      if (otherControl === undefined || otherControl === null) {
        return null;
      }

      // return current inputs validation
      const response: any = errorMessage !== undefined && !isEmptyInputValue(errorMessage) ? { match: errorMessage } : { match: true };

      return otherControl.value === c.value ? null : response;
    };
  }

  static minDate(minDate: Date, errorMessage?: string): ValidatorFn {
    return (c: AbstractControl): any => {
      if (c === undefined || c === null) {
        return null;
      }
      const response: any = errorMessage !== undefined && !isEmptyInputValue(errorMessage) ? { minDate: errorMessage } : { minDate: true };

      return c.value >= minDate ? null : response;
    };
  }

  static maxDate(maxDate: Date, errorMessage?: string): ValidatorFn {
    return (c: AbstractControl): any => {
      if (c === undefined || c === null) {
        return null;
      }
      const response: any = errorMessage !== undefined && !isEmptyInputValue(errorMessage) ? { minDate: errorMessage } : { minDate: true };

      return c.value <= maxDate ? null : response;
    };
  }

  static minDateControl(controlName: string, errorMessage?: string): ValidatorFn {
    return (c: AbstractControl): any => {
      if (c === undefined || c === null) {
        return null;
      }
      const response: any = errorMessage !== undefined && !isEmptyInputValue(errorMessage) ? { minDate: errorMessage } : { minDate: true };

      const minDate: AbstractControl = c.root.get(controlName);
      if (minDate === undefined || minDate === null) {
        return null;
      }

      return c.value >= minDate.value ? null : response;
    };
  }

  static maxDateControl(controlName: string, errorMessage?: string): ValidatorFn {
    return (c: AbstractControl): any => {
      if (c === undefined || c === null) {
        return null;
      }
      const response: any = errorMessage !== undefined && !isEmptyInputValue(errorMessage) ? { minDate: errorMessage } : { minDate: true };

      const maxDate: AbstractControl = c.root.get(controlName);
      if (maxDate === undefined || maxDate === null) {
        return null;
      }

      return c.value <= maxDate.value ? null : response;
    };
  }

  static requiredIf(predicate: () => boolean, errorMessage?: string): ValidatorFn {
    return (c: AbstractControl): any => {
      if (c === undefined) {
        return null;
      }
      const response: any = errorMessage !== undefined && !isEmptyInputValue(errorMessage) ? { requiredIf: errorMessage } : { requiredIf: true };

      return predicate.call(undefined) && isEmptyInputValue(c.value) ? response : null;
    };
  }

  static validateIf(predicate: () => boolean, validator: ValidatorFn): ValidatorFn {
    return (c: AbstractControl): any => {
      if (c === undefined) {
        return null;
      }

      return predicate.call(undefined) ? validator(c) : null;
    };
  }

  static addErrorMessage(validator: ValidatorFn, errorMessage: string): ValidatorFn {
    return (c: AbstractControl): any => {
      if (c === undefined) {
        return null;
      }
      const valid: { [key: string]: any } = validator(c);
      if (valid !== null) {
        // tslint:disable-next-line:no-for-in
        for (const key in valid) {
          if (valid.hasOwnProperty(key)) {
            valid[key] = errorMessage;
            break;
          }
        }
      }

      return valid;
    };
  }

  static matchingPasswords(control1: string, control2: string, errorMessage?: string): ValidatorFn {
    return (c: AbstractControl): any => {
      if (c === undefined || c === null) {
        return null;
      }

      const newPassword: any = c.get(control1);
      const confirmPassword: any = c.get(control2);

      // if no values, valid
      if (!newPassword || !confirmPassword) {
        return null;
      }

      // return current inputs validation
      const response: any = errorMessage !== undefined && !isEmptyInputValue(errorMessage) ? { nomatch: errorMessage } : { nomatch: true };

      return  newPassword.value === confirmPassword.value ? null : response;
    };
  }
}

function isEmptyInputValue(value: any): boolean {
  // we don't check for string here so it also works with arrays
  return value == null || value.length === 0;
}
