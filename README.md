# CleanAdmin

> A CleanDDD-based full-stack admin starter built with .NET 10, Aspire, FastEndpoints, and Blazor.

English | [简体中文](README.zh-CN.md)

[![.NET](https://img.shields.io/badge/.NET-10.0-blueviolet)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-13.x-512BD4)](https://learn.microsoft.com/dotnet/aspire/)
[![License](https://img.shields.io/badge/license-MIT-green)](./LICENSE)

CleanAdmin is an open-source template for building modern admin systems with clear domain boundaries and production-friendly defaults. It includes backend API, frontend UI, infrastructure orchestration, migration service, and test projects in one solution.

## Why CleanAdmin

- CleanDDD style project structure (Domain, Infrastructure, API, Web)
- .NET Aspire orchestration for local distributed development
- FastEndpoints + FluentValidation + Swagger for API productivity
- Built-in Redis, RabbitMQ, PostgreSQL integration
- CAP, Hangfire, OpenTelemetry, Prometheus support out of the box
- Blazor-based frontend with Ant Design ecosystem and Tailwind CSS

## Architecture

The solution includes:

- `CleanAdmin.AppHost`: Aspire AppHost (orchestration entry point)
- `CleanAdmin.ApiService`: backend API service
- `CleanAdmin.Web`: Blazor web host and reverse proxy entry
- `CleanAdmin.Web.Client`: frontend client project
- `CleanAdmin.MigrationService`: database migration and seed service
- `CleanAdmin.Domain`: domain model and business rules
- `CleanAdmin.Infrastructure`: EF Core, repository, integration details
- `CleanAdmin.ServiceDefaults`: shared telemetry/health/service defaults

## Tech Stack

- .NET SDK `10.0.100` (see `global.json`)
- ASP.NET Core + Blazor
- FastEndpoints
- Entity Framework Core + PostgreSQL
- Redis + RabbitMQ
- CAP + Hangfire
- OpenTelemetry + Prometheus
- Aspire

## Quick Start

### 1) Prerequisites

- .NET SDK 10.0.100 (preview allowed)
- Docker Desktop
- Node.js 18+

### 2) Restore dependencies

```bash
dotnet restore
```

### 3) Build frontend assets

```bash
cd src/CleanAdmin.Web
npm install
npm run build:css
```

### 4) Run the full stack with Aspire

```bash
dotnet run --project src/CleanAdmin.AppHost
```

AppHost will orchestrate PostgreSQL, Redis, RabbitMQ, migration service, API, and Web frontend.

## Local Development

Run API only:

```bash
dotnet run --project src/CleanAdmin.ApiService
```

Run Web only:

```bash
dotnet run --project src/CleanAdmin.Web
```

Watch frontend CSS during development:

```bash
cd src/CleanAdmin.Web
npm run watch:css
```

## Common Endpoints

- Swagger (dev): `/swagger`
- Health check: `/health`
- Liveness: `/alive`
- Metrics (Prometheus): `/metrics`
- CAP dashboard: `/cap`
- Hangfire dashboard: `/hangfire`
- Code analysis visualization: `/code-analysis`

## Configuration

- Default local app settings are in each project's `appsettings*.json`
- Aspire parameters are configured in `src/CleanAdmin.AppHost/appsettings.json`
- Prefer environment variables or user secrets for sensitive values

Example:

```bash
dotnet user-secrets set "Auth:Jwt:TokenSigningKey" "your-long-random-signing-key" --project src/CleanAdmin.ApiService
dotnet user-secrets set "Auth:ApiKey" "your-api-key" --project src/CleanAdmin.ApiService
```

## Database Migration

`CleanAdmin.MigrationService` runs migrations when started by AppHost.

If you need EF CLI manually:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add <MigrationName> -p src/CleanAdmin.Infrastructure
dotnet ef database update -p src/CleanAdmin.Infrastructure
```

## Testing

Run all tests:

```bash
dotnet test
```

## IDE Snippets

This repository includes productivity snippets under `vs-snippets` and `.vscode/csharp.code-snippets`.

Read more: `vs-snippets/README.md`

## Related Projects

- [NetCorePal Cloud Framework](https://github.com/netcorepal/netcorepal-cloud-framework)
- [ASP.NET Core](https://github.com/dotnet/aspnetcore)
- [Entity Framework Core](https://github.com/dotnet/efcore)
- [CAP](https://github.com/dotnetcore/CAP)
- [FastEndpoints](https://fast-endpoints.com/)

## Contributing

Contributions are welcome.

1. Fork this repository
2. Create your feature branch (`git checkout -b feat/your-feature`)
3. Commit your changes (`git commit -m "feat: add ..."`)
4. Push to your branch and open a Pull Request

Please make sure build and tests pass before submitting.

## License

MIT License. See `LICENSE` for details.
