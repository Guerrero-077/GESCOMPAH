import { Injectable, signal, computed, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import {
  ContractCard,
  ContractSelectModel,
  ContractCreateModel,
} from '../../models/contract.models';
import { ContractService } from './contract.service';

// Tipos utilitarios
export interface StoreError {
  status?: number;
  code?: string;
  message: string;
}

type UpsertMode = 'append' | 'prepend';

@Injectable({ providedIn: 'root' })
export class ContractStore {
  private readonly svc = inject(ContractService);

  // Estado base
  private readonly _items = signal<ReadonlyArray<ContractCard>>([]);
  private readonly _index = signal<ReadonlyMap<number, number>>(new Map());
  private readonly _loading = signal(false);
  private readonly _error = signal<StoreError | null>(null);
  private readonly _busyIds = signal<ReadonlySet<number>>(new Set());

  // Para cancelar/ignorar respuestas viejas
  private _lastRequestId = 0;

  // Selectores
  readonly items = computed(() => this._items());
  readonly loading = computed(() => this._loading());
  readonly error = computed(() => this._error());
  readonly count = computed(() => this._items().length);
  readonly busyIds = computed(() => this._busyIds());

  // Alias
  readonly rows = this.items;

  isBusy(id: number): boolean {
    return this._busyIds().has(id);
  }

  getById(id: number): ContractCard | undefined {
    const i = this._index().get(id);
    return i !== undefined ? this._items()[i] : undefined;
  }

  // Helpers de colección
  private rebuildIndex(list: ReadonlyArray<ContractCard>): void {
    const map = new Map<number, number>();
    for (let i = 0; i < list.length; i++) map.set(list[i].id, i);
    this._index.set(map);
  }

  setAll(list: ContractCard[] | null | undefined): void {
    const arr = (list ?? []).slice(); // copia defensiva
    this._items.set(arr);
    this.rebuildIndex(arr);
  }

  upsertMany(list: ContractCard[], mode: UpsertMode = 'append'): void {
    if (!list?.length) return;

    this._items.update(curr => {
      const index = this._index();
      const next = curr.slice();
      const fresh: ContractCard[] = [];

      for (const it of list) {
        const pos = index.get(it.id);
        if (pos !== undefined) {
          next[pos] = it; // reemplaza en el mismo lugar
        } else {
          fresh.push(it);
        }
      }
      const merged = mode === 'prepend' ? [...fresh, ...next] : [...next, ...fresh];
      this.rebuildIndex(merged);
      return merged;
    });
  }

  upsertOne(item: ContractCard): void {
    this._items.update(curr => {
      const index = this._index();
      const pos = index.get(item.id);
      if (pos === undefined) {
        const merged = [item, ...curr];
        this.rebuildIndex(merged);
        return merged;
      }
      const copy = curr.slice();
      copy[pos] = item;
      this.rebuildIndex(copy);
      return copy;
    });
  }

  patchOne(id: number, patch: Partial<ContractCard>): void {
    this._items.update(curr => {
      const index = this._index();
      const pos = index.get(id);
      if (pos === undefined) return curr;
      const copy = curr.slice();
      copy[pos] = { ...copy[pos], ...patch };
      this.rebuildIndex(copy);
      return copy;
    });
  }

  patchActiveMany(ids: number[], value: boolean): void {
    if (!ids?.length) return;

    this._items.update(curr => {
      const index = this._index();
      const next = curr.slice();
      let touched = false;

      for (const id of ids) {
        const pos = index.get(id);
        if (pos !== undefined) {
          const row = next[pos];
          if (row.active !== value) {
            next[pos] = { ...row, active: value };
            touched = true;
          }
        }
      }

      if (!touched) return curr;      // nada cambió → no alterar referencia
      this.rebuildIndex(next);
      return next;
    });
  }
  remove(id: number): void {
    this._items.update(curr => {
      const filtered = curr.filter(x => x.id !== id);
      if (filtered.length === curr.length) return curr;
      this.rebuildIndex(filtered);
      return filtered;
    });
  }

  clear(): void {
    this._items.set([]);
    this._index.set(new Map());
    this._error.set(null);
    this._loading.set(false);
    this._busyIds.set(new Set());
  }

  changeActiveStatusLocal(id: number, active: boolean): void {
    this.patchOne(id, { active } as Partial<ContractCard>);
  }

  // Internos
  private markBusy(id: number, val: boolean): void {
    this._busyIds.update(curr => {
      const next = new Set(curr);
      val ? next.add(id) : next.delete(id);
      return next;
    });
  }

  private setError(e: unknown): void {
    // Ignora errores de sesión expirada (401) porque el authInterceptor ya navega a login
    const status = (e as any)?.status ?? (e as any)?.statusCode;
    const type = (e as any)?.type;
    if (status === 401 || type === 'Unauthorized' || (e as any)?.__authExpired) return;

    const m = (e as any)?.message ?? (typeof e === 'string' ? e : 'Error inesperado');
    const code = (e as any)?.error?.code ?? (e as any)?.code;
    this._error.set({ status, code, message: String(m) });
  }

  // I/O
  async loadAll(options: { force?: boolean } = {}): Promise<void> {
    const requestId = ++this._lastRequestId;
    this._loading.set(true);
    this._error.set(null);
    try {
      const data = await firstValueFrom(this.svc.getAll(options));
      // Ignora respuestas viejas
      if (requestId !== this._lastRequestId) return;
      this.setAll(data ?? []);
    } catch (e) {
      if (requestId !== this._lastRequestId) return;
      this.setError(e);
    } finally {
      if (requestId === this._lastRequestId) this._loading.set(false);
    }
  }

  async create(dto: ContractCreateModel): Promise<number> {
    try {
      const id = await firstValueFrom(this.svc.create(dto));
      // Opcional: cargar detalle y upsertOne aquí si la API no lo devuelve
      return id;
    } catch (e) {
      this.setError(e);
      throw e;
    }
  }

  async update(id: number, dto: Partial<ContractCreateModel>): Promise<void> {
    try {
      const updated = await firstValueFrom(this.svc.update(id, dto));
      this.patchFromDetail(updated);
    } catch (e) {
      this.setError(e);
      throw e;
    }
  }

  async delete(id: number): Promise<void> {
    try {
      await firstValueFrom(this.svc.delete(id));
      this.remove(id);
    } catch (e) {
      this.setError(e);
      throw e;
    }
  }

  async deleteLogic(id: number): Promise<void> {
    try {
      await firstValueFrom(this.svc.deleteLogic(id));
      this.remove(id);
    } catch (e) {
      this.setError(e);
      throw e;
    }
  }

  /** Toggle remoto (optimista + rollback + busy por fila). */
  async changeActiveStatusRemote(id: number, active: boolean): Promise<void> {
    if (this.isBusy(id)) return;
    const prev = this.getById(id)?.active;

    this.markBusy(id, true);
    this.changeActiveStatusLocal(id, active);

    try {
      await firstValueFrom(this.svc.changeActiveStatus(id, active));
    } catch (e) {
      if (prev !== undefined) this.changeActiveStatusLocal(id, prev);
      this.setError(e);
      throw e;
    } finally {
      this.markBusy(id, false);
    }
  }

  /** Sincroniza campos clave provenientes del detalle. */
  patchFromDetail(updated: ContractSelectModel): void {
    this.patchOne(updated.id, {
      startDate: updated.startDate,
      endDate: updated.endDate,
      active: updated.active,
      // Agrega aquí otros campos “card” que provengan del detalle
    } as Partial<ContractCard>);
  }
}
