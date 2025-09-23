export type Primitive = string | number | boolean | null | undefined;

export interface PageQuery {
  page?: number;          // 1-based
  size?: number;          // tamaño de página
  search?: string | null; // texto libre
  sort?: string | null;   // campo permitido por el backend (whitelist)
  desc?: boolean;         // true = DESC
  filters?: Record<string, Primitive | Primitive[]>;
}

export interface PageResult<T> {
  items: T[];
  total: number;
  page: number;
  size: number;
  totalPages: number;
}
