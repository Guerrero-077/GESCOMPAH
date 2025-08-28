import { Injectable, signal, computed } from '@angular/core';
import { ContractSelectModel } from '../../models/contract.models';

@Injectable({ providedIn: 'root' })
export class ContractStore {
  
  private _contracts = signal<ContractSelectModel[]>([]);

  // Señal pública reactiva
  readonly contracts = computed(() => this._contracts());

  setContracts(contracts: ContractSelectModel[]): void {
    this._contracts.set(contracts);
  }

  addContract(contract: ContractSelectModel): void {
    this._contracts.update(contracts => [...contracts, contract]);
  }

  updateContract(updatedContract: ContractSelectModel): void {
    this._contracts.update(contracts =>
      contracts.map(contract =>
        contract.id === updatedContract.id ? updatedContract : contract
      )
    );
  }

  deleteContract(id: number): void {
    this._contracts.update(contracts =>
      contracts.filter(contract => contract.id !== id)
    );
  }

  clear(): void {
    this._contracts.set([]);
  }

  get snapshot(): ContractSelectModel[] {
    return this._contracts();
  }
}
