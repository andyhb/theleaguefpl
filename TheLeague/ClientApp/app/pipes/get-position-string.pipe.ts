import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'getPositionString' })
export class GetPositionStringPipe implements PipeTransform {
    transform(position: number): string {
        if (position === 1) return "GK";
        if (position === 2) return "DEF";
        if (position === 3) return "MID";
        if (position === 4) return "FOR";

        return "";
    }
}