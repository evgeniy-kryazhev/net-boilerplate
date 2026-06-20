# AGENTS.md

Compact guidance for future OpenCode sessions in this repository.

## Project Shape

- This is a **dotnet new template** (`netboilerplate`). When generated, `NetBoilerplate` is replaced by the project name in all namespaces, files, and content.
- `Directory.Build.props` sets `net10.0`, nullable, implicit usings globally.
- `src/{ProjectName}.Web` is the ASP.NET Core API entrypoint. `Program.cs` wires Serilog, localization, EF/Identity, Minio storage, bearer auth, Scalar/OpenAPI, and endpoint groups.
- `src/{ProjectName}.Application` contains services and DI in `ApplicationHostExtensions.AddExCraftApplication`.
- `src/{ProjectName}.Application.Dto` holds API DTO contracts. Do not return EF entities from new API endpoints when a DTO pattern exists.
- `src/{ProjectName}.Domain` holds Identity entities and abstractions.
- `src/{ProjectName}.Infrastructure` owns EF Core, Identity stores, repositories, Minio storage, `ApplicationDbContext`, and migrations.
- `src/{ProjectName}.Shared` contains repository abstractions, base entities, paging DTOs, localization helpers, and common exceptions/extensions.
- `src/{ProjectName}.Migrator` is a console host that creates the database if needed, applies EF migrations, then runs seeders.
- There is no test project in the solution at the time of writing.

## Commands

Run from the repository root:

```powershell
dotnet restore {ProjectName}.slnx
dotnet build {ProjectName}.slnx
dotnet run --project src/{ProjectName}.Web/{ProjectName}.Web.csproj
dotnet run --project src/{ProjectName}.Migrator/{ProjectName}.Migrator.csproj
```

- Use `dotnet build {ProjectName}.slnx` as the normal verification step after code changes.
- `dotnet test {ProjectName}.slnx` currently has no tests to run unless a test project is added later.

## Database And Config

- EF is configured with `UseSqlServer` in both runtime DI and the design-time context factory.
- `DB_URL` overrides the configured connection string for both the web app and migrator.
- The migrator seeds identity data. Current credentials: `admin@mail.ru` / `1q2w3E*`.
- Add EF migrations from infrastructure project root:
  ```powershell
  dotnet ef migrations add <Name> `
    --project src/{ProjectName}.Infrastructure/{ProjectName}.Infrastructure.csproj `
    --startup-project src/{ProjectName}.Migrator/{ProjectName}.Migrator.csproj
  ```

## API Patterns

- Keep endpoint mapping in `{ProjectName}.Web/Endpoints`; each group is mapped in `Program.cs`.
- Generic list/get endpoints use `MapEndpoints<TEntity, TDto>` plus `IBasicService<TEntity, TDto>` and permission names from `ApplicationPermissions`.
- For a new CRUD-style entity, follow: domain entity → repository registration (infrastructure) → DTO (Application.Dto) → service (Application.Services) → `IBasicService<TEntity,TDto>` registration → endpoint group (Web/Endpoints).
- Permission-protected endpoints use `.RequirePermission(...)`.
- `/` redirects to `/scalar`; OpenAPI via `app.MapOpenApi()` and Scalar via `app.MapScalarApiReference()`.

## Template

```powershell
# Install from local path
dotnet new install D:\projects\NetBoilerplate

# Generate a new project
dotnet new netboilerplate -n MyProject -o .\MyProject
```

- `sourceName`: `NetBoilerplate` — replaced by `-n` value in all namespaces, filenames, and content.
- Excluded from template: `bin/`, `obj/`, `.vs/`, `*.DotSettings.user`, `test/`, `qodana.yaml`.
