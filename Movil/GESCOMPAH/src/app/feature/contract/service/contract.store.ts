import { Injectable, signal, computed } from '@angular/core';
import { ContractCard, ContractSelectModel } from '../models/contract.models';

@Injectable({ providedIn: 'root' })
export class ContractStore {
  // Única fuente de verdad para la grilla
  private _rows = signal<ContractCard[]>([]);

  // Exposición reactiva
  readonly rows = computed(() => this._rows());

  // Setters
  setRows(list: ContractCard[]): void { this._rows.set(list ?? []); }
  clear(): void { this._rows.set([]); }

  // Mutadores
  deleteRow(id: number): void {
    this._rows.update(arr => arr.filter(c => c.id !== id));
  }

  updateRowActive(id: number, active: boolean): void {
    this._rows.update(arr => arr.map(c => c.id === id ? ({ ...c, active }) : c));
  }

  /** Sincroniza algunas propiedades de la fila desde un update de detalle */
  patchFromDetail(updated: ContractSelectModel): void {
    this._rows.update(arr =>
      arr.map(c => c.id === updated.id
        ? { ...c, startDate: updated.startDate, endDate: updated.endDate, active: updated.active }
        : c)
    );
  }
}
