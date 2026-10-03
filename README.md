# Jersey Store

Jersey Store is a full-stack football jersey storefront with an ASP.NET Core API and a Next.js frontend.

## Current project status

**Current milestone: Milestone 5 — Cart (next). Milestone 4 — Admin login and product management is complete.**

The API serves a PostgreSQL-backed product catalog through EF Core, and the storefront supports product details, catalog search and filters, paged results, and ordering through WhatsApp. Milestone 4 added a role-protected admin area at `/admin` for product, variant, and stock management, plus local image uploads with gallery thumbnails, primary-image selection, and image removal, product search, and a settings page for the store WhatsApp number (stored in the database, falling back to `Store:WhatsAppNumber`). Customer pages show a floating WhatsApp support button. Public catalog and WhatsApp checkout remain available without signing in.

The project vision and milestone scope are guided by [`docs/milestones.md`](docs/milestones.md). Store all project plans in `.kilo/plans/`.

## Admin setup (local development)

The API requires a JWT signing key and does not contain a default key or administrator password. Generate a private signing key (at least 32 random bytes), then store it in .NET user secrets:

```bash
dotnet user-secrets set --project backend/JerseyStore.Api/JerseyStore.Api.csproj "Authentication:Jwt:SigningKey" "<private-random-signing-key>"
dotnet user-secrets set --project backend/JerseyStore.Api/JerseyStore.Api.csproj "AdminBootstrap:Email" "<admin-email>"
dotnet user-secrets set --project backend/JerseyStore.Api/JerseyStore.Api.csproj "AdminBootstrap:Password" "<strong-admin-password>"
```

The API creates the configured administrator and `Admin` role idempotently at startup. After the account has been provisioned, remove the bootstrap email and password secrets; the Identity account and role remain in the database:

```bash
dotnet user-secrets remove --project backend/JerseyStore.Api/JerseyStore.Api.csproj "AdminBootstrap:Email"
dotnet user-secrets remove --project backend/JerseyStore.Api/JerseyStore.Api.csproj "AdminBootstrap:Password"
```

Run the app with `make dev`, then open http://localhost:3000/admin/login. The frontend uses `API_URL` (see `frontend/.env.example`) for server-side API requests. Admin JWTs are held in an HttpOnly cookie and admin API traffic is forwarded through the Next.js server.

Uploaded PNG, JPEG, and WebP files (maximum 5 MB) are written to `backend/JerseyStore.Api/uploads` by default and served from the API under `/uploads/`. Configure `ImageStorage__Directory` for a persistent writable directory in deployment; local disk must be persistent across application restarts and instances. Keep signing keys and bootstrap credentials in a secret store or environment variables, never in tracked settings.

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