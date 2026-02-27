# AGENTS.md — CleanAdmin

## Project Overview

Full-stack .NET 10 admin template using Clean Architecture + DDD + CQRS.
Backend: ASP.NET Core with FastEndpoints. Frontend: Blazor WebAssembly with Ant Design Blazor + Tailwind CSS 4.
Orchestration: .NET Aspire. Database: PostgreSQL. Cache: Redis. Queue: RabbitMQ.
DDD framework: NetCorePal Cloud Framework. Mediator: MediatR. Validation: FluentValidation.

## Build & Run

```bash
# Prerequisites: .NET SDK 10.0.100, Node.js 18+, Docker Desktop

# Restore and build
dotnet restore
dotnet build CleanAdmin.slnx

# Install frontend deps (required before first build of Web project)
npm install                          # run in src/CleanAdmin.Web/

# Run full stack via Aspire (starts PostgreSQL, Redis, RabbitMQ, API, Web)
dotnet run --project src/CleanAdmin.AppHost

# Build CSS manually
npm run build:css                    # run in src/CleanAdmin.Web/
npm run watch:css                    # watch mode
```

## Testing

Test stack: **xUnit v3**, **Shouldly** (assertions), **Moq** (mocking), **FastEndpoints.Testing** + **Testcontainers** (integration). Docker Desktop must be running for integration tests.

```bash
# Run all tests
dotnet test

# Run a single test project
dotnet test test/CleanAdmin.Domain.Tests
dotnet test test/CleanAdmin.ApiService.Tests

# Run a single test class
dotnet test --filter "FullyQualifiedName~OrderTests"

# Run a single test method
dotnet test --filter "FullyQualifiedName=CleanAdmin.Domain.Tests.OrderTests.Order_Init_Test"
```

## Linting & Analysis

No standalone lint command. Analyzers run during `dotnet build`:
- **SonarAnalyzer.CSharp** on all non-test projects
- **NetCorePal.Extensions.CodeAnalysis** on Domain and ApiService projects
- **Nullable reference types** enabled; null-safety warnings (`CS8600`–`CS8625`) are **errors**

## Architecture & Layering

```
CleanAdmin.ApiService  -->  CleanAdmin.Infrastructure  -->  CleanAdmin.Domain
     (API + App layer)          (EF Core, Repos)            (Aggregates, Events)
```

Strict one-way dependency: **ApiService -> Infrastructure -> Domain**. Never reference upstream.

## File Organization

| Concept                  | Location                                                    |
|--------------------------|-------------------------------------------------------------|
| Aggregate roots/entities | `src/CleanAdmin.Domain/AggregatesModel/{Aggregate}/`        |
| Domain events            | `src/CleanAdmin.Domain/DomainEvents/`                       |
| Repositories             | `src/CleanAdmin.Infrastructure/Repositories/`               |
| Entity configurations    | `src/CleanAdmin.Infrastructure/EntityConfigurations/`       |
| Commands + handlers      | `src/CleanAdmin.ApiService/Application/Commands/{Feature}/` |
| Queries + handlers       | `src/CleanAdmin.ApiService/Application/Queries/{Feature}/`  |
| API endpoints            | `src/CleanAdmin.ApiService/Endpoints/{Feature}/`            |
| Domain event handlers    | `src/CleanAdmin.ApiService/Application/DomainEventHandlers/`|
| Unit tests               | `test/CleanAdmin.Domain.Tests/`                             |
| Integration tests        | `test/CleanAdmin.ApiService.Tests/`                         |

**Co-location rule**: Command + Validator + Handler go in one file. Endpoint + Request + Response + Validator + Summary go in one file. Repository interface + implementation go in one file.

## Naming Conventions

| Element            | Convention                          | Example                                      |
|--------------------|-------------------------------------|----------------------------------------------|
| Classes            | PascalCase                          | `User`, `CreateUserCommand`                  |
| Interfaces         | `I` + PascalCase                    | `IUserRepository`, `ICurrentUser`            |
| Private fields     | `_camelCase`                        | `_mediator`                                  |
| Local vars/params  | camelCase                           | `cancellationToken`, `hashedPassword`        |
| Strongly-typed IDs | `{Entity}Id`                        | `UserId`, `OrderId`                          |
| Commands           | `{Verb}{Entity}Command`             | `CreateUserCommand`                          |
| Queries            | `{Verb}{Entity}Query`               | `GetUserListQuery`                           |
| Handlers           | `{CommandOrQuery}Handler`           | `CreateUserCommandHandler`                   |
| Domain events      | `{Entity}{Action}DomainEvent`       | `UserCreatedDomainEvent`                     |
| Endpoints          | `{Action}{Entity}Endpoint`          | `CreateUserEndpoint`                         |
| Request/Response   | `{Action}{Entity}Request/Response`  | `CreateUserRequest`, `CreateUserResponse`    |
| Validators         | `{TypeName}Validator`               | `CreateUserCommandValidator`                 |
| DB table names     | snake_case                          | `"users"`, `"user_roles"`                    |

## Type Patterns

- **Strongly-typed IDs**: `public partial record UserId : IGuidStronglyTypedId;` — never manually assign IDs; rely on EF value generators.
- **Records** for: Commands, Queries, Domain Events, DTOs, Responses, IDs.
- **Classes** for: Entities, Handlers, Validators, Endpoints, Services.
- **`sealed`** on Endpoints, Request/Response types. **`internal sealed`** on infrastructure implementations.
- **Primary constructors** (C# 12) for dependency injection everywhere.
- **`private set`** on all entity properties — state changes only through domain methods.
- **Collection expressions** `[]` for initializing collections: `public ICollection<UserRole> UserRoles { get; private set; } = [];`

## Error Handling

- **`KnownException`** for all business rule violations — automatically translated to HTTP error responses.
- **FluentValidation** at both endpoint and command levels. Validators must inherit `AbstractValidator<T>` (not `Validator<T>`).
- Command handlers: `var entity = await repo.GetAsync(id, ct) ?? throw new KnownException("...");`
- Domain entities: throw `KnownException` for invariant violations.
- Validation pipeline via MediatR behavior: `AddKnownExceptionValidationBehavior()`.

## Async Patterns

- Full async/await pipeline — always propagate `CancellationToken`.
- Parameter name: `ct` in endpoints, `cancellationToken` in handlers.
- Repository methods must be async: `GetAsync`, `AddAsync`.
- Command handlers must **not** call `SaveChanges` — the Unit of Work behavior handles this.
- Domain event handlers implement `Handle()` (not `HandleAsync()`).

## DDD / CQRS Rules

- Every aggregate root implements `IAggregateRoot` and extends `Entity<TId>`.
- Only aggregate roots have repositories. Child entities are accessed through the root.
- Domain events are raised only by aggregates/entities (`AddDomainEvent()`).
- Commands modify state via repositories; Queries read directly from `ApplicationDbContext`.
- FastEndpoints are thin: map HTTP request -> Command/Query via MediatR, return result with `Send.OkAsync(result.AsResponseData())`.
- Endpoint permissions: `Permissions(AppPermissions.System_Users_Create)`.

## Copilot / Agent Instructions

Detailed coding instructions for each DDD concept are in `.github/instructions/`:
`aggregate`, `command`, `query`, `endpoint`, `repository`, `entity-configuration`,
`dbcontext`, `domain-event`, `domain-event-handler`, `integration-event`,
`integration-event-converter`, `integration-event-handler`, `unit-testing`.

The master workflow guide is at `.github/copilot-instructions.md`. Development order:
1. Define aggregates/entities -> 2. Domain events -> 3. Repositories -> 4. Entity configs ->
5. Commands + handlers -> 6. Queries + handlers -> 7. Endpoints -> 8. Domain event handlers ->
9. Integration events -> 10. Integration event converters -> 11. Integration event handlers.

## Global Usings

Each project has a `GlobalUsings.cs` with common imports. Per-file imports are only needed for types not globally imported. Import order convention: `System.*` -> `Microsoft.*` -> third-party -> internal project references.

## Miscellaneous

- Solution file: `CleanAdmin.slnx` (XML-based `.slnx` format, not `.sln`)
- C# language version: `preview` (C# 14). Target: `net10.0`.
- Central Package Management via `Directory.Packages.props`.
- XML doc comments use Chinese (Simplified) for domain concepts.
- EF migrations project: `src/CleanAdmin.MigrationService/`
- Add migration: `dotnet ef migrations add <Name> -p src/CleanAdmin.MigrationService`
