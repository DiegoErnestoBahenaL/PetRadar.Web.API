# Etapa base (runtime) - ARM64
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-arm64v8 AS base
WORKDIR /app
EXPOSE 5000

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy-arm64v8 AS build
WORKDIR /src

# Copia solo el csproj para restaurar primero
COPY ["PetRadar.Web.API/PetRadar.Web.API.csproj", "PetRadar.Web.API/"]
RUN dotnet restore "PetRadar.Web.API/PetRadar.Web.API.csproj"

# Copia todo y compila
COPY . .
WORKDIR "/src/PetRadar.Web.API"
RUN dotnet publish "PetRadar.Web.API.csproj" -c Release -o /app/publish

# Imagen final, pequeña
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Escucha en 5000 dentro del contenedor
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PetRadar.Web.API.dll"]
