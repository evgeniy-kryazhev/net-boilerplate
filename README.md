# NetBoilerplate

ASP.NET Core Web API template with Identity, role/permission management, EF Core + SQL Server, Minio storage, Serilog, Scalar/OpenAPI, and localization.

## Quick start

```bash
dotnet new install NetBoilerplate.Api
dotnet new netboilerplate -n MyProject -o ./MyProject
cd ./MyProject
dotnet run --project src/MyProject.Web/MyProject.Web.csproj
```

Open `https://localhost:5013/scalar` for the API reference.

## Project structure

```
src/
├── MyProject.Web              # ASP.NET Core API entrypoint
├── MyProject.Application      # Application services & DI
├── MyProject.Application.Dto  # API DTO contracts
├── MyProject.Domain           # Identity entities & abstractions
├── MyProject.Infrastructure   # EF Core, Identity stores, Minio
├── MyProject.Migrator         # DB migration & seeding host
└── MyProject.Shared           # Base entities, repositories, localization
```

## Features

- **ASP.NET Core Identity** — bearer token auth with user/role management
- **Role & permission system** — granular claim-based permissions
- **EF Core + SQL Server** — with code-first migrations
- **Minio file storage** — S3-compatible object storage
- **Serilog** — structured logging to console
- **Scalar / OpenAPI** — interactive API docs at `/scalar`
- **Localization** — JSON-based i18n (en/ru)
- **SMTP** — email confirmation, password reset, test endpoint
- **dotnet new template** — ready to publish as NuGet package

## Default credentials

| Role  | Email            | Password |
|-------|------------------|----------|
| Admin | admin@mail.ru    | 1q2w3E*  |

## Publish to NuGet

Push a version tag to trigger the CI:

```bash
git tag v1.0.0
git push origin v1.0.0
```

Or run manually via GitHub Actions → `Publish NuGet Package` → `Run workflow`.

## NuGet packages

Install the published template:

```bash
dotnet new install NetBoilerplate.Api
```

## License

MIT
