# Jersey Store

Milestone 0: a Next.js page fetches a health message from an ASP.NET Core API.

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