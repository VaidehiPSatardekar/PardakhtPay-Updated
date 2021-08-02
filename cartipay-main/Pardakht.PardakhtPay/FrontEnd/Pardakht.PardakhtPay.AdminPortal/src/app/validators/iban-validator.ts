import { ValidatorFn, AbstractControl, ValidationErrors } from "@angular/forms";
import { GenericHelper } from "../helpers/generic";

export const ibanValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {

    if ( !control.parent || !control )
    {
        return null;
    }

    let value = control.value;

    if (GenericHelper.validateIban(value)) {
        return null;
    }

    return {'ibanCouldNotBeenValidated': true};
};