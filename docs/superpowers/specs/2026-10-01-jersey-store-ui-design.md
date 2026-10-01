# Jersey Store UI Design

## Goal

Restyle the existing Jersey Store storefront using the dark sports-tech direction in the supplied reference, while keeping the shopping flow direct and uncluttered.

## Visual direction

- Use a near-black navy canvas and slightly lighter navy surfaces.
- Use electric lime for primary actions, selected controls, and small emphasis; reserve cyan for occasional details.
- Use compact, confident sans-serif typography and thin blue-gray dividers.
- Favor product photography and restrained jersey-image treatments over decorative cards, gradients, or extra marketing content.
- Keep contrast and focus states accessible, with responsive layouts for narrow screens.

## Catalog page

- Keep a compact header with the Jersey Store wordmark and a small set of relevant navigation/actions.
- Use one short hero with a concise matchday message and one primary action to the catalog. Do not add category/era tiles, brand-stat strips, invented shipping promises, or unrelated navigation destinations.
- Place the existing search, club, season, size, price, and sort controls directly above the catalog results. Retain reset, pending state, validation, result count, and pagination behavior.
- Present products in a responsive grid with image, club/season/kit metadata, price or sold-out state, and a clear hover/focus treatment.

## Product detail

- Apply the same color, typography, and border system to the existing product detail.
- Preserve its product description, stock-aware size selection, per-size price, sold-out state, and WhatsApp purchase link. Use lime only for the actionable purchase/selected state.

## Out of scope

- No new product categories, retro collection, favorites, cart, track-order, shipping claims, or custom-jersey flows.
- No backend/API changes or product-data changes.

## Validation

- Run frontend lint and production build.
- Start the frontend and inspect catalog and product detail at desktop and mobile widths, checking image rendering, filter usability, size selection, and WhatsApp URL behavior.
