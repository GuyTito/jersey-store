# Jersey Store: Milestone 0 Plan

**Goal:** Both apps run locally and communicate. Milestone 0 is complete when a Next.js page displays a message returned by a C# endpoint.

This folder is a self-contained copy of the Milestone 0 setup plan. It does not scaffold or run the apps.

## 1. Check versions and prerequisites

Versions checked on **September 29, 2026**:

- **.NET SDK 10.0.401** and **ASP.NET Core 10.0.12**: current stable versions listed on Microsoft's [.NET 10 downloads page](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). .NET 11 is RC1, so use .NET 10, the current LTS release, rather than a release candidate. Microsoft's [.NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core) lists .NET 10 as LTS.
- **Node.js 24.21.0 LTS**: listed on the [Node.js downloads page](https://nodejs.org/en/download). Node 26 is newer but is Current, not LTS; Node 24 is the LTS choice.
- **Next.js 16.3.6**: the current stable patch on the 16.x Active LTS line. The [September 22 security update](https://nextjs.org/blog/nextjs-security-update-september-22-2026) says 16.3.6 is available. The [Next.js blog](https://nextjs.org/blog) announces 16.3.7 for September 30, after this version check.

Install .NET SDK 10, Node.js 24 LTS, Git, and a modern browser. The ASP.NET Core runtime is included with the SDK. Verify in PowerShell:

```powershell
dotnet --version
dotnet --list-sdks
dotnet --list-runtimes
node --version
npm --version
git --version
```

Expect the active SDK to report `10.0.401`, runtimes to include `Microsoft.AspNetCore.App 10.0.12`, and Node to report `v24.21.0`. On macOS/Linux, the same verification commands work; use the platform's .NET installer and, optionally, a Node version manager such as `nvm`. On Windows, use the Node installer or a Windows-compatible version manager.

**What and why:** Matching the SDK, runtime, and target framework keeps the C# project on a supported release line. Checking tools first separates installation problems from code problems.

## 2. Initialize the repository and create the backend solution

Run from the parent folder where you want the repository:

```powershell
mkdir jersey-store
cd jersey-store
git init

mkdir backend
cd backend
dotnet new sln --name JerseyStore --format sln
dotnet new webapi --name JerseyStore.Api --output JerseyStore.Api --use-controllers --framework net10.0 --no-https --kestrelHttpPort 5080
dotnet sln JerseyStore.sln add JerseyStore.Api/JerseyStore.Api.csproj
cd ..
```

The explicit `--format sln` matters because the .NET 10 SDK defaults to `.slnx`, while this project requires `JerseyStore.sln`. The Web API template defaults to Minimal APIs, so `--use-controllers` explicitly selects controllers. Remove the template's sample weather files:

```powershell
Remove-Item .\backend\JerseyStore.Api\Controllers\WeatherForecastController.cs, .\backend\JerseyStore.Api\WeatherForecast.cs
```

On macOS/Linux, use:

```bash
rm backend/JerseyStore.Api/Controllers/WeatherForecastController.cs backend/JerseyStore.Api/WeatherForecast.cs
```

**What and why:** The solution contains one API project now, leaving room to add layers later without adding them prematurely. Controllers teach attribute routing and action methods; this project uses HTTP only on port `5080` for a simpler local CORS setup.

## 3. Add the health endpoint, CORS policy, and OpenAPI

Replace `backend/JerseyStore.Api/Program.cs` with:

```csharp
const string FrontendCorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);

var frontendOrigin = builder.Configuration["Cors:AllowedOrigin"]
    ?? throw new InvalidOperationException("Cors:AllowedOrigin must be configured.");

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseCors(FrontendCorsPolicy);
app.UseAuthorization();

app.MapControllers();

app.Run();
```

Set `backend/JerseyStore.Api/appsettings.Development.json` to:

```json
{
  "Cors": {
    "AllowedOrigin": "http://localhost:3000"
  }
}
```

Replace `backend/JerseyStore.Api/Properties/launchSettings.json` with:

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "JerseyStore.Api": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5080",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

Create `backend/JerseyStore.Api/Controllers/HealthController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace JerseyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<HealthResponse>(StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> Get()
    {
        return Ok(new HealthResponse(
            "ok",
            "Jersey Store API is running.",
            DateTimeOffset.UtcNow));
    }
}

public sealed record HealthResponse(
    string Status,
    string Message,
    DateTimeOffset Timestamp);
```

The endpoint returns JSON at `GET http://localhost:5080/api/health`. The Web API template should include the built-in OpenAPI package and project reference; keep `AddOpenApi` and `MapOpenApi` enabled. The generated document is at `http://localhost:5080/openapi/v1.json`. The current template generates the document but does **not** include Swagger UI; open the JSON URL directly or load it into an API client.

**What and why:** `Route` and `HttpGet` map a controller action to a URL. `AddCors` registers a named policy using environment-based configuration, and `UseCors` runs after routing and before authorization so middleware can apply that policy to matched endpoints. `AddControllers` and `AddOpenApi` register framework services through ASP.NET Core's dependency injection container.

## 4. Create the Next.js frontend and connect it in the browser

From the repository root:

```powershell
npx create-next-app@latest frontend --ts --app --tailwind --eslint --no-src-dir --use-npm --disable-git
cd frontend
npm install next@16.3.6
```

This selects TypeScript, App Router, Tailwind, ESLint, and no `src/` directory. No `src/` is recommended here: keeping `app/` beside configuration files makes the App Router conventions visible without adding another folder level. `--disable-git` leaves Git initialized at the repository root.

Create `frontend/.env.local` and `frontend/.env.example`, each containing:

```dotenv
NEXT_PUBLIC_API_URL=http://localhost:5080
```

Replace `frontend/app/page.tsx` with:

```tsx
"use client";

import { useEffect, useState } from "react";

type HealthResponse = {
  status: string;
  message: string;
  timestamp: string;
};

type HealthState =
  | { status: "loading" }
  | { status: "success"; data: HealthResponse }
  | { status: "error"; message: string };

export default function Home() {
  const [healthState, setHealthState] = useState<HealthState>({
    status: "loading",
  });

  useEffect(() => {
    const controller = new AbortController();

    async function loadHealth() {
      try {
        const apiUrl = process.env.NEXT_PUBLIC_API_URL;

        if (!apiUrl) {
          throw new Error("NEXT_PUBLIC_API_URL is not configured.");
        }

        const response = await fetch(`${apiUrl}/api/health`, {
          signal: controller.signal,
        });

        if (!response.ok) {
          throw new Error(`API responded with HTTP ${response.status}.`);
        }

        const data = (await response.json()) as HealthResponse;
        setHealthState({ status: "success", data });
      } catch (error) {
        if (controller.signal.aborted) {
          return;
        }

        setHealthState({
          status: "error",
          message: error instanceof Error ? error.message : "Request failed.",
        });
      }
    }

    void loadHealth();

    return () => controller.abort();
  }, []);

  return (
    <main className="mx-auto max-w-2xl p-8">
      <h1 className="text-3xl font-bold">Jersey Store</h1>
      <p className="mt-2 text-gray-600">Backend connection check</p>

      <section className="mt-8 rounded border p-6" aria-live="polite">
        {healthState.status === "loading" && <p>Contacting the API...</p>}

        {healthState.status === "success" && (
          <div>
            <p className="font-semibold">{healthState.data.message}</p>
            <p className="mt-2">Status: {healthState.data.status}</p>
            <p className="mt-1 text-sm text-gray-600">
              Timestamp: {new Date(healthState.data.timestamp).toLocaleString()}
            </p>
          </div>
        )}

        {healthState.status === "error" && (
          <p role="alert" className="text-red-700">
            Could not reach the API: {healthState.message}
          </p>
        )}
      </section>
    </main>
  );
}
```

If `.env.local` changes after the frontend starts, stop and restart its dev server so Next.js reloads the value. `NEXT_PUBLIC_` variables are included in browser code, so use this only for a non-secret URL.

**What and why:** `"use client"` makes the fetch run in the browser, where the browser enforces CORS. The state union gives the page distinct loading, success, and error views rather than hiding network failures.

## 5. Add root-level ignore rules, README, and confirm the structure

Create the root `.gitignore`:

```gitignore
# Node.js / Next.js
node_modules/
.next/
out/
coverage/
.env
.env.*
!.env.example
!**/.env.example

# .NET
**/bin/
**/obj/
.vs/
*.user
*.suo

# Editors and operating systems
.idea/
.DS_Store
Thumbs.db
```

Create the root `README.md`:

````markdown
# Jersey Store

Milestone 0: a Next.js page fetches a health message from an ASP.NET Core API.

## Run the backend

From the repository root:

```powershell
dotnet run --project backend/JerseyStore.Api/JerseyStore.Api.csproj --launch-profile JerseyStore.Api
```
````

API: http://localhost:5080/api/health  
OpenAPI document: http://localhost:5080/openapi/v1.json

## Run the frontend

In a second terminal, from the repository root:

```powershell
cd frontend
npm run dev
```

Open http://localhost:3000. The page fetches the API in the browser.

````

Expected relevant tree; generated Next.js config and static files remain in `frontend/`:

```text
jersey-store/
├── .gitignore
├── README.md
├── backend/
│   ├── JerseyStore.sln
│   └── JerseyStore.Api/
│       ├── Controllers/
│       │   └── HealthController.cs
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── appsettings.Development.json
│       ├── appsettings.json
│       ├── JerseyStore.Api.csproj
│       └── Program.cs
└── frontend/
    ├── app/
    │   ├── globals.css
    │   ├── layout.tsx
    │   └── page.tsx
    ├── public/
    ├── .env.example
    ├── .env.local
    ├── package.json
    ├── package-lock.json
    ├── next.config.ts
    ├── postcss.config.mjs
    └── tsconfig.json
````

Suggested first commit and tag after verification:

```powershell
git add .
git commit -m "chore: complete milestone 0"
git tag v0.0
```

**What and why:** The root ignore file prevents build output and local environment settings from entering Git while keeping `.env.example` tracked. The README records the two-process development workflow; the solution stays intentionally limited to one project.

## 6. Verify the milestone

1. From the repository root, start the backend:

   ```powershell
   dotnet run --project backend/JerseyStore.Api/JerseyStore.Api.csproj --launch-profile JerseyStore.Api
   ```

2. Open `http://localhost:5080/api/health` in a browser. Confirm a JSON response with `status`, `message`, and `timestamp`.

3. In a second terminal, start the frontend:

   ```powershell
   cd frontend
   npm run dev
   ```

4. Open `http://localhost:3000`. Confirm the page displays the C# message and status.

5. To prove CORS is enabled, open browser Developer Tools → **Network**, select the `health` request, and inspect its response headers. It should include `Access-Control-Allow-Origin: http://localhost:3000`. Alternatively, inspect the header with:

   ```powershell
   curl.exe -i -H "Origin: http://localhost:3000" http://localhost:5080/api/health
   ```

   A direct address-bar visit to the API does **not** test CORS; the cross-origin browser `fetch` does. If CORS is misconfigured, the browser console commonly reports the request was blocked because there is no matching `Access-Control-Allow-Origin` header, and the page displays its error state.

**What and why:** The endpoint check confirms routing and serialization; the page check confirms the browser-to-API path. Inspecting browser request and response headers distinguishes a CORS policy problem from an unavailable API.

## 7. Troubleshoot common setup issues

- **HTTPS certificate warning:** This plan uses HTTP only, so no HTTPS development certificate is needed. If HTTPS is later enabled, run `dotnet dev-certs https --trust`; certificate trust steps vary by OS.
- **CORS error:** Confirm `Cors:AllowedOrigin` in `appsettings.Development.json` exactly matches the browser origin, including scheme and port (`http://localhost:3000`, with no trailing slash). Confirm `UseCors` is after `UseRouting` and before `UseAuthorization`.
- **Port mismatch or address in use:** Make sure `launchSettings.json`, `NEXT_PUBLIC_API_URL`, and the browser URL all use port `5080`. If another process owns that port, choose another port consistently in all three places.
- **Environment variable not picked up:** Confirm the file is named `frontend/.env.local`, the key is `NEXT_PUBLIC_API_URL`, and the Next.js dev server was restarted after editing it.
- **API returns 404 or fails to start:** Check that `HealthController` is under `Controllers/`, its route is `api/[controller]`, the request is `GET /api/health`, and the backend is running with the `JerseyStore.Api` launch profile.

**What and why:** These problems usually come from mismatched configuration or request pipeline setup. Checking the actual origin, port, and active environment narrows the diagnosis quickly.

---

This plan stops at Milestone 0: no database, products, authentication, cart, orders, or payments.
