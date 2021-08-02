import { ValidatorFn, AbstractControl, ValidationErrors } from "@angular/forms";
import { GenericHelper } from "../helpers/generic";

export const manualTransferAmountValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {

    if (!control.parent || !control) {
        return null;
    }

    const transferWholeAmount = control.parent.get('transferWholeAmount');

    if (!transferWholeAmount) {
        return null;
    }

    if (transferWholeAmount.value == true) {

        return null;
    }

    let value = control.value;

    if (!GenericHelper.isNullOrUndefinedOrEmpty(value)) {
        var num = Number(value);

        if (num > 0) {
            return null;
        }
    }

    return { 'manualTransferAmountNotBeenValidated': true };
};