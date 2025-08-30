
Aplicación web para gestionar contratos de arrendamiento y procesos administrativos. 
El repositorio se divide en:

- **Back**: API REST creada con ASP.NET Core 8.
- **Front**: cliente generado con Angular CLI 20.

## Requisitos previos
Instala las siguientes herramientas antes de comenzar:

- [Git](https://git-scm.com/) para clonar el repositorio.
- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download) y la herramienta de migraciones de EF Core:
  ```bash
  dotnet tool install --global dotnet-ef
  ```
- [Node.js 18+](https://nodejs.org/) que incluye **npm**.
- Angular CLI 20 de forma global:
  ```bash
  npm install -g @angular/cli@20
  ```
- [Docker](https://www.docker.com/) para levantar SQL Server en un contenedor.

## Clonar el proyecto
```bash
git clone <URL_DEL_REPOSITORIO>
cd GESCOMPAH
```

## Base de datos SQL Server en Docker
1. Crear y ejecutar un contenedor SQL Server 2022:
   ```bash
   docker run --name gescomph-sql -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=<TU_PASSWORD>' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
   ```
2. Crear la base de datos `gescomph` dentro del contenedor:
   ```bash
   docker exec -it gescomph-sql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P <TU_PASSWORD> -Q "CREATE DATABASE gescomph;"
   ```

La cadena de conexión del backend se configura en `Back/WebGESCOMPAH/WebGESCOMPAH/appsettings.json`. Un ejemplo para desarrollo local sería:
```text
Server=localhost,1433;Database=gescomph;User Id=sa;Password=<TU_PASSWORD>;Encrypt=False;TrustServerCertificate=True
```
Ajusta los valores según tus credenciales y entorno.

## Backend (ASP.NET Core)
1. Restaurar dependencias y aplicar migraciones desde la carpeta del backend:
   ```bash
   cd Back/WebGESCOMPAH
   dotnet restore
   dotnet ef database update --project Entity --startup-project WebGESCOMPAH/WebGESCOMPAH
   ```
2. Ejecutar la API:
   ```bash
   dotnet run --project WebGESCOMPAH/WebGESCOMPAH
   ```
   La API quedará disponible en `http://localhost:5000` (o en el puerto configurado).

### Crear nuevas migraciones
Si cambias las entidades, genera una nueva migración y actualiza la base de datos desde la misma carpeta `Back/WebGESCOMPAH`:
```bash
dotnet ef migrations add NombreMigracion --project Entity --startup-project WebGESCOMPAH/WebGESCOMPAH
dotnet ef database update --project Entity --startup-project WebGESCOMPAH/WebGESCOMPAH
```

## Frontend (Angular)
1. Instalar dependencias:
   ```bash
   cd Front/GESCOMPAH
   npm install
   ```
2. Iniciar el servidor de desarrollo:
   ```bash
   npm start
   # o
   ng serve
   ```
   La aplicación estará disponible en `http://localhost:4200`.

## Notas
- Verifica que el backend permita el origen `http://localhost:4200` para evitar problemas de CORS.
- Ajusta la cadena de conexión en `appsettings.json` si tu instancia de SQL Server usa otros datos de acceso.