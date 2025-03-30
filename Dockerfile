# Imagen base para ejecución con dependencias necesarias
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Solucionar problemas con librerías faltantes
RUN ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so

# Instalar dependencias para System.Drawing
RUN apt-get update && apt-get install -y --allow-unauthenticated libgdiplus libc6-dev libx11-dev
RUN ln -s libgdiplus.so gdiplus.dll

# Configurar directorio de trabajo
WORKDIR /app

# Configurar variables de entorno
ENV ASPNETCORE_URLS=http://+:3000
ENV DOCUMENT_SLIDING_EXPIRATION_TIME="10"
ENV REDIS_CACHE_CONNECTION_STRING=""
ENV DOCUMENT_PATH=""

# Exponer puertos HTTP
EXPOSE 3000

# Imagen para compilación del proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Establecer directorio de trabajo
WORKDIR /src

# Copiar y restaurar dependencias
COPY ["pdf-viewer.csproj", "./"]


# Instalar dependencias
RUN dotnet restore "./pdf-viewer.csproj"

# Verificar dependencias instaladas
RUN dotnet list "./pdf-viewer.csproj" package

# Copiar el código fuente
COPY . .

# Construir el proyecto
RUN dotnet build -c Release -o /app

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish -c Release -o /app

# Imagen final con el código publicado
FROM base AS final

# Definir directorio de trabajo
WORKDIR /app

# Copiar archivos de la etapa de publicación
COPY --from=publish /app .

# Definir el punto de entrada
ENTRYPOINT ["dotnet", "/app/pdf-viewer.dll"]
