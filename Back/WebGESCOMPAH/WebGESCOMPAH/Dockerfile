# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
## Runtime con Playwright preinstalado (navegadores + deps)
## Usa una imagen alineada con Microsoft.Playwright 1.54.0
FROM mcr.microsoft.com/playwright/dotnet:v1.54.0-jammy AS base
WORKDIR /app
EXPOSE 8080

# (Opcional) usuario sin privilegios provisto por la imagen de Playwright
# USER pwuser


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY Back/WebGESCOMPAH/WebGESCOMPAH/WebGESCOMPAH.csproj WebGESCOMPAH/
COPY Back/WebGESCOMPAH/Business/Business.csproj Business/
COPY Back/WebGESCOMPAH/Entity/Entity.csproj Entity/
COPY Back/WebGESCOMPAH/Data/Data.csproj Data/
COPY Back/WebGESCOMPAH/Utilities/Utilities.csproj Utilities/
COPY Back/WebGESCOMPAH/Templates/Templates.csproj Templates/
COPY Back/WebGESCOMPAH/GESCOMPAH.sln .


RUN dotnet restore "./WebGESCOMPAH/WebGESCOMPAH.csproj"
# Copiar todo el código
COPY Back/WebGESCOMPAH/ .




WORKDIR "/src/WebGESCOMPAH"
RUN dotnet build "./WebGESCOMPAH.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebGESCOMPAH.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebGESCOMPAH.dll"]
