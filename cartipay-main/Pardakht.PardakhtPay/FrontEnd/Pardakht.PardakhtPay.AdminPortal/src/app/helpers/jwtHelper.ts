// tslint:disable:triple-equals
// tslint:disable:one-variable-per-declaration
// tslint:disable:no-increment-decrement
// tslint:disable:no-conditional-assignment
// tslint:disable:no-bitwise

export class JwtHelper {

  isTokenExpired(token: string, offsetSeconds?: number): boolean {
      const date: Date = this.getTokenExpirationDate(token);
    offsetSeconds = offsetSeconds || 0;

    if (date == null) {
      return false;
    }

    // Token expired?
    return !(date.valueOf() > (new Date().valueOf() + (offsetSeconds * 1000)));
  }

  private urlBase64Decode(str: string): string {
    let output: string = str.replace(/-/g, '+').replace(/_/g, '/');
    switch (output.length % 4) {
      case 0: { break; }
      case 2: { output += '=='; break; }
      case 3: { output += '='; break; }
      default: {
        throw new Error('Illegal base64url string!');
      }
    }

    return this.b64DecodeUnicode(output);
  }

  // credits for decoder goes to https://github.com/atk
  private b64decode(str: string): string {
    const chars: string = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=';
    let output: string = '';

    str = String(str).replace(/=+$/, '');

    if (str.length % 4 == 1) {
      throw new Error("'atob' failed: The string to be decoded is not correctly encoded.");
    }

    for (
      // initialize result and counters
      let bc: number = 0, bs: any, buffer: any, idx: number = 0;
      // get next character

      buffer = str.charAt(idx++);
      // character found in table? initialize bit storage and add its ascii value;

      ~buffer && (bs = bc % 4 ? bs * 64 + buffer : buffer,
        // and if not first of each 4 characters,
        // convert the first 8 bits to one ascii character
        bc++ % 4) ? output += String.fromCharCode(255 & bs >> (-2 * bc & 6)) : 0
    ) {
      // try to find character in table (0-63, not found => -1)
      buffer = chars.indexOf(buffer);
    }

    return output;
  }

  // https://developer.mozilla.org/en/docs/Web/API/WindowBase64/Base64_encoding_and_decoding#The_Unicode_Problem
  private b64DecodeUnicode(str: string): string {
    return decodeURIComponent(Array.prototype.map.call(this.b64decode(str), (c: any) => {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
  }

    public decodeToken(token: string): any {
        try {
            const parts: string[] = token.split('.');

            if (parts.length !== 3) {
                throw new Error('JWT must have 3 parts');
            }

            const decoded: string = this.urlBase64Decode(parts[1]);
            if (!decoded) {
                throw new Error('Cannot decode the token');
            }

            return JSON.parse(decoded);
        }
        catch{
            localStorage.removeItem('token');
            return null;
        }
  }

  public getTokenExpirationDate(token: string): Date {
    let decoded: any;
    decoded = this.decodeToken(token);

    if (!decoded.hasOwnProperty('exp')) {
      return null;
    }

    const date: Date = new Date(0); // The 0 here is the key, which sets the date to the epoch
    date.setUTCSeconds(decoded.exp);

    return date;
  }

    public getTenantGuid(token: string): string {

        //return undefined;
        let decoded: any;
        decoded = this.decodeToken(token);

        if (!decoded.hasOwnProperty('http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata')) {
            return undefined;
        }

        var item = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata'];

        if (item != null && item != undefined) {
            var guid = JSON.parse(item)['TenantGuid'];

            if (guid == null || guid == '') {
                return undefined;
            }

            return guid;
        }

        return undefined;
    }

    public getParentAccountId(token: string): string {

        let decoded: any;
        decoded = this.decodeToken(token);

        if (!decoded.hasOwnProperty('http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata')) {
            return undefined;
        }

        var item = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata'];

        if (item != null && item != undefined) {
            var guid = JSON.parse(item)['ParentAccountId'];

            if (guid == null || guid == '') {
                return undefined;
            }

            return guid;
        }

        return undefined;
    }

    public getAccountGuid(token: string): string {
        let decoded: any;
        decoded = this.decodeToken(token);
        if (!decoded.hasOwnProperty('nameid')) {
            return undefined;
        }
        var item = decoded['nameid'];

        return item;
    }

    public getUsername(token: string) {
        let decoded: any;
        decoded = this.decodeToken(token);
        if (!decoded.hasOwnProperty('unique_name')) {
            return undefined;
        }
        var item = decoded['unique_name'];

        return item;
    }
}
