# ETAPA 1: SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Rutas corregidas para el contexto de la raíz
COPY ["CC.Api/CC.Api.csproj", "CC.Api/"]
COPY ["CC.Application/CC.Application.csproj", "CC.Application/"]
COPY ["CC.Domain/CC.Domain.csproj", "CC.Domain/"]
COPY ["CC.Infrastructure/CC.Infrastructure.csproj", "CC.Infrastructure/"]
COPY ["CC.Utilities/CC.Utilities.csproj", "CC.Utilities/"]

RUN dotnet restore "CC.Api/CC.Api.csproj"

# Copiamos todo el contenido de la solución
COPY . . 

WORKDIR "/src/CC.Api"
RUN dotnet build "CC.Api.csproj" -c Release -o /app/build

# ETAPA 2: Publicar
FROM build AS publish
RUN dotnet publish "CC.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ETAPA 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CC.Api.dll"]