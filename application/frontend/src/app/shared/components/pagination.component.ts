import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule, MatPaginatorModule],
  template: `
    <mat-paginator
      [length]="totalCount"
      [pageSize]="pageSize"
      [pageIndex]="pageNumber - 1"
      [pageSizeOptions]="pageSizeOptions"
      (page)="onPageChange($event)"
      showFirstLastButtons>
    </mat-paginator>
  `,
  styles: [`
    :host {
      display: block;
      margin-top: 16px;
    }
  `]
})
export class PaginationComponent {
  @Input() totalCount = 0;
  @Input() pageSize = 10;
  @Input() pageNumber = 1;
  @Input() pageSizeOptions = [5, 10, 25, 50, 100];
  
  @Output() pageChange = new EventEmitter<{ pageNumber: number; pageSize: number }>();

  onPageChange(event: PageEvent): void {
    this.pageChange.emit({
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize
    });
  }
}
