# Jersey Store

Jersey Store is a full-stack football jersey storefront with an ASP.NET Core API and a Next.js frontend.

## Current project status

**Current milestone: Milestone 3 — Search, filter, pagination.**

The project has progressed beyond the initial health-check setup. The API serves a PostgreSQL-backed product catalog through EF Core, and the storefront supports product details, catalog search and filters, paged results, and ordering through WhatsApp. Milestone 3 is the current catalog milestone; the next roadmap milestone is admin login and product management.

The project vision and milestone scope are guided by [`docs/milestones.md`](docs/milestones.md). Store all project plans in `.kilo/plans/`.

## Run both (with hot reload)

From the repository root:

```bash
make dev
```

- API: http://localhost:5080/api/health
- OpenAPI: http://localhost:5080/openapi/v1.json
- Frontend: http://localhost:3000

Both backend (`dotnet watch`) and frontend (`next dev`) support hot reload.

## Run individually

```bash
make dev-backend   # API only (port 5080)
make dev-frontend  # Frontend only (port 3000)
```

## Manual (two terminals)

**Terminal 1 — Backend:**
```bash
dotnet watch --project backend/JerseyStore.Api/JerseyStore.Api.csproj --launch-profile JerseyStore.Api
```

**Terminal 2 — Frontend:**
```bash
cd frontend && pnpm dev
```