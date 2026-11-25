# Jackobo WebApiTemplate Template

A clean, ready-to-build skeleton for an Jackobo-based ASP.NET Core Web API service.
It contains the standard three-layer structure and a fully working sample endpoint that
demonstrates the request flow used across all services.

> This is the **template source** (a clean reference solution). In a later step it will be
> turned into a `dotnet new` template and packaged as a NuGet so it can be used directly from
> the Visual Studio "Create a new project" dialog.

## Projects

| Project          | Responsibility                                                                 |
|------------------|--------------------------------------------------------------------------------|
| `Core`           | Application logic: mediator handlers (queries/commands), DTOs, abstractions, errors. |
| `Infrastructure` | Outside-world integrations: service implementations, config, HTTP clients, auth. |
| `WebAPI`         | Host: startup (`Program.cs`), controllers, settings, Dockerfile.               |
| `Core.Tests`, `Infrastructure.Tests`, `WebAPI.Tests` | Unit tests, one project per layer. |

All projects target **net10.0** and reference **Jackobo.AspNetCore.Framework**.

## Framework documentation (`AGENTS.md`)

The framework's usage documentation ships **inside the `Jackobo.AspNetCore.Framework` NuGet package**,
so after a restore it is available offline at
`%UserProfile%\.nuget\packages\jackobo.aspnetcore.framework\<version>\docs\`. The `README.md` in that
folder is the index of the whole set, so it stays accurate when documents are added.

Nothing would make an AI coding agent look there on its own, so the template ships an
[`AGENTS.md`](AGENTS.md) at the root of every generated solution - the file agents read by
convention - that points at that folder and summarises the rules that must not be broken. Keep it
next to `README.md` and extend it with the conventions specific to your service.

## Unit tests

| Choice | Package |
|---|---|
| Test framework | **xUnit v3** (`xunit.v3`) — runs on Microsoft.Testing.Platform, the only mode the .NET 10 SDK supports. |
| Isolation (mocking) framework | **NSubstitute** (+ `NSubstitute.Analyzers.CSharp`) — same substitute API for interfaces and virtual members, no separate `Mock<T>.Object`. |

`global.json` selects the Microsoft.Testing.Platform runner for `dotnet test`; without it the SDK
falls back to VSTest and the run fails.

```powershell
dotnet test Jackobo.WebApiTemplate.slnx
```

Each project ships one sample test to copy:

- `Core.Tests/Handlers/GetSampleByIdQueryHandlerTests.cs` — handler with a substituted
  `ISampleService`, plus the FluentValidation validator.
- `Infrastructure.Tests/Services/SampleServiceTests.cs` — service behaviour, including the
  `ApiErrors.RecordNotFound` failure path.
- `WebAPI.Tests/Controllers/SampleControllerTests.cs` — controller with a substituted
  `ISender` resolved from `HttpContext.RequestServices`.

The three test projects sit next to the projects they cover, at the root of the solution. They are
deliberately not grouped in a `Tests` solution folder: Visual Studio builds its own `.slnx` when a
solution is created from the "Create a new project" dialog, so any solution folder declared by the
template is lost anyway (the "add projects to a solution file" post action is
[ignored in Visual Studio](https://github.com/dotnet/sdk/blob/main/documentation/TemplateEngine/Post-Action-Registry.md)).

Handlers, validators and services are `internal`, so `Core.csproj` and `Infrastructure.csproj`
declare `InternalsVisibleTo` for their test project (and for `DynamicProxyGenAssembly2`, the
assembly NSubstitute emits its substitutes into). The Jackobo `OperationResult` analyzers (`JKB0001`,
`JKB0002`) are switched off in the test projects, where an `Assert` — not an `if` — is the guard.

Pass `--IncludeTests false` to scaffold the solution without the test projects.

## The sample flow

The sample shows the full path a request takes:

```
SampleController
  -> GetSampleByIdQuery / CreateSampleCommand   (mediator request, in Core)
    -> ...QueryHandler / ...CommandHandler       (handler, in Core, auto-discovered)
      -> ISampleService -> SampleService         (abstraction in Core, impl in Infrastructure)
        -> SampleDTO                             (response, wrapped in StandardApiResponse)
```

Endpoints (Swagger UI opens at the `swagger` path):

- `GET  /api/v1/sample/{id}` — returns a sample item.
- `POST /api/v1/sample`      — creates a sample item.

The framework configures no authentication, so both endpoints are open. Add an authentication
scheme and `[Authorize]` once your service needs them.

## Getting started

```powershell
dotnet build Jackobo.WebApiTemplate.slnx
dotnet test Jackobo.WebApiTemplate.slnx
dotnet run --project WebAPI
```

Then open the Swagger UI and try the sample endpoints.

### Scaffolding a new service

Four parameters are **required** - they identify the service and have no sensible automatic
value:

| Parameter | Maps to | Example |
|---|---|---|
| `--AppTitle` | `JackoboWebApiOptions.appTitle` (Swagger document title) | `"Orders API"` |
| `--ElasticIndex` | `Logger:ElasticIndex` in `appsettings.json` | `orders-service` |
| `--RedisKeyPrefix` | `Redis:KeyPrefix` in `appsettings.json` | `OrdersService` |
| `--ApiErrorsPrefix` | `ProjectCode` in `Core/Errors/ApiErrors.cs`, prepended to every error code | `ORD` |

```powershell
dotnet new jackobo-webapi -n MyCompany.Orders.Service `
    --AppTitle "Orders API" `
    --ElasticIndex orders-service `
    --RedisKeyPrefix OrdersService `
    --ApiErrorsPrefix ORD
```

or through the wrapper script: `.\new-project.ps1 -Name ... -AppTitle ... -ElasticIndex ... -RedisKeyPrefix ... -ApiErrorsPrefix ...`.

## Configuration

The framework ships its own per environment defaults (`EnvironmentDefaults\jackobodefaults*.json`,
embedded in `Jackobo.AspNetCore.Framework`) and applies them **before** the application's
`appsettings` files. Everything that depends on the environment and not on the service already
comes from there:

| Owned by the framework (do **not** repeat it) | Owned by this service |
|---|---|
| `AllowedHosts`, `Environment:Type`, `Kestrel:Limits`, `PerformanceMetrics` | `Logger:ElasticIndex` |
| the whole `Serilog` section (sinks + minimum levels) | `Redis:KeyPrefix` |
| `Redis:Shared:ConnectionString` | `Redis:Dedicated:ConnectionString` |
| | every `ExternalApi` entry |

Precedence (last one wins): `jackobodefaults.json` -> `jackobodefaults.{env}.json` (cascaded) ->
`appsettings.json` -> `appsettings.{env}.json` (cascaded) -> environment variables. A key left
behind with an empty placeholder in `appsettings.json` therefore **wipes out** the framework
default - only declare what this service owns.

### Environments

| `ASPNETCORE_ENVIRONMENT` | Files loaded | Launch profile |
|---|---|---|
| `Development` | `appsettings.json` + `appsettings.Development.json` | `Development` (local runs) |
| `QA` | `jackobodefaults.QA.json` + `appsettings.json` + `appsettings.QA.json` | `QA`, `Container (Dockerfile)` |
| `Production` | `jackobodefaults.Production.json` + `appsettings.json` + `appsettings.Production.json` | - |

The framework ships no defaults for `Development`, so `appsettings.Development.json` sets
`Environment:Type` to `Development` itself (Swagger and exception details on). A dotted name such
as `QA.Staging` cascades: it loads the `QA` files first, then the `QA.Staging` ones.

### Logging

The service declares no `Serilog` section at all. The sinks come from the framework defaults and
the elastic index - which identifies the service and is the same in every environment - is declared
once:

```json
"Logger": { "ElasticIndex": "my-service" }
```

A sink is disabled by prefixing its `Name` with `X` (`XRollingFileAlternate`); a
service that needs a sink of its own declares it under its own key in `Serilog:WriteTo`.

### Redis

`Redis:KeyPrefix` is declared by **every service**: the framework refuses to register a Redis
client without a per service key prefix as soon as a Redis connection string is configured -
including a shared one that may later come from the framework environment defaults.

There is no registration code - the framework registers the clients from configuration:

- **Shared** (`Redis:Shared:ConnectionString`, from the framework defaults) -> `ISharedRedisService`.
- **Dedicated** (`Redis:Dedicated:ConnectionString`, declared by this service per environment) ->
  `IDedicatedRedisService`.

## Where to make your changes

Search the solution for `TODO:` — every place you are expected to adapt is marked. The main ones:

1. `Core/GlobalConstants.cs` — your constants.
2. `Core/Errors/ApiErrors.cs` — your business error catalog (passed to
   `JackoboWebApiOptions.applicationSpecificErrors`, mandatory).
3. `Core/Abstractions` + `Core/DTO` + `Core/Handlers` — replace the `Sample` query/command/DTO.
4. `Infrastructure/Services` — replace `SampleService` with real data access.
5. `Infrastructure/InfrastructureDependencyInjection.cs` — register your services, DbContext, clients.
6. `WebAPI/Program.cs` — set the API title and any cross-cutting services.
7. `WebAPI/appsettings.json` — the elastic index, the Redis key prefix and the configuration this
   service owns (see [Configuration](#configuration)).
