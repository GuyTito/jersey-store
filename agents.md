# Jersey Store agent guidance

## Current project context

Jersey Store is a full-stack football jersey storefront with an ASP.NET Core API and a Next.js frontend.

**Current milestone: Milestone 5 — Cart (next). Milestone 4 — Admin login and product management is complete.**

The API serves a PostgreSQL-backed product catalog through EF Core, and the storefront supports product details, catalog search and filters, paged results, and ordering through WhatsApp. Milestone 4 added a role-protected admin area at `/admin` for product, variant, and stock management, plus local image uploads with gallery thumbnails, primary-image selection, and image removal, product search, and a settings page for the store WhatsApp number (stored in the database, falling back to `Store:WhatsAppNumber`). Customer pages show a floating WhatsApp support button. Public catalog and WhatsApp checkout remain available without signing in.

## Project guidelines

- The project vision and milestone scope are guided by `docs/milestones.md`.
- Store all project plans in `.kilo/plans/`.
- Keep the project state synchronized between `README.md` and `agents.md`. When the project status or current milestone changes, update both files in the same change so AI agents receive consistent context.

## Next.js-specific guidance

<!-- BEGIN:nextjs-agent-rules -->

This is NOT the Next.js you know.

This version has breaking changes — APIs, conventions, and file structure may all differ from your training data. Read the relevant guide in `node_modules/next/dist/docs/` (resolved from the frontend directory; in monorepos the `next` package may not be visible from the repository root) before writing any code. Heed deprecation notices.

This managed block is written and re-added by `next dev`; verify its source at `frontend/node_modules/next/dist/server/lib/generate-agent-files.js`.

<!-- END:nextjs-agent-rules -->
