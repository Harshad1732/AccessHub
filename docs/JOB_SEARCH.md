# Job search guide — AccessHub on your resume

## Resume project entry (copy/adapt)

**AccessHub** — Multi-tenant RBAC admin API (.NET 8, EF Core, SQL Server, JWT) + React admin UI  
- Designed org-scoped roles and `resource.action` permissions with API-level enforcement and audit logging  
- Implemented JWT permission claims and policy-based authorization on a sample Invoices domain API  
- Built React admin console for users, roles, and permission assignment with 403-aware UX  

## LinkedIn headline tweak

`.NET Developer | ASP.NET Core | SQL Server | Building secure B2B access control (RBAC/IAM)`

## 2-minute interview story

> In ERP work I handled module access for users and roles. I built AccessHub to generalize that: organizations, roles, fine-grained permissions, and an audit trail. Login returns a JWT with permission claims; every protected endpoint uses authorization policies so the UI cannot bypass security. I chose claims in the token for fast checks in the MVP, and I’d add refresh tokens and permission versioning for production.

## Target roles (1 yr experience)

- Junior / Associate .NET Developer (product SaaS)
- Backend .NET + light frontend
- Internal tools / platform teams

## Weekly application habit

- 15–20 tailored applications
- Link GitHub README in every application
- Optional: 30–60s demo video (login → create role → show audit → 403 as viewer)

## Study checklist (parallel)

- [ ] HTTP status codes and REST basics
- [ ] OAuth2/OIDC flows (conceptual)
- [ ] Explain your schema on a whiteboard
- [ ] One mock interview: "How does permission check work on each request?"
