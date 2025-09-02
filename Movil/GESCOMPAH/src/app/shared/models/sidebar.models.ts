export interface SidebarChild {
  label: string;
  icon?: string;
  route: string;          // <- obligatorio
}

export interface SidebarItem {
  label: string;
  icon: string;
  route?: string;         // solo en ítems directos
  children: SidebarChild[]; // <- SIEMPRE array (vacío para ítem directo)
}


export interface BackendSubMenuItem {
  id: number;
  name: string;
  description: string;
  route: string;
  permissions: string[];
}

export interface BackendMenuItem {
  id: number;
  name: string;
  description: string;
  icon: string;
  forms: BackendSubMenuItem[];
}
