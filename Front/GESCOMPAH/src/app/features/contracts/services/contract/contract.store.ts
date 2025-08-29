import { Injectable, signal, computed } from '@angular/core';
import { ContractSelectModel, ContractCard } from '../../models/contract.models';

@Injectable({ providedIn: 'root' })
export class ContractStore {
  private _contracts = signal<ContractSelectModel[]>([]); // read-model completo
  private _cards     = signal<ContractCard[]>([]);        // read-model “mine”

  readonly contracts = computed(() => this._contracts());
  readonly cards     = computed(() => this._cards());

  // Setters
  setContracts(list: ContractSelectModel[]): void { this._contracts.set(list ?? []); }
  setCards(list: ContractCard[]): void { this._cards.set(list ?? []); }

  // Mutadores
  addContract(contract: ContractSelectModel): void {
    this._contracts.update(arr => [...arr, contract]);
  }

  updateContract(updated: ContractSelectModel): void {
    this._contracts.update(arr => arr.map(c => c.id === updated.id ? updated : c));
  }

  deleteContract(id: number): void {
    this._contracts.update(arr => arr.filter(c => c.id !== id));
  }

  deleteCard(id: number): void {
    this._cards.update(arr => arr.filter(c => c.id !== id));
  }

  patchCardActive(id: number, active: boolean): void {
    this._cards.update(arr => arr.map(c => c.id === id ? ({ ...c, active }) : c));
  }

  /** Parchea algunos campos de la Card desde un update del modelo completo (si aplica). */
  patchCardIfDatesMatch(updated: ContractSelectModel): void {
    this._cards.update(arr =>
      arr.map(c => c.id === updated.id
        ? { ...c, startDate: updated.startDate, endDate: updated.endDate, active: updated.active }
        : c)
    );
  }

  clear(): void { this._contracts.set([]); this._cards.set([]); }

  // Snapshot del modelo completo (para update optimista)
  get snapshot(): ContractSelectModel[] { return this._contracts(); }
}
