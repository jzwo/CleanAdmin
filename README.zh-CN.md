# CleanAdmin

> 基于 CleanDDD 的全栈后台模板，使用 .NET 10、Aspire、FastEndpoints 和 Blazor 构建。

[English](README.md) | 简体中文

[![.NET](https://img.shields.io/badge/.NET-10.0-blueviolet)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-13.x-512BD4)](https://learn.microsoft.com/dotnet/aspire/)
[![License](https://img.shields.io/badge/license-MIT-green)](./LICENSE)

CleanAdmin 是一个面向开源的管理系统脚手架，提供清晰的领域边界和面向生产的默认配置。项目在一个解决方案中同时包含后端 API、前端 UI、基础设施编排、迁移服务与测试工程。

## 为什么选择 CleanAdmin

- CleanDDD 风格分层结构（Domain、Infrastructure、API、Web）
- 基于 .NET Aspire 的本地分布式开发编排
- FastEndpoints + FluentValidation + Swagger，提升 API 开发效率
- 内置 Redis、RabbitMQ、PostgreSQL 集成
- 开箱即用支持 CAP、Hangfire、OpenTelemetry、Prometheus
- 基于 Blazor，结合 Ant Design 生态与 Tailwind CSS

## 架构说明

解决方案包含以下项目：

- `CleanAdmin.AppHost`：Aspire AppHost（统一编排入口）
- `CleanAdmin.ApiService`：后端 API 服务
- `CleanAdmin.Web`：Blazor Web 宿主与反向代理入口
- `CleanAdmin.Web.Client`：前端客户端工程
- `CleanAdmin.MigrationService`：数据库迁移与种子数据服务
- `CleanAdmin.Domain`：领域模型与业务规则
- `CleanAdmin.Infrastructure`：EF Core、仓储与集成实现
- `CleanAdmin.ServiceDefaults`：统一可观测性、健康检查与服务默认配置

## 技术栈

- .NET SDK `10.0.100`（见 `global.json`）
- ASP.NET Core + Blazor
- FastEndpoints
- Entity Framework Core + PostgreSQL
- Redis + RabbitMQ
- CAP + Hangfire
- OpenTelemetry + Prometheus
- Aspire

## 快速开始

### 1）环境准备

- .NET SDK 10.0.100（允许预览版）
- Docker Desktop
- Node.js 18+

### 2）还原依赖

```bash
dotnet restore
```

### 3）构建前端样式资源

```bash
cd src/CleanAdmin.Web
npm install
npm run build:css
```

### 4）通过 Aspire 启动完整系统

```bash
dotnet run --project src/CleanAdmin.AppHost
```

AppHost 会统一编排 PostgreSQL、Redis、RabbitMQ、MigrationService、ApiService 和 Web 前端。

## 本地开发

仅启动 API：

```bash
dotnet run --project src/CleanAdmin.ApiService
```

仅启动 Web：

```bash
dotnet run --project src/CleanAdmin.Web
```

开发阶段监听 CSS：

```bash
cd src/CleanAdmin.Web
npm run watch:css
```

## 常用端点

- Swagger（开发环境）：`/swagger`
- 健康检查：`/health`
- 存活检查：`/alive`
- Prometheus 指标：`/metrics`
- CAP 仪表盘：`/cap`
- Hangfire 仪表盘：`/hangfire`
- 代码分析可视化：`/code-analysis`

## 配置说明

- 本地默认配置位于各项目的 `appsettings*.json`
- Aspire 参数位于 `src/CleanAdmin.AppHost/appsettings.json`
- 敏感信息建议使用环境变量或 user-secrets 管理

示例：

```bash
dotnet user-secrets set "Auth:Jwt:TokenSigningKey" "your-long-random-signing-key" --project src/CleanAdmin.ApiService
dotnet user-secrets set "Auth:ApiKey" "your-api-key" --project src/CleanAdmin.ApiService
```

## 数据库迁移

通过 AppHost 启动时，`CleanAdmin.MigrationService` 会自动执行迁移。

如需手动使用 EF CLI：

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add <MigrationName> -p src/CleanAdmin.Infrastructure
dotnet ef database update -p src/CleanAdmin.Infrastructure
```

## 测试

运行全部测试：

```bash
dotnet test
```

## IDE 代码片段

仓库内置开发效率代码片段，位于 `vs-snippets` 与 `.vscode/csharp.code-snippets`。

更多说明：`vs-snippets/README.md`

## 相关项目

- [NetCorePal Cloud Framework](https://github.com/netcorepal/netcorepal-cloud-framework)
- [ASP.NET Core](https://github.com/dotnet/aspnetcore)
- [Entity Framework Core](https://github.com/dotnet/efcore)
- [CAP](https://github.com/dotnetcore/CAP)
- [FastEndpoints](https://fast-endpoints.com/)

## 参与贡献

欢迎提交贡献。

1. Fork 本仓库
2. 创建功能分支（`git checkout -b feat/your-feature`）
3. 提交代码（`git commit -m "feat: add ..."`）
4. 推送分支并创建 Pull Request

提交前请确保构建与测试通过。

## 许可证

MIT License。详见 `LICENSE`。
