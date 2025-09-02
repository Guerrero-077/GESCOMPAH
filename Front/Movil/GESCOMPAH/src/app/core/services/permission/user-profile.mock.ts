export const USER_PROFILE_MOCK = {
  "id": 1,
  "fullName": "Administrador General",
  "email": "admin@gescomph.com",
  "roles": [
    "Administrador"
  ],
  "permissions": [
    "Ver",
    "Crear",
    "Editar",
    "Eliminar"
  ],
  "menu": [
    {
      "id": 1,
      "name": "Dashboard",
      "description": "Panel de control principal",
      "icon": "home",
      "forms": [
        {
          "id": 1,
          "name": "Vista General",
          "description": "Vista general del dashboard",
          "route": "dashboard",
          "permissions": [
            "Ver",
            "Crear",
            "Editar",
            "Eliminar"
          ]
        }
      ]
    },
    {
      "id": 2,
      "name": "Establecimientos",
      "description": "Gestión de establecimientos",
      "icon": "store",
      "forms": [
        {
          "id": 2,
          "name": "Lista de Establecimientos",
          "description": "Ver y gestionar establecimientos",
          "route": "establishments/main",
          "permissions": [
            "Ver",
            "Crear",
            "Editar",
            "Eliminar"
          ]
        }
      ]
    },
    {
      "id": 3,
      "name": "Arrendatarios",
      "description": "Gestión de arrendatarios",
      "icon": "people",
      "forms": [
        {
          "id": 3,
          "name": "Arrendatarios",
          "description": "Gestión de arrendatarios",
          "route": "tenants",
          "permissions": [
            "Ver",
            "Crear",
            "Editar",
            "Eliminar"
          ]
        }
      ]
    },
    {
      "id": 4,
      "name": "Seguridad",
      "description": "Gestión de seguridad y permisos",
      "icon": "security",
      "forms": [
        {
          "id": 4,
          "name": "Modelos de Seguridad",
          "description": "Configuración de modelos de seguridad",
          "route": "security/main",
          "permissions": [
            "Ver",
            "Crear",
            "Editar",
            "Eliminar"
          ]
        }
      ]
    },
    {
      "id": 5,
      "name": "Configuración",
      "description": "Configuración general del sistema",
      "icon": "settings",
      "forms": [
        {
          "id": 5,
          "name": "Configuración Principal",
          "description": "Ajustes principales del sistema",
          "route": "settings/main",
          "permissions": [
            "Ver",
            "Crear",
            "Editar",
            "Eliminar"
          ]
        }
      ]
    }
  ]
}