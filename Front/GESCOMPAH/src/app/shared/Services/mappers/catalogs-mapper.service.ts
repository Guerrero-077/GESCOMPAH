import { Injectable } from '@angular/core';
import { CitySelectModel } from '../../../features/setting/models/city.models';

/**
 * Mappers “tontos” de respuestas a listas de {id, name}.
 * Evita acoplar shared -> features (no importa el tipo concreto).
 */
@Injectable({ providedIn: 'root' })
export class CatalogsMapperService {
  /**
   * Normaliza distintas formas de respuesta a una lista { id, name }[]
   * Acepta estructuras:
   * - res (array)
   * - res.data, res.items, res.result (array)
   * Y campos:
   * - id | cityId | value
   * - name | cityName | label
   */
  toCityList(res: any): CitySelectModel[] {
    const raw = Array.isArray(res)
      ? res
      : Array.isArray(res?.data)   ? res.data
      : Array.isArray(res?.items)  ? res.items
      : Array.isArray(res?.result) ? res.result
      : [];

    return raw
      .map((c: any) => ({
        id: Number(c?.id ?? c?.cityId ?? c?.value),
        name: String(c?.name ?? c?.cityName ?? c?.label ?? '').trim(),
        departmentName: String(c?.departmentName ?? c?.deptName ?? '').trim(),
        active: c?.active !== undefined ? Boolean(c.active) : true
      }))
      .filter((c: CitySelectModel) => !!c.id && !!c.name);
  }

  /** Ejemplo adicional (por si lo necesitas en el futuro) */
  toRoleList(res: any): Array<{ id: number; name: string }> {
    const raw = Array.isArray(res)
      ? res
      : Array.isArray(res?.data)   ? res.data
      : Array.isArray(res?.items)  ? res.items
      : Array.isArray(res?.result) ? res.result
      : [];

    return raw
      .map((r: any) => ({
        id: Number(r?.id ?? r?.roleId ?? r?.value),
        name: String(r?.name ?? r?.roleName ?? r?.label ?? '').trim(),
      }))
      .filter((r: any) => !!r.id && !!r.name);
  }
}
