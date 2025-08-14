# RPSSL – Rock-Paper-Scissors-Spock-Lizard API (ASP.NET Core 8)

An educational, layered .NET 8 Web API that implements core mechanics for the Rock–Paper–Scissors–Spock–Lizard game. The solution demonstrates clean layering, versioned REST endpoints, EF Core persistence, health checks, structured logging with Serilog + Seq, and Docker Compose orchestration with SQL Server.

## Solution Layout

```
Rpssl.sln
src/
  Rpssl.Api/            <-- ASP.NET Core Web API host (Serilog, versioning, Swagger)
  Rpssl.Application/    <-- Application layer (validators, orchestrations, use cases)
  Rpssl.Domain/         <-- Domain model & business rules
  Rpssl.Infrastructure/ <-- EF Core, repositories, external service clients, Polly policies
  Rpssl.SharedKernel/   <-- Cross-cutting abstractions (result types, interfaces, constants)
tests/
  Rpssl.Api.Tests/
  Rpssl.Application.Tests/
  Rpssl.Infrastructure.Tests/
docker-compose*.yml     <-- sql (SQL Server), seq (logging), api (this service)
README_DOCKER.md        <-- Extra docker specific notes
global.json             <-- Pins .NET SDK (8.x)
```

## Key Technologies

- .NET 8 / ASP.NET Core minimal hosting model
- API Versioning (`Asp.Versioning.Mvc` & API Explorer integration)
- Swagger / OpenAPI (`Swashbuckle.AspNetCore` + annotations)
- Entity Framework Core (SQL Server provider)
- FluentValidation for request validation
- Health Checks (core + SQL Server)
- Serilog with Seq sink (structured logging)
- JWT Authentication (package references present; configure keys/authority as needed)
- Polly for transient fault handling (Infrastructure layer)
- MSTest + Moq + EF Core InMemory for tests

## Prerequisites

Local (non‑Docker) development:
- .NET SDK 8.x (pinned by `global.json`)
- SQL Server instance (localdb, Docker, or remote) if running persistence locally

Containerized development:
- Docker Desktop (Linux containers mode)

Optional:
- Seq (provided via docker compose) for log exploration

## Quick Start (Using Docker Compose)

This starts SQL Server, Seq, and the API in one step.

```powershell
git clone https://github.com/hornet1986/rpssl.git
cd rpssl
docker compose up -d --build
```

Endpoints (default dev):
- API: http://localhost:8080
- (If HTTPS configured) https://localhost:8081
- Swagger UI: http://localhost:8080/swagger
- Seq UI: http://localhost:5342 (ingestion at http://localhost:5341)

Follow logs:
```powershell
docker compose logs -f api
```

Tear down (keeping volumes):
```powershell
docker compose down
```

Tear down and remove data volumes (destroys DB & Seq data):
```powershell
docker compose down -v
```

See `README_DOCKER.md` for deeper operational notes (rebuild without cache, troubleshooting, etc.).

## Quick Start (Running Locally Without Docker)

1. Ensure a SQL Server is available (or adjust connection for InMemory provider while prototyping).
2. Set environment variables or `appsettings.Development.json` connection string, e.g.:
	`ConnectionStrings:RpsslDatabase=Server=localhost;Database=RpsslDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true`
3. Restore & build:
	```powershell
	dotnet restore
	dotnet build --no-restore
	```
4. Run the API:
	```powershell
	dotnet run --project src/Rpssl.Api
	```
5. Browse Swagger at http://localhost:5080/swagger (actual port may differ; console output lists the bound URLs). You can override with:
	```powershell
	dotnet run --project src/Rpssl.Api --urls "http://localhost:8080"
	```

## Configuration & Environment

Environment variables (Docker examples):
- `ASPNETCORE_ENVIRONMENT` (Development / Staging / Production)
- `ConnectionStrings__RpsslDatabase` (double underscore maps to `:` in config keys)
- `Serilog__WriteTo__1__Args__ServerUrl` (Seq ingestion URL)

Add secrets for local development via `dotnet user-secrets` in `Rpssl.Api` (UserSecretsId already present) for items like JWT signing keys, external API keys, etc.

## Logging

Serilog is configured (packages included) to emit structured logs. When running under Docker Compose, logs are forwarded to the Seq container. Access the Seq UI at http://localhost:5342.

## Health Checks

Standard health endpoints (typical patterns):
- Liveness: `/health` (application up)
- Readiness: may be extended to include DB (`/health/db` or similar) – adjust README once exact paths are finalized.

If you change or add health endpoints, update this section accordingly.

## Entity Framework Core & Migrations

Add migrations (example):
```powershell
dotnet ef migrations add InitialCreate --project src/Rpssl.Infrastructure --startup-project src/Rpssl.Api
```
Apply migrations:
```powershell
dotnet ef database update --project src/Rpssl.Infrastructure --startup-project src/Rpssl.Api
```
Inside a running container:
```powershell
docker compose exec api dotnet ef database update --project src/Rpssl.Infrastructure --startup-project src/Rpssl.Api
```

## Testing

Run all tests:
```powershell
dotnet test --no-build
```
Or a single test project:
```powershell
dotnet test tests/Rpssl.Application.Tests/Rpssl.Application.Tests.csproj
```

Continuous test run (watch mode):
```powershell
dotnet watch --project tests/Rpssl.Api.Tests test
```

## API Versioning & Swagger

API versioning packages are referenced. Expect versioned routes such as `/api/v1/...`. Swagger (Swashbuckle) surfaces multiple versions when configured. Use the dropdown in Swagger UI to switch versions.

## Authentication / Authorization

Packages for JWT Bearer auth are present. Configure in `Program.cs` (not shown here) with issuer, audience, and signing keys. For local development you can supply minimal configuration via user secrets or environment variables. Update this section once the auth story is finalized (e.g., issuing tokens, required scopes/claims).

## Resilience (Polly)

`Polly` is referenced for transient fault handling. Typical patterns to implement (if not already) include retries with jitter, circuit breakers for external calls, and timeout strategies.

## Development Workflow

Common tasks:
- Add a feature: modify Domain + Application; wire via Infrastructure; expose via Api controllers.
- Update DB schema: add migration, update DB, commit migration file.
- Observe behavior: check structured logs in console or Seq.

## Makefile / Tasks (Future)

Consider adding a `Directory.Build.props`, `Makefile`, or `dotnet tool` manifest for build pipelines. (Not included yet.)

## Production Hardening Checklist (Not Yet Implemented)

- Proper HTTPS certificates & TLS termination
- Secure secrets storage (Azure Key Vault, AWS Secrets Manager, etc.)
- Authentication & authorization policies
- Rate limiting & input throttling
- Comprehensive observability (structured logs, metrics, tracing)
- CI pipeline (build, test, code quality, container scan)
- Automated database migrations / migrations gating

## Contributing

1. Create a feature branch: `git checkout -b feature/short-desc`
2. Commit with conventional commit style if desired (e.g., `feat: add game outcome service`)
3. Run `dotnet test` before pushing.
4. Open a PR; ensure CI passes.

## License

MIT (see `LICENSE`).

## Attributions

Game concept adapted from Sam Kass / Karen Bryla (popularized by “The Big Bang Theory”).

---
Feel free to open issues for enhancements, questions, or clarifications. This README will evolve as the API surface and gameplay logic are implemented.