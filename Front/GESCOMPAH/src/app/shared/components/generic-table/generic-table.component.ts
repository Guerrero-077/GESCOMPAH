import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
  TemplateRef,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { TableColumn } from '../../models/TableColumn.models';
import { HasRoleAndPermissionDirective } from '../../../core/security/directives/HasRoleAndPermission.directive';
import { StandardButtonComponent } from '../standard-button/standard-button.component';

export type RowDetailContext<T> = { $implicit: T; row: T };

@Component({
  selector: 'app-generic-table',
  standalone: true,
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
    MatTooltipModule,
    FormsModule,
    HasRoleAndPermissionDirective,
    StandardButtonComponent
  ],
  templateUrl: './generic-table.component.html',
  styleUrls: ['./generic-table.component.css']
})
export class GenericTableComponent<T> implements OnInit, AfterViewInit, OnChanges, OnDestroy {
  @Input() data: readonly T[] | T[] | null = null;
  @Input() columns: TableColumn<T>[] = [];

  @Input() createButtonLabel = '+ Crear';
  @Input() titulo = 'Tabla Genérica';
  @Input() subTitulo = 'Subtítulo de la tabla';

  @Input() showViewButton = true;
  @Input() showDetailButton = true;
  @Input() showActionsColumn = true;

  @Input() showFilterButton = true;
  @Input() filterParams: any = {};
  @Input() filterTooltip: string = 'Filtros';
  @Output() filterClick = new EventEmitter<any>();

  @Output() edit = new EventEmitter<T>();
  @Output() delete = new EventEmitter<T>();
  @Output() view = new EventEmitter<T>();
  @Output() create = new EventEmitter<void>();

  @Input() detailTemplate?: TemplateRef<RowDetailContext<T>>;
  @Input() expandableRows = true;
  expandedRow: T | null = null;

  displayedColumns: string[] = [];
  dataSource = new MatTableDataSource<T>();

  filterKey = '';
  private filterSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  private _paginator?: MatPaginator;
  private _sort?: MatSort;

  @ViewChild(MatPaginator) set paginator(p: MatPaginator | undefined) {
    this._paginator = p;
    this.connectPaginator();
  }

  @ViewChild(MatSort) set sort(s: MatSort | undefined) {
    this._sort = s;
    this.connectSort();
  }

  constructor(private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.updateDisplayedColumns();
    this.dataSource.data = this.data ? [...this.data] as T[] : [];

    this.filterSubject
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(value => {
        this.dataSource.filter = value.trim().toLowerCase();
        if (this._paginator) this._paginator.firstPage();
      });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['columns'] || changes['showActionsColumn']) {
      this.updateDisplayedColumns();
    }

    if (changes['data']) {
      this.dataSource.data = this.data ? [...this.data] as T[] : [];

      if (this._paginator) {
        const pageSize = this._paginator.pageSize || 5;
        const maxPageIndex = Math.max(0, Math.ceil(this.dataSource.data.length / pageSize) - 1);
        if (this._paginator.pageIndex > maxPageIndex) this._paginator.pageIndex = 0;
        this._paginator.length = this.dataSource.data.length;
      }

      this.connectSort();
      this.connectPaginator();

      try { (this.dataSource as any)._updateChangeSubscription(); } catch { }

      this.cdr.detectChanges();
    }
  }

  ngAfterViewInit() {
    this.connectSort();
    this.connectPaginator();
  }

  private updateDisplayedColumns(): void {
    this.displayedColumns = this.columns
      .filter(col => col.key?.toString().toLowerCase() !== 'id')
      .map(col => col.key.toString());

    if (this.showActionsColumn) {
      this.displayedColumns.push('actions');
    }
  }

  private connectPaginator() {
    if (this._paginator) {
      this.dataSource.paginator = this._paginator;
      this._paginator.length = this.dataSource.data.length;
    }
  }

  private connectSort() {
    if (this._sort) this.dataSource.sort = this._sort;
  }

  onFilterChange(value: string) { this.filterSubject.next(value); }
  onFilterClick() { this.filterClick.emit(this.filterParams); }

  get hasData(): boolean { return (this.dataSource?.data?.length || 0) > 0; }

  onEdit(row: T) { this.edit.emit(row); }
  onDelete(row: T) { this.delete.emit(row); }

  onView(row: T) {
    if (this.expandableRows && this.detailTemplate) {
      this.expandedRow = (this.expandedRow === row) ? null : row;
      return;
    }
    this.view.emit(row);
  }

  onCreate() { this.create.emit(); }

  isExpanded(row: T) { return this.expandedRow === row; }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.filterSubject.complete();
  }
}
