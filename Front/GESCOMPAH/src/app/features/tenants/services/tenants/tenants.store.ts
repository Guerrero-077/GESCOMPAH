import { Injectable, signal, computed, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { TenantsService } from './tenants.service';
import {
  TenantsCreateModel,
  TenantsSelectModel,
  TenantsUpdateModel
} from '../../models/tenants.models';

@Injectable({ providedIn: 'root' })
export class TenantStore {
  private readonly svc = inject(TenantsService);

  // Estado base
  private readonly _items   = signal<TenantsSelectModel[]>([]);
  private readonly _loading = signal(false);
  private readonly _error   = signal<string | null>(null);

  // Concurrencia por ítem (p. ej., toggle)
  private readonly _busyIds = signal<Set<number>>(new Set());

  // Selectores
  readonly items   = computed(() => this._items());
  readonly loading = computed(() => this._loading());
  readonly error   = computed(() => this._error());
  readonly count   = computed(() => this._items().length);
  readonly busyIds = computed(() => this._busyIds());

  isBusy(id: number): boolean {
    return this._busyIds().has(id);
  }

  // Helpers de colección
  setAll(list: TenantsSelectModel[]): void {
    this._items.set(list ?? []);
  }

  /** Inserta/actualiza sin reordenar el array. */
  upsertMany(list: TenantsSelectModel[]): void {
    if (!list?.length) return;
    const byId = new Map(list.map(it => [it.id, it]));
    this._items.update(arr => {
      const next = arr.map(it => byId.get(it.id) ?? it);
      // agrega los que no estaban
      byId.forEach((it, id) => {
        if (!arr.some(x => x.id === id)) next.unshift(it);
      });
      return next;
    });
  }

  upsertOne(item: TenantsSelectModel): void {
    this._items.update(arr => {
      const i = arr.findIndex(x => x.id === item.id);
      if (i === -1) return [item, ...arr];
      const copy = arr.slice();
      copy[i] = item;
      return copy;
    });
  }

  patchOne(id: number, patch: Partial<TenantsSelectModel>): void {
    this._items.update(arr => arr.map(x => x.id === id ? { ...x, ...patch } : x));
  }

  remove(id: number): void {
    this._items.update(arr => arr.filter(x => x.id !== id));
  }

  clear(): void {
    this._items.set([]);
    this._error.set(null);
    this._loading.set(false);
    this._busyIds.set(new Set());
  }

  changeActiveStatus(id: number, active: boolean): void {
    this.patchOne(id, { active } as Partial<TenantsSelectModel>);
  }

  getById(id: number): TenantsSelectModel | undefined {
    return this._items().find(x => x.id === id);
  }

  private markBusy(id: number, val: boolean): void {
    this._busyIds.update(set => {
      const next = new Set(set);
      val ? next.add(id) : next.delete(id);
      return next;
    });
  }

  private setError(e: unknown): void {
    const status = (e as any)?.status ?? (e as any)?.statusCode;
    const type = (e as any)?.type;
    if (status === 401 || type === 'Unauthorized' || (e as any)?.__authExpired) return;
    this._error.set(String((e as any)?.message ?? e ?? 'Error'));
  }

  // I/O
  async loadAll(): Promise<void> {
    this._loading.set(true);
    this._error.set(null);
    try {
      const data = await firstValueFrom(this.svc.getAll());
      this.setAll(data ?? []);
    } catch (e) {
      this.setError(e);
    } finally {
      this._loading.set(false);
    }
  }

  async create(dto: TenantsCreateModel): Promise<void> {
    try {
      const created = await firstValueFrom(this.svc.create(dto));
      this.upsertOne(created);
    } catch (e) {
      this.setError(e);
      throw e;
    }
  }

  async update(id: number, dto: TenantsUpdateModel): Promise<void> {
    try {
      const updated = await firstValueFrom(this.svc.update(id, dto));
      this.upsertOne(updated);
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

  // Borrado lógico: eliminar de la vista. Si prefieres mantenerlo, usar changeActiveStatus(..., false).
  async deleteLogic(id: number): Promise<void> {
    try {
      await firstValueFrom(this.svc.deleteLogic(id));
      this.remove(id);
    } catch (e) {
      this.setError(e);
      throw e;
    }
  }

  /** Cambia estado remoto (optimista + rollback + busy por id). */
  async changeActiveStatusRemote(id: number, active: boolean): Promise<void> {
    if (this.isBusy(id)) return; // evita doble clic
    const prev = this.getById(id)?.active; // true/false/undefined
    this.markBusy(id, true);
    this.changeActiveStatus(id, active); // optimista

    try {
      const updated = await firstValueFrom(this.svc.changeActiveStatus(id, active));
      if (updated) this.upsertOne(updated);
    } catch (err) {
      if (prev !== undefined) this.changeActiveStatus(id, prev); // rollback
      this.setError(err);
      throw err;
    } finally {
      this.markBusy(id, false);
    }
  }
}
