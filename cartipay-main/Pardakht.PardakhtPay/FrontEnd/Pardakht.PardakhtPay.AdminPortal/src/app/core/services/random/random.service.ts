import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class RandomService {

    constructor() { }

    generateRandomKey(length: number = 60): string {
        return this.generateId(length);
    }

    private dec2hex(dec): string {
        return ('0' + dec.toString(16)).substr(-2);
    }

    private generateId(len): string {
        var arr = new Uint8Array((len || 60) / 2)
        window.crypto.getRandomValues(arr)
        return Array.from(arr, this.dec2hex).join('')
    }
}
