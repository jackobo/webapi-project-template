# AGENTS.md

Instructions for AI coding agents (and new developers) working in this solution.

## Read the Jackobo framework documentation first

This service is built on the **Jackobo.AspNetCore.Framework** NuGet package. That package is not a
utility library - it *is* the host: it owns startup, the configuration cascade, logging, the
mediator, the result contract, error handling, Redis and the Roslyn analyzers. Writing code here without knowing its conventions produces code that compiles and is
still wrong.

The full usage documentation **ships inside the package**, so it is available offline on any
machine that has restored this solution:

```text
%UserProfile%\.nuget\packages\jackobo.aspnetcore.framework\<version>\docs\
```

Replace `<version>` with the version of the `Jackobo.AspNetCore.Framework` `PackageReference` in
`Core\Core.csproj`, `Infrastructure\Infrastructure.csproj` and `WebAPI\WebAPI.csproj` (they are
always kept identical). On Windows `%UserProfile%` is typically `C:\Users\<you>`.

Read, in this order:

1. **`13-agent-checklist.md`** - the do/don't rules that are violated most often. Read it before
   writing any code.
2. **`README.md`** - the index of the whole set. Always take the list of documents from it rather
   than from memory: documents get added, and it is the only place that stays accurate. Use it to
   jump straight to the area you are about to touch (startup and options, configuration,
   `OperationResult`, mediator and controllers, Redis, HTTP and external services, logging and performance, data access, background services, and whatever has
   been added since).

The same documents live in the framework repository, which is the authoritative source:
<https://github.com/jackobo/aspnet-core-framework/tree/main/docs>.

## The rules that matter most

These are summarised here only so that an agent that reads nothing else still avoids the four
classic mistakes. They are explained in full in `13-agent-checklist.md`.

- **Business failures are `OperationResult` failures, not exceptions.** Throwing for an expected
  outcome bypasses the standard response contract.
- **Never read `OperationResult.Value` without checking success first.** The `JKB0001`-`JKB0004`
  analyzers ship with the package and will flag it - do not suppress them.
- **Never re-register what the framework already registers.** Logging, Swagger, health checks, the
  mediator and Redis clients are wired by `JackoboWebapi` itself.
- **Never copy environment-level configuration into `appsettings.json`.** The framework ships
  `jackobodefaults*.json` per environment and applies them *before* the application settings, so a
  key repeated locally - even empty - overrides the shared default. This service declares only what
  it owns (see the Configuration section of `README.md`).

## This solution

| Project | Responsibility |
|---|---|
| `Core` | Mediator handlers (queries/commands), DTOs, abstractions, the error catalog. |
| `Infrastructure` | Integrations: service implementations, config sections, HTTP clients. |
| `WebAPI` | Host: `Program.cs`, controllers, `appsettings`, Dockerfile. |
| `*.Tests` | xUnit v3 + NSubstitute, one project per layer. |

`README.md` in this folder describes the layout, the sample request flow, the configuration
ownership split and the places marked `TODO:` that you are expected to adapt. Search the solution
for `TODO:` before adding anything new.

Build and test:

```powershell
dotnet build Jackobo.WebApiTemplate.slnx
dotnet test Jackobo.WebApiTemplate.slnx
```

Prefer what the framework already provides over hand-rolled equivalents, and follow the
conventions in its documentation rather than generic ASP.NET Core guidance.
