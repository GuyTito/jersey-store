Milestone 0: Setup

Goal: both apps run and talk to each other.

Create frontend/ (create-next-app, TypeScript, App Router) and backend/ (dotnet new webapi)
Add a GET /api/health endpoint, enable CORS for localhost:3000, and fetch it from a Next.js page

Done when: your Next.js page displays a message that came from C#.

Milestone 1: Read-only catalog with WhatsApp buy button

Goal: a working storefront with no database.

Create a Product class and hold 6 to 10 products in a hardcoded in-memory list
Add GET /api/products and GET /api/products/{slug}
Build a home/shop grid page and a product detail page in Next.js
"Buy on WhatsApp" opens https://wa.me/233XXXXXXXXX?text=<encoded message>. The number is in international format with no +, and the message is prefilled with the product name, size, and price.

C# you learn: controllers, routing, DTOs, dependency injection (put the list behind an IProductService interface right away, since you'll swap it in Milestone 2).

Milestone 2: Real database

Goal: products live in a database instead of code.

EF Core with SQLite (easy) or PostgreSQL, plus migrations
Entities: Product, ProductVariant (size and stock), ProductImage
Seed data, and swap your in-memory service for an EF Core one without touching the controllers

C# you learn: DbContext, relationships, Include, migrations, async/await, why interfaces pay off.

Milestone 3: Search, filter, pagination

Goal: a shop that still works with 200 jerseys.

Query params: ?club=&season=&size=&minPrice=&maxPrice=&sort=&page=
Return a paged response (items, totalCount, page)

C# you learn: IQueryable, composing queries conditionally, projections with Select.

Milestone 4: Admin login and product management

Goal: stop editing the database by hand.

ASP.NET Core Identity, JWT login, and an Admin role
Admin endpoints for product CRUD and stock updates, plus image upload (local folder)
Admin pages in Next.js behind a login

C# you learn: authentication vs authorization, [Authorize(Roles = "Admin")], FluentValidation, file uploads, global exception middleware.

I put admin before customer accounts on purpose: you need it now, and customers can still buy through WhatsApp without logging in.

Milestone 5: Cart

Goal: a customer can add multiple items and send them in one message.

Cart in Next.js state, saved to localStorage
The WhatsApp message lists every item with sizes, quantities, and the total
Milestone 6: Orders in your backend

Goal: every order is recorded, even while payment is still manual. This is the bridge to Paystack.

Order and OrderItem entities, with the price stored at time of purchase
POST /api/orders: the server recalculates prices from the database and never trusts the client
After creating the order, redirect to WhatsApp with the order number in the message
Admin can view orders and change status (Pending → Paid → Shipped → Delivered), with valid transitions enforced
Decrement stock inside a transaction

C# you learn: transactions, concurrency (two people buying the last size M), enums/state machines, business logic in the service layer.

Milestone 7: Customer accounts
Register and login for customers, "My orders" page, and saved addresses
Milestone 8: Paystack (test mode)
Typed HttpClient for Paystack, initialize → redirect → verify, webhook with signature validation, and idempotent handling
WhatsApp becomes optional support instead of the checkout

C# you learn: HttpClientFactory, the options pattern, HMAC, secrets management, idempotency.

Milestone 9: Polish and ship
xUnit tests for the order and payment logic, integration tests with WebApplicationFactory
Emails, Serilog logging, Docker Compose, deployment
