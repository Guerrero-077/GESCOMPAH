// sidebar.config.ts
export interface BackendSubMenuItem {
  id: number;
  name: string;
  description: string;
  route: string;
  permissions: string[];
  parentId?: number; // <- NUEVO: padre dentro del mismo módulo (opcional)
}

export interface BackendMenuItem {
  id: number;
  name: string;
  description: string;
  icon: string;
  forms: BackendSubMenuItem[];
}

// Modelos internos del sidebar (árbol ya armado)
export interface SidebarNode {
  id: number;
  label: string;
  route?: string;           // leaf = tiene route
  icon?: string;
  children: SidebarNode[];  // siempre array (puede estar vacío)
}

export interface SidebarItem {
  label: string;
  icon: string;
  route?: string;           // si no tiene children, será link directo
  children: SidebarNode[];  // si tiene children, será acordeón
}
