FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NetBoilerplate.Web/NetBoilerplate.Web.csproj", "src/NetBoilerplate.Web/"]
COPY ["src/NetBoilerplate.Application/NetBoilerplate.Application.csproj", "src/NetBoilerplate.Application/"]
COPY ["src/NetBoilerplate.Application.Dto/NetBoilerplate.Application.Dto.csproj", "src/NetBoilerplate.Application.Dto/"]
COPY ["src/NetBoilerplate.Domain/NetBoilerplate.Domain.csproj", "src/NetBoilerplate.Domain/"]
COPY ["src/NetBoilerplate.Infrastructure/NetBoilerplate.Infrastructure.csproj", "src/NetBoilerplate.Infrastructure/"]
COPY ["src/NetBoilerplate.Shared/NetBoilerplate.Shared.csproj", "src/NetBoilerplate.Shared/"]
COPY ["src/NetBoilerplate.Migrator/NetBoilerplate.Migrator.csproj", "src/NetBoilerplate.Migrator/"]
RUN dotnet restore "./src/NetBoilerplate.Web/NetBoilerplate.Web.csproj"
COPY . .
WORKDIR "/src/src/NetBoilerplate.Web"
RUN dotnet build "./NetBoilerplate.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NetBoilerplate.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetBoilerplate.Web.dll"]
