import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
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
    FormsModule
  ],
  templateUrl: './generic-table.component.html',
  styleUrls: ['./generic-table.component.css']
})
export class GenericTableComponent<T> implements OnInit, AfterViewInit, OnChanges, OnDestroy {
  @Input() data: T[] | null = null;
  @Input() columns: TableColumn<T>[] = [];
  @Input() createButtonLabel = '+ Crear';
  @Input() titulo = 'Tabla Genérica';
  @Input() subTitulo = 'Subtítulo de la tabla';

  /** 👉 Botón de filtros (opcional y configurable) */
  @Input() showFilterButton = true;                 // Mostrar/ocultar botón
  @Input() filterParams: any = {};                  // Parámetros que provee el padre
  @Input() filterTooltip: string = 'Filtros';       // Tooltip del botón
  @Output() filterClick = new EventEmitter<any>();  // Evento al hacer clic en filtros

  @Output() edit = new EventEmitter<T>();
  @Output() delete = new EventEmitter<T>();
  @Output() view = new EventEmitter<T>();
  @Output() create = new EventEmitter<void>();

  displayedColumns: string[] = [];
  dataSource = new MatTableDataSource<T>();

  filterKey = '';
  private filterSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  // Guardamos referencias internas y las conectamos mediante setters
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
    this.displayedColumns = this.columns.map(col => col.key.toString()).concat('actions');
    this.dataSource.data = this.data || [];

    this.filterSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(value => {
      this.dataSource.filter = value.trim().toLowerCase();
      // cuando filtras, volver a la primera página
      if (this._paginator) { this._paginator.firstPage(); }
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['columns']) {
      this.displayedColumns = this.columns.map(col => col.key.toString()).concat('actions');
    }

    if (changes['data']) {
      // asignar datos
      this.dataSource.data = this.data || [];

      // si el paginator ya está, actualizar su length y pageIndex si es necesario
      if (this._paginator) {
        const pageSize = this._paginator.pageSize || 5;
        const maxPageIndex = Math.max(0, Math.ceil(this.dataSource.data.length / pageSize) - 1);
        if (this._paginator.pageIndex > maxPageIndex) {
          this._paginator.pageIndex = 0;
        }
        this._paginator.length = this.dataSource.data.length;
      }

      // reconectar sort/paginator por si se crearon después
      this.connectSort();
      this.connectPaginator();

      // forzar que MatTableDataSource recalcule
      try { (this.dataSource as any)._updateChangeSubscription(); } catch { }

      // forzar CD para que Angular reevalúe la vista (importante cuando aparece paginator después)
      this.cdr.detectChanges();
    }
  }

  ngAfterViewInit() {
    // componentes presentes al iniciar la vista se conectarán por setters,
    // pero por seguridad conectamos aquí también:
    this.connectSort();
    this.connectPaginator();
  }

  private connectPaginator() {
    if (this._paginator) {
      this.dataSource.paginator = this._paginator;
      this._paginator.length = this.dataSource.data.length;
    }
  }

  private connectSort() {
    if (this._sort) {
      this.dataSource.sort = this._sort;
    }
  }

  onFilterChange(value: string) {
    this.filterSubject.next(value);
  }

  /** 👉 Click en el botón de filtros: emite al padre con los params actuales */
  onFilterClick() {
    this.filterClick.emit(this.filterParams);
  }

  get hasData(): boolean {
    return (this.dataSource?.data?.length || 0) > 0;
  }

  onEdit(row: T) {
    console.log('GenericTableComponent: Emitting edit event with row:', row);
    this.edit.emit(row);
  }
  onDelete(row: T) {
    console.log('GenericTableComponent: Emitting delete event with row:', row);
    this.delete.emit(row);
  }
  onView(row: T) { this.view.emit(row); }
  onCreate() { this.create.emit(); }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.filterSubject.complete();
  }
}
