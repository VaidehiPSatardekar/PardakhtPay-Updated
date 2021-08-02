import { ValidatorFn, AbstractControl, ValidationErrors } from "@angular/forms";
import { GenericHelper } from "../helpers/generic";

export const iranianPhoneNumberValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {

    if (!control.parent || !control) {
        return null;
    }

    let value = control.value;

    if (!GenericHelper.isNullOrUndefinedOrEmpty(value)) {

        if (GenericHelper.validateIranianPhoneNumber(value)) {
            return null;
        }
    }

    return { 'iranianPhoneNumberCouldNotBeenValidated': true };
};