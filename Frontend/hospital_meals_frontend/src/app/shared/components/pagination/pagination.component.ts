import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss',
})
export class PaginationComponent {
  page = input.required<number>();
  pageSize = input.required<number>();
  totalCount = input.required<number>();
  pageSizeOptions = input<number[]>([25, 50, 100, 250, 500]);

  pageChange = output<number>();
  pageSizeChange = output<number>();

  get totalPages(): number {
    const total = this.totalCount();
    const size = this.pageSize();
    if (total === 0 || size <= 0) return 0;
    return Math.ceil(total / size);
  }

  get startItem(): number {
    const total = this.totalCount();
    if (total === 0) return 0;
    return (this.page() - 1) * this.pageSize() + 1;
  }

  get endItem(): number {
    return Math.min(this.page() * this.pageSize(), this.totalCount());
  }

  get hasPrev(): boolean {
    return this.page() > 1;
  }

  get hasNext(): boolean {
    return this.page() < this.totalPages;
  }

  goPrev(): void {
    if (this.hasPrev) this.pageChange.emit(this.page() - 1);
  }

  goNext(): void {
    if (this.hasNext) this.pageChange.emit(this.page() + 1);
  }

  onPageSizeChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    const size = parseInt(value, 10);
    if (!Number.isNaN(size)) this.pageSizeChange.emit(size);
  }
}
