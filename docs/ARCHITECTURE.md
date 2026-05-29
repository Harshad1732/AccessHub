# AccessHub Architecture

## RBAC model

```mermaid
erDiagram
    Organization ||--o{ ApplicationUser : has
    Organization ||--o{ Role : has
    Organization ||--o{ Invoice : has
    Role ||--o{ RolePermission : grants
    Permission ||--o{ RolePermission : included_in
    ApplicationUser ||--o{ UserRole : assigned
    Role ||--o{ UserRole : receives
    ApplicationUser ||--o{ AuditEvent : performs
```

- **Permission**: global catalog (`invoices.read`, `users.write`, …)
- **Role**: scoped to one organization; holds many permissions
- **User**: belongs to one organization (except super admin); has many roles
- **Super admin**: `IsSuperAdmin = true`, no org; all permissions in JWT

## Request flow

```mermaid
sequenceDiagram
    participant UI as React
    participant API as ASP.NET API
    participant Auth as JWT Middleware
    participant RBAC as Permission Handler
    participant DB as SQL Server

    UI->>API: POST /api/auth/login
    API->>DB: Validate user + load roles
    API-->>UI: JWT with permission claims
    UI->>API: GET /api/invoices + Bearer
    API->>Auth: Validate token
    Auth->>RBAC: invoices.read policy
    RBAC-->>API: Succeed or 403
    API->>DB: Query invoices
    API-->>UI: 200 or 403
```

## Layers

| Layer | Responsibility |
|-------|----------------|
| **Domain** | Entities, permission code constants |
| **Application** | `IPermissionService`, DTOs, use-case interfaces |
| **Infrastructure** | EF Core, Identity, JWT token generation, audit persistence |
| **Api** | HTTP, authorization policies, controllers |

## Security notes (MVP)

- Passwords hashed via ASP.NET Identity
- JWT signed with symmetric key (configure via `Jwt:Key` in appsettings)
- CORS limited to `http://localhost:5173` in development
- Do not commit production secrets; use user secrets or environment variables

## Future improvements

- Refresh tokens and permission version claim
- Row-level security filters on all org-scoped queries
- Policy-based ABAC (deny overrides)
- Azure AD / OIDC federation
