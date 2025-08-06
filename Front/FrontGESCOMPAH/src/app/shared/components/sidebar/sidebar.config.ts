export interface SidebarItem {
  label: string;
  icon: string;
  route: string;
}

export const SIDEBAR_BY_ROLE: Record<string, SidebarItem[]> = {
  admin: [
    { label: 'Dashboard', icon: 'home', route: 'dashboard' },
    { label: 'Propiedades', icon: 'description', route: 'main' },
    { label: 'Arrendatarios', icon: 'person', route: 'tenants' },
    { label: 'Contratos', icon: 'assignment', route: 'contratos' },
    { label: 'Citas', icon: 'event', route: 'citas' },
    { label: 'Reportes', icon: 'bar_chart', route: 'reportes' },
    { label: 'SistemSegurity', icon: 'security', route: 'mainSistemSegurity' },
    { label: 'Configuración', icon: 'settings', route: 'configuracion' }
  ],
  arrendamiento: [
    { label: 'Dashboard', icon: 'home', route: '/dashboard' },
    { label: 'Citas', icon: 'event', route: '/citas' }
  ]
};
