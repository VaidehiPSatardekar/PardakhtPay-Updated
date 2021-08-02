import { Injectable } from '@angular/core';
import { TimeZone } from '../../../models/timeZone';

@Injectable({
  providedIn: 'root'
})
export class TimeZoneService {
    zones: TimeZone[] = [];

    constructor() {

        var zone = new TimeZone();
        zone.timeZoneId = 'Iran Standard Time';
        zone.translate = 'TIMEZONE.IRAN';

        this.zones.push(zone);

        var zoneUtc = new TimeZone();

        zoneUtc.timeZoneId = 'UTC';
        zoneUtc.translate = 'TIMEZONE.UTC';

        this.zones.push(zoneUtc);
    }

    getTimeZones() {

        return this.zones;
    }

    setTimeZone(zone: TimeZone) {
        localStorage.setItem('timeZone', JSON.stringify(zone));
    }

    getTimeZone(): TimeZone {
        try {
            var timeZoneStr = localStorage.getItem('timeZone');

            if (timeZoneStr == undefined || timeZoneStr == null || timeZoneStr == '') {
                var z = this.zones[0];
                this.setTimeZone(z);
                return z;
            }
            var zone = JSON.parse(timeZoneStr) as TimeZone;

            return zone;
        } catch (error) {
            console.log(error);
            var zone = this.zones[0];
            this.setTimeZone(zone);

            return zone;
        }
    }
}
