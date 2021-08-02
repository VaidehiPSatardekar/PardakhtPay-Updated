// tslint:disable:function-name
// tslint:disable:no-for-in

export class GenericHelper {

    static populateData(into: any, data?: any): void {
        if (data) {
            for (const key in data) {
                if (data.hasOwnProperty(key)) {
                    into[key] = data[key];
                }
            }
        }
    }

    static detectChanges(origin: any, data?: any): boolean {
        if (data) {
            for (const key in origin) {
                if (origin[key] != data[key]) {
                    return true;
                }
            }
        }

        return false;
    }

    static detectNonNullableChanges(origin: any, data?: any): boolean {
        if (data) {
            for (const key in origin) {
                if (GenericHelper.isNullOrUndefinedOrEmpty(origin[key]) && GenericHelper.isNullOrUndefinedOrEmpty(data[key])) {
                    continue;
                }
                else {
                    if (origin[key] != data[key]) {
                        // console.log(origin[key]) ;
                        // console.log(data[key]) ;   
                        // console.log(key) ;   
                        return true;
                    }
                }
            }
        }

        return false;
    }

    static isNullOrUndefinedOrEmpty(data: any): boolean {
        return data === undefined || data === null || data === '';
    }


    // tslint:disable-next-line:typedef
    static enumGetNamesAndValues<T extends number>(e: any) {
        return this.enumGetNames(e).map(n => ({ name: n, value: e[n] as T }));
    }

    // tslint:disable-next-line:typedef
    static enumGetNames(e: any) {
        // tslint:disable-next-line:prefer-type-cast
        return this.enumGetObjValues(e).filter(v => typeof v === 'string') as string[];
    }

    // tslint:disable-next-line:typedef
    static enumGetValues<T extends number>(e: any) {
        // tslint:disable-next-line:prefer-type-cast
        return this.enumGetObjValues(e).filter(v => typeof v === 'number') as T[];
    }

    // tslint:disable-next-line:typedef
    static enumGetObjValues(e: any): (number | string)[] {
        return Object.keys(e).map(k => e[k]);
    }

    static mod(divident, divisor) {
        var partLength = 10;

        while (divident.length > partLength) {
            var part = divident.substring(0, partLength);
            divident = (part % divisor) + divident.substring(partLength);
        }

        return divident % divisor;
    }

    static validateIban(iban: string): boolean {
        if (GenericHelper.isNullOrUndefinedOrEmpty(iban)) {
            return false;
        }

        if (!iban.toLowerCase().startsWith('ir')) {
            return false;
        }

        let numbers = iban.slice(2, iban.length);

        if (numbers.length != 24 || !new RegExp('(\\d){24}').test(numbers)) {
            return false;
        }
        let testIban = numbers.substr(3, 21) + "1827" + numbers.substr(0, 3);
        let testNumber = Number(testIban);
        console.log(testIban);
        console.log(testNumber);
        let modValue = this.mod(testIban, 97);
        console.log(modValue);
        return modValue === 10;
    }

    static extractBankCode(iban: string): string {
        if (GenericHelper.isNullOrUndefinedOrEmpty(iban)) {
            return '';
        }

        var str = iban.toLowerCase().replace('ir', '');

        return str.substr(2, 3);
    }

    static validateCardNumber(cardNumber: string): boolean {
        if (cardNumber == undefined || cardNumber == null || cardNumber == '' || cardNumber.length != 16 || !(new RegExp('(\\d){16}').test(cardNumber))) {
            return false;
        }

        var sum = 0;

        for (var i = 0; i < cardNumber.length; i++) {
            var c = cardNumber[i];
            var d = parseInt(c) * (i % 2 == 0 ? 2 : 1);
            sum += d > 9 ? d - 9 : d;
        }

        if (sum % 10 == 0) {
            return true;
        }

        if (cardNumber.startsWith("505801")) {
            return true;
        }

        return false;
    }

    static validateIranianPhoneNumber(phoneNumber: string): boolean {
        var regex = '^[0][9][0-9]{9,9}$';

        if (phoneNumber == undefined || phoneNumber == null || !(new RegExp(regex).test(phoneNumber))) {

            return false;
        }

        return true;
    }
}
