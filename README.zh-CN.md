# CleanAdmin

> 基于 CleanDDD 的全栈后台模板，使用 .NET 10、Aspire 和 Blazor 构建。

[English](README.md) | 简体中文

[![.NET](https://img.shields.io/badge/.NET-10.0-blueviolet?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-13.x-512BD4)](https://learn.microsoft.com/dotnet/aspire/)
[![License](https://img.shields.io/badge/license-MIT-green)](./LICENSE)

CleanAdmin 是一个基于 .NET 10 的开源后台模板，面向轻量、快速、可维护的企业级 Web 应用开发。模板集成 FastEndpoints、Aspire 与
Scalar，提供现代 API 开发与文档体验。

CleanAdmin 聚焦清晰架构与简洁实现，在单一解决方案中整合后端 API、前端 UI、基础设施编排、迁移服务与测试工程；同时支持
Microsoft.Kiota 自动生成强类型 API 客户端，降低联调与维护成本，提升团队交付效率。

## 👀 项目预览

- 在线预览地址：https://cleanadmin.azurewebsites.net
- 说明：预览站当前基于 Apifox 生成的 Mock 数据进行展示。

## 🌟 为什么选择 CleanAdmin

相比传统后台模板，CleanAdmin 更关注工程的可维护性与现代化的开发体验：

- **基于 netcorepal-cloud-framework 的底层支撑**：落地领域驱动设计（DDD）战术模式，内置
  CQRS、事件驱动、分布式事务（最终一致性）、多租户、多环境部署（含灰度）与分库分表等核心架构能力。
- **Aspire 云原生零配置开发环境**：一键自动拉起 API、Web、数据库及中间件等全套服务，免去繁杂的本地环境搭建。
- **基于 FastEndpoints 的极简 API**：采用 REPR 模式与垂直切片架构替代传统控制器，提升代码内聚性与执行效率。
- **现代 API 文档体验（Scalar）**：提供更现代的 API 文档与调试界面，便于接口浏览、联调与团队协作。
- **Kiota 强类型 API 客户端**：基于 OpenAPI 自动生成强类型化 API 客户端，减少开发开销。
- **Blazor 全栈开发**：前后端统一 C# 技术栈。基于 Ant Design Blazor 与 Tailwind CSS 提供后端友好的前端开发体验。

## 🏗️ 架构说明

解决方案包含以下项目：

| 项目 | 说明 |
|------|------|
| `CleanAdmin.AppHost` | Aspire AppHost — 统一编排入口 |
| `CleanAdmin.ApiService` | 后端 API 服务 |
| `CleanAdmin.Web` | Blazor Web 宿主与 YARP 反向代理入口 |
| `CleanAdmin.Web.Client` | Blazor WebAssembly 前端客户端 |
| `CleanAdmin.MigrationService` | EF Core 数据库迁移工作服务 |
| `CleanAdmin.Domain` | 领域模型、聚合与业务规则 |
| `CleanAdmin.Infrastructure` | EF Core DbContext、仓储与集成实现 |
| `CleanAdmin.Shared` | 前后端共享代码 |
| `CleanAdmin.ServiceDefaults` | Aspire 共享默认配置、可观测性与健康检查 |
| `CleanAdmin.*.Tests` | 单元测试与集成测试（xUnit、Testcontainers） |

## 🧰 技术栈

| 类别 | 技术 |
|------|------|
| 运行时 | .NET 10（SDK `10.0.100`，见 `global.json`） |
| 云编排 | .NET Aspire |
| API 框架 | FastEndpoints（REPR 模式，垂直切片） |
| 前端 | Blazor WebAssembly + Ant Design Blazor + Tailwind CSS |
| 数据库 | PostgreSQL（EF Core + Npgsql） |
| 缓存 | Redis |
| 消息队列 | RabbitMQ（通过 CAP 实现分布式事务） |
| CQRS / 中介者 | MediatR |
| 模型验证 | FluentValidation |
| 身份认证 | JWT + API Key（双方案） |
| 任务调度 | Hangfire（Redis 存储） |
| API 客户端生成 | Microsoft Kiota |
| API 文档 | Scalar |
| 可观测性 | OpenTelemetry + Prometheus + Serilog |
| 反向代理 | YARP |
| DDD 框架 | NetCorePal Cloud Framework |
| 测试 | xUnit v3 + Testcontainers + Shouldly + Moq |

## 🚀 快速开始

### 1) ✅ 环境准备

- .NET SDK 10.0.100
- Docker Desktop
- Node.js 18+

### 2) 📦 通过模板创建新项目

CleanAdmin 支持 `dotnet new` 模板，可以快速创建以自己项目名命名的全新解决方案：

```bash
# 安装模板（从 NuGet）
dotnet new install CleanAdmin.Template

# 或从本地源码安装
dotnet new install .

# 创建新项目（将 CleanAdmin 替换为你的项目名）
dotnet new cleanadmin -n CleanAdmin -o CleanAdmin
cd CleanAdmin
```

### 3) 📦 还原依赖

```bash
dotnet restore
```

### 4) 📦 安装前端依赖

Tailwind CSS 构建需要 Node.js 依赖：

```bash
cd src/CleanAdmin.Web
npm install
cd ../..
```

### 5) ▶️ 通过 Aspire 启动完整系统

```bash
dotnet run --project src/CleanAdmin.AppHost
```

AppHost 会统一编排 PostgreSQL、Redis、RabbitMQ、MigrationService、ApiService 和 Web 前端。

## 🛠️ 后端开发说明

后端开发请参考 `.github/instructions` 下的规范文件（如 `endpoint.instructions.md`、`command.instructions.md`、
`query.instructions.md`、`aggregate.instructions.md` 等）。

## 💻 本地开发

开发阶段通过 Release 模式构建 ApiService 以生成 API 客户端：

```bash
dotnet build src/CleanAdmin.ApiService -c Release
```

开发阶段监听 CSS 以供热重载：

```bash
cd src/CleanAdmin.Web
npm run watch:css
```

## 🔗 常用端点

- Scalar（开发环境）：`/scalar`
- 健康检查：`/health`
- 存活检查：`/alive`
- Prometheus 指标：`/metrics`
- CAP 仪表盘：`/cap`
- Hangfire 仪表盘：`/hangfire`
- 代码分析可视化：`/code-analysis`

## ⚙️ 配置说明

- 本地默认配置位于各项目的 `appsettings*.json`
- Aspire 参数位于 `src/CleanAdmin.AppHost/appsettings.json`
- 敏感信息建议使用环境变量或 user-secrets 管理

示例：

```bash
dotnet user-secrets set "Auth:Jwt:TokenSigningKey" "your-long-random-signing-key" --project src/CleanAdmin.ApiService
dotnet user-secrets set "Auth:ApiKey" "your-api-key" --project src/CleanAdmin.ApiService
```

## 🗄️ 数据库迁移

通过 AppHost 启动时，`CleanAdmin.MigrationService` 会自动执行迁移。

如需手动使用 EF CLI：

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add <MigrationName> -p src/CleanAdmin.MigrationService
dotnet ef database update -p src/CleanAdmin.MigrationService
```

## 🧪 测试

运行全部测试：

```bash
dotnet test
```

## ✨ IDE 代码片段

仓库内置常用 DDD 模式的开发效率代码片段：

- **VS Code**：`.vscode/csharp.code-snippets` — 输入 `epp`、`ncpcmd`、`ncpar`、`ncprepo` 等前缀触发
- **Visual Studio**：`vs-snippets/NetCorePalTemplates.snippet` — 运行 `vs-snippets/Install-VSSnippets.ps1` 安装

## 📚 相关项目

- [NetCorePal Cloud Framework](https://github.com/netcorepal/netcorepal-cloud-framework)
- [Ant Design Blazor](https://github.com/ant-design-blazor/ant-design-blazor)
- [FastEndpoints](https://fast-endpoints.com/)

## 🤝 参与贡献

欢迎通过多种方式参与项目建设，提交代码、提出建议或反馈问题都非常感谢。

## 📄 许可证

MIT License。详见 `LICENSE`。
