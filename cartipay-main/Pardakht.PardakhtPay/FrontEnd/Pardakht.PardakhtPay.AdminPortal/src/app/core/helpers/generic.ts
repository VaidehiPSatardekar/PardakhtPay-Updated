// tslint:disable:function-name
// tslint:disable:no-for-in

// @dynamic
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

  static insertSpaces(string: string) {
    if(string){
      string = string.replace(/([a-z])([A-Z])/g, '$1 $2');
      string = string.replace(/([A-Z])([A-Z][a-z])/g, '$1 $2')
      return string;
    }
    return '';
  }
}
