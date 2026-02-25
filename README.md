# CleanAdmin

> A CleanDDD-based full-stack admin template built with .NET 10, Aspire, and Blazor.

English | [简体中文](README.zh-CN.md)

[![.NET](https://img.shields.io/badge/.NET-10.0-blueviolet?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-13.x-512BD4)](https://learn.microsoft.com/dotnet/aspire/)
[![License](https://img.shields.io/badge/license-MIT-green)](./LICENSE)

CleanAdmin is an open-source admin template based on .NET 10, designed for lightweight, fast, and maintainable
enterprise web application development. It integrates FastEndpoints, Aspire, and Scalar to provide a modern API
development and documentation experience.

CleanAdmin focuses on clear architecture and simple implementation. In a single solution, it combines backend APIs,
frontend UI, infrastructure orchestration, migration services, and test projects. It also supports Microsoft.Kiota to
auto-generate strongly typed API clients, reducing integration and maintenance costs while improving team delivery
efficiency.

## 👀 Project Preview

- Live preview: https://cleanadmin.azurewebsites.net
- Note: The preview site is currently powered by mock data generated with Apifox.

## 🌟 Why Choose CleanAdmin

Compared with traditional admin templates, CleanAdmin puts stronger emphasis on maintainability and a modern developer
experience:

- **Built on netcorepal-cloud-framework**: Implements DDD tactical patterns and includes core architecture capabilities
  such as CQRS, event-driven workflows, distributed transactions (eventual consistency), multi-tenancy,
  multi-environment deployment (including canary), and database sharding.
- **Aspire cloud-native zero-config development environment**: One command can orchestrate API, Web, database, and
  middleware services without complex local setup.
- **Minimal API style with FastEndpoints**: Uses REPR and vertical slices instead of traditional controllers for higher
  cohesion and better execution efficiency.
- **Modern API docs with Scalar**: Provides a modern API document and debugging UI for browsing, integration, and team
  collaboration.
- **Kiota strongly typed API clients**: Auto-generates strongly typed API clients from OpenAPI to reduce development
  overhead.
- **Blazor full-stack development**: Uses a unified C# stack across frontend and backend, and provides a
  backend-friendly frontend development experience based on Ant Design Blazor and Tailwind CSS.

## 🏗️ Architecture

The solution includes the following projects:

- `CleanAdmin.AppHost`: Aspire AppHost (unified orchestration entry)
- `CleanAdmin.ApiService`: backend API service
- `CleanAdmin.Web`: Blazor Web host and reverse proxy entry
- `CleanAdmin.Web.Client`: frontend client project
- `CleanAdmin.MigrationService`: database migration service
- `CleanAdmin.Domain`: domain model and business rules
- `CleanAdmin.Infrastructure`: EF Core, repository, and integration implementations
- `CleanAdmin.ServiceDefaults`: unified observability, health checks, and service defaults

## 🧰 Tech Stack

- .NET SDK `10.0.100` (see `global.json`)
- ASP.NET Core + Blazor
- FastEndpoints
- Entity Framework Core + PostgreSQL
- Redis + RabbitMQ
- CAP + Hangfire
- OpenTelemetry + Prometheus
- Aspire

## 🚀 Quick Start

### 1) ✅ Prerequisites

- .NET SDK 10.0.100
- Docker Desktop
- Node.js 18+

### 2) 📦 Restore dependencies

```bash
dotnet restore
```

### 3) ▶️ Run the full stack with Aspire

```bash
dotnet run --project src/CleanAdmin.AppHost
```

AppHost will orchestrate PostgreSQL, Redis, RabbitMQ, MigrationService, ApiService, and the Web frontend.

## 🛠️ Backend Development Notes

For backend development conventions, refer to the guideline files under `.github/instructions` (such as
`endpoint.instructions.md`, `command.instructions.md`, `query.instructions.md`, and `aggregate.instructions.md`).

## 💻 Local Development

During development, build ApiService in Release mode to generate API clients:

```bash
dotnet build src/CleanAdmin.ApiService -c Release
```

Watch CSS for hot reload during development:

```bash
cd src/CleanAdmin.Web
npm run watch:css
```

## 🔗 Common Endpoints

- Scalar (development): `/scalar`
- Health check: `/health`
- Liveness: `/alive`
- Prometheus metrics: `/metrics`
- CAP dashboard: `/cap`
- Hangfire dashboard: `/hangfire`
- Code analysis visualization: `/code-analysis`

## ⚙️ Configuration

- Default local settings are in each project's `appsettings*.json`
- Aspire parameters are in `src/CleanAdmin.AppHost/appsettings.json`
- Use environment variables or user secrets for sensitive values

Example:

```bash
dotnet user-secrets set "Auth:Jwt:TokenSigningKey" "your-long-random-signing-key" --project src/CleanAdmin.ApiService
dotnet user-secrets set "Auth:ApiKey" "your-api-key" --project src/CleanAdmin.ApiService
```

## 🗄️ Database Migration

When starting with AppHost, `CleanAdmin.MigrationService` runs migrations automatically.

If you need EF CLI manually:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add <MigrationName> -p src/CleanAdmin.MigrationService
dotnet ef database update -p src/CleanAdmin.MigrationService
```

## 🧪 Testing

Run all tests:

```bash
dotnet test
```

## ✨ IDE Snippets

This repository includes productivity snippets under `vs-snippets` and `.vscode/csharp.code-snippets`.

Read more: `vs-snippets/README.md`

## 📚 Related Projects

- [NetCorePal Cloud Framework](https://github.com/netcorepal/netcorepal-cloud-framework)
- [Ant Design Blazor](https://github.com/ant-design-blazor/ant-design-blazor)
- [FastEndpoints](https://fast-endpoints.com/)

## 🤝 Contributing

Contributions are welcome in many forms. Code submissions, suggestions, and issue reports are all appreciated.

## 📄 License

MIT License. See `LICENSE` for details.
