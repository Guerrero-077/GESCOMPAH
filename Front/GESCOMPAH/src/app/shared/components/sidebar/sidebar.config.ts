export interface SidebarItem {
  label: string;
  icon: string;
  route?: string; // Make route optional
  children?: SidebarItem[]; // Add children property for nesting
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
