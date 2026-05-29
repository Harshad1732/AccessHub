# AccessHub — Advanced User Access Management (IAM)

A portfolio-grade B2B admin console for **organizations, users, roles, permissions, audit logs**, and a sample protected **Invoices** API. Built with **.NET 8**, **EF Core**, **SQL Server**, **JWT**, and **React + MUI**.

## Features

- Multi-organization (tenant) model
- RBAC: roles with fine-grained `resource.action` permissions
- JWT claims include permissions; API enforces policies (403, not UI-only)
- Audit trail for admin mutations (orgs, users, roles)
- Sample protected resource: Invoices (`invoices.read` / `invoices.write`)
- React admin UI with permission-aware navigation

## Architecture

```
React (web/)  →  ASP.NET Core API  →  Application services  →  EF Core  →  SQL Server
```

See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for RBAC model and design decisions.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- SQL Server LocalDB (Visual Studio) **or** Docker for SQL Server

## Quick start (LocalDB)

```powershell
# Terminal 1 — API
cd d:\IAM
dotnet restore
dotnet run --project src/AccessHub.Api

# Terminal 2 — Web
cd d:\IAM\web
npm install
npm run dev
```

- API + Swagger: http://localhost:5177/swagger  
- Web UI: http://localhost:5173  

Database is created and seeded on first API run.

## Seed accounts

| Email | Password | Role |
|-------|----------|------|
| `superadmin@accesshub.local` | `SuperAdmin123!` | Super admin (all permissions) |
| `admin@acme.local` | `Admin123!` | Acme org admin |
| `viewer@acme.local` | `Viewer123!` | Acme viewer (read-only) |

## Docker SQL Server (optional)

```powershell
docker compose up -d
```

Copy connection string from [docker-compose.override.example.json](docker-compose.override.example.json) into `src/AccessHub.Api/appsettings.Development.json`.

## Tests

```powershell
dotnet test
```

7 unit tests cover permission resolution and permission catalog.

## Project structure

```
src/
  AccessHub.Api/           Web API, JWT, controllers
  AccessHub.Application/   Services, DTOs, interfaces
  AccessHub.Domain/        Entities, permission constants
  AccessHub.Infrastructure/ EF Core, Identity, seed data
web/                       React + Vite + MUI admin UI
tests/AccessHub.Tests/     xUnit tests
docs/                      Architecture notes
```

## Interview talking points

1. **Why JWT claims for permissions?** Fast checks per request; tradeoff is stale claims until re-login (MVP).
2. **Why separate OrgRoles from Identity roles?** ERP-style app RBAC vs framework authentication.
3. **Why audit log?** Compliance and traceability for access changes.

## License

MIT — use freely for portfolio and learning.
