FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar solo el archivo .csproj y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todos los demás archivos, incluyendo el modelo ML.NET
COPY . ./
RUN dotnet publish -c Release -o out

# Generar la imagen de tiempo de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Establecer la variable de entorno
ENV ASPNETCORE_ENVIRONMENT Development

ENV APP_NET_CORE Proyecto.dll

CMD ASPNETCORE_URLS=http://*:$PORT dotnet $APP_NET_CORE