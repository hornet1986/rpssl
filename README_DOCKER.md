# Running Rpssl with Docker Compose

## Prerequisites
- Docker Desktop (Windows) with Linux containers mode.

## Services
- `sql`: Microsoft SQL Server 2022 (Express)
- `seq`: Seq structured log server (optional, used by Serilog configuration)
- `api`: The Rpssl ASP.NET Core 8.0 API

## Quick Start
```powershell
# Build and start all services
docker compose up -d --build

# View logs
docker compose logs -f api

# Stop
docker compose down

# Remove volumes as well (destroys DB data & Seq data)
docker compose down -v
```

API will be available at:
- http://localhost:8080
- https://localhost:8081 (if HTTPS is configured inside container; current Dockerfile exposes 8081 but no certificate is bundled by default)

Seq UI: http://localhost:5342 (ingestion API: http://localhost:5341)

## Connection String
Inside the container the API uses:
```
Server=sql;Database=RpsslDb;User ID=sa;Password=Your_strong_password123!;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true
```
Change the `SA_PASSWORD` & matching `ConnectionStrings__RpsslDatabase` in `docker-compose.yml` for production. Password must meet SQL Server complexity requirements.

## Applying EF Core Migrations
If migrations are included in the project they will run on first query if using EnsureCreated/automatic. If you need to apply them manually you can exec into the running container:
```powershell
docker compose exec api dotnet ef database update --project src/Rpssl.Infrastructure --startup-project src/Rpssl.Api
```
(or bake the command into an entrypoint script.)

## Environment Overrides
Use `docker-compose.override.yml` for local-only tweaks (volumes for live code reload, debugger config, etc.).

## Regenerating Images
```powershell
docker compose build --no-cache api
```

## Troubleshooting
- If `api` fails because of database connectivity, check `sql` logs: `docker compose logs sql`.
- Ensure ports 1433, 5341, 5342, 8080, 8081 are free.
- On corporate networks, pulling images may require proxy settings.

## Security Notes
- Change the default passwords before exposing services publicly.
- Consider enabling encryption and proper certificates for production.
- Limit Seq exposure or secure behind authentication.
