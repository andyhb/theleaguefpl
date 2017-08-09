import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'formatDate' })
export class FormatDatePipe implements PipeTransform {
    transform(date: Date): string {
        var dateFormat = require('dateformat');

        return dateFormat(date, "HH:MM dd/mm/yyyy");
    }
}