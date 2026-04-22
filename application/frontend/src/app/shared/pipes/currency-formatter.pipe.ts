import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'currency',
  standalone: true
})
export class CurrencyFormatterPipe implements PipeTransform {
  transform(value: number | null | undefined, currencySymbol: string = '$'): string {
    if (value === null || value === undefined) {
      return `${currencySymbol}0.00`;
    }
    return `${currencySymbol}${value.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')}`;
  }
}
