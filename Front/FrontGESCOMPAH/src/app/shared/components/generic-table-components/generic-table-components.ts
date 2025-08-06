import { AfterViewInit, Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { TableColumn } from '../../models/TableColumn.models';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-generic-table-components',
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatTooltipModule
  ],
  templateUrl: './generic-table-components.html',
  styleUrl: './generic-table-components.css'
})
export class GenericTableComponents<T> implements AfterViewInit {
  @Input() data: T[] = [];
  @Input() columns: TableColumn<T>[] = [];

  @Output() edit = new EventEmitter<T>();
  @Output() delete = new EventEmitter<T>();
  @Output() view = new EventEmitter<T>();

  displayedColumns: string[] = [];

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  dataSource = new MatTableDataSource<T>();

  ngOnChanges() {
    this.dataSource.data = this.data;
    this.displayedColumns = this.columns.map(col => col.key.toString()).concat('actions');
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  applyFilter(event: Event) {
    const target = event.target as HTMLInputElement;
    if (target?.value) {
      this.dataSource.filter = target.value.trim().toLowerCase();
    }
  }


  onEdit(row: T) { this.edit.emit(row); }
  onDelete(row: T) { this.delete.emit(row); }
  onView(row: T) { this.view.emit(row); }
}