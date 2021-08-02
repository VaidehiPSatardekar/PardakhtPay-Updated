import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { TimeZoneService } from '../services/timeZoneService/time-zone.service';

@Pipe({
    name: 'localizedDate',
    pure: false
})
export class LocalizedDatePipe implements PipeTransform {

    constructor(private translateService: TranslateService,
        private timeZoneService: TimeZoneService) {
    }

    transform(value: any, pattern: string = 'short'): any {
        let localeId = '';
        var zone = this.timeZoneService.getTimeZone();

        if(!value){
            return '';
        }

        if (zone.timeZoneId == 'Iran Standard Time') {
            var parts = value.split('T');
            if(parts.length != 2){
                return '';
            }

            var dateParts = parts[0].split('-');

            if(dateParts.length != 3){
                return '';
            }

            return dateParts[2] + '/' + dateParts[1] + '/' + dateParts[0] + ' ' + parts[1];
        }
        else {
            switch (this.translateService.currentLang) {
                case 'fa':
                    localeId = 'fa';
                    pattern = 'dd/MM/yyyy HH:mm';
                    break;
                case 'en':
                    localeId = 'en-GB'; // doesn't seem to work
                    pattern = 'dd/MM/yyyy HH:mm'; // workaround to format GB dates
                    break;
                default:
                    localeId = 'fa';
                    pattern = 'dd/MM/yyyy HH:mm';
                    break;
            }
            const datePipe: DatePipe = new DatePipe(localeId);

            // const transformedValue = datePipe.transform(value, pattern);
            // console.log(transformedValue);
            // return transformedValue;
            return datePipe.transform(value, pattern);
        }
    }

}
