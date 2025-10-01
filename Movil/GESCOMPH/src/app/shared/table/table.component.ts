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
  SimpleChanges 
} from '@angular/core';

import { IonSearchbar, IonContent, IonButton, IonList, IonCard, IonCardHeader, IonCardTitle, IonCardSubtitle, IonCardContent, IonRow, IonIcon, IonGrid, IonCol } from "@ionic/angular/standalone";


import { CommonModule } from '@angular/common';
import { TableColumn } from '../models/TableColumn.models';
import { debounceTime, distinctUntilChanged, Subject, takeUntil } from 'rxjs';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-table',
  standalone: true,
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss'],
  imports: [
    CommonModule,
    FormsModule,
    IonSearchbar,
    IonContent,
    IonButton,
    IonList,
    IonCard,
    IonCardHeader,
    IonCardTitle,
    IonCardSubtitle,
    IonCardContent,
    IonRow,
    IonIcon,
    IonGrid,
    IonCol
],
})
export class TableComponent<T> implements OnInit, AfterViewInit, OnChanges, OnDestroy {
  
  @Input() data: T[] | null = null;
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

  @Output() download = new EventEmitter<T>();
  @Output() view = new EventEmitter<T>();

  displayedColumns: string[] = [];
  dataSource: T[] = [];

  filterKey = '';
  private filterSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.updateDisplayedColumns();
    this.dataSource = this.data || [];

    this.filterSubject
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(value => {
        this.dataSource = (this.data || []).filter(item =>
          JSON.stringify(item).toLowerCase().includes(value.trim().toLowerCase())
        );
        this.cdr.detectChanges();
      });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['columns'] || changes['showActionsColumn']) {
      this.updateDisplayedColumns();
    }

    if (changes['data']) {
      this.dataSource = this.data || [];
      this.cdr.detectChanges();
    }
  }

  ngAfterViewInit() {}

  private updateDisplayedColumns(): void {
    this.displayedColumns = this.columns.map(col => col.key.toString());
    if (this.showActionsColumn) {
      this.displayedColumns.push('actions');
    }
  }

  onFilterChange(value: string) {
    this.filterSubject.next(value);
  }

  onFilterClick() {
    this.filterClick.emit(this.filterParams);
  }

  get hasData(): boolean {
    return (this.dataSource?.length || 0) > 0;
  }

  onDownload(row: T) {
    this.download.emit(row);
  }

  onView(row: T) {
    this.view.emit(row);
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.filterSubject.complete();
  }

  getValue(row: T, key: string | keyof T | null | undefined) {
  if (!key) return '';
  return row[key as keyof T];
}


}
