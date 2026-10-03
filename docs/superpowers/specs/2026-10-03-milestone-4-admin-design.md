# Milestone 4: Admin login and product management

## Goal

Allow an administrator to manage the Jersey Store catalog and stock without editing the database directly, while keeping the public storefront and customer WhatsApp checkout available without sign-in.

## Scope

- Add ASP.NET Core Identity user and role storage to the existing PostgreSQL-backed EF Core context.
- Authenticate administrators with JWT bearer tokens and authorize management APIs with an `Admin` role.
- Provide an explicit, repeatable initial-admin provisioning path whose credentials come only from secrets or environment configuration.
- Add admin APIs for product create, read, update, and delete; variant stock updates; and product image upload.
- Add Next.js admin login and product-management pages, with a server-side proxy that stores the API token in an HttpOnly cookie and forwards admin requests.
- Keep public catalog endpoints and customer checkout behavior unchanged.

Customer registration, customer accounts, orders, payment, and remote image hosting are out of scope.

## Design

### Backend authentication and authorization

Extend the current EF Core context with the Identity user and role stores and create a migration. Configure ASP.NET Core Identity and JWT bearer authentication in the API. The login endpoint validates credentials and returns a short-lived signed token with the user's role claims and expiration. Admin management endpoints require the `Admin` role; public catalog endpoints remain anonymous.

Provision the first administrator through an idempotent startup/bootstrap operation driven by required configuration values supplied through development secrets or deployment environment variables. Never add a default password or credential to tracked settings. Missing bootstrap configuration should not silently create a user.

### Admin catalog APIs and image storage

Add admin-only endpoints for product CRUD, variant stock changes, and image upload. Keep persistence and validation in the existing service/data layers rather than placing business rules in controllers. Validate product and variant inputs, reject invalid stock values, and return explicit client errors for invalid requests and not-found responses for missing products.

Store uploaded images under a configured local application upload directory, use generated file names rather than client-supplied paths, validate allowed image content and size, and return a stable URL for the product. Do not expose arbitrary filesystem paths. Report storage failures as errors rather than returning a successful response with a missing image.

### Next.js admin experience

Add an admin login page and protected product-management pages. A Next.js server-side route handler acts as a backend-for-frontend: it forwards login credentials to the API, stores the returned JWT in an HttpOnly cookie, and forwards subsequent admin API requests with the bearer token. The browser never reads or persists the JWT itself. Clear the cookie on logout and on expired/invalid authentication. Apply same-origin checks to state-changing proxy requests to mitigate cross-site request forgery.

The admin UI provides product listing and create/edit/delete flows, stock updates, image upload, and visible loading and API-error states. Existing storefront routes and public catalog calls do not acquire an authentication requirement.

## Delivery sequence

1. Add Identity/JWT dependencies and configuration, Identity persistence and migration, initial-admin provisioning, and login endpoint.
2. Add product administration and image upload APIs with role protection, validation, and integration tests.
3. Add the Next.js login/proxy and cookie lifecycle, then protected product-management pages and admin interaction states.
4. Run targeted API and frontend checks; verify public catalog behavior remains unchanged and document required configuration and local image-storage behavior.

## Acceptance criteria

- Valid administrator credentials produce a JWT; invalid credentials do not disclose whether the user or password was incorrect.
- Anonymous requests to admin APIs are rejected, and authenticated non-admin users are forbidden.
- An administrator can create, read, update, and delete products, update variant stock, and upload a valid image; invalid input and upload failures return visible errors.
- The Next.js admin flow keeps the JWT in an HttpOnly cookie, uses the server-side proxy for protected requests, and supports login/logout and expired-token handling.
- Public product listing and detail pages continue to work without authentication.
- Automated tests cover login outcomes, authorization boundaries, product/stock operations, image validation/storage errors, and the relevant frontend admin behavior.
