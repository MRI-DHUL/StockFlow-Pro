import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'dateFormatter',
  standalone: true
})
export class DateFormatterPipe implements PipeTransform {
  constructor(private datePipe: DatePipe) {}

  transform(value: Date | string | null | undefined, format: string = 'medium'): string {
    if (!value) return '-';
    return this.datePipe.transform(value, format) || '-';
  }
}
