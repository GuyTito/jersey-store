# API integration tests

The API tests run migrations and seed deterministic fixtures in an isolated PostgreSQL database. Create a disposable database named `JerseyStoreTestDb`, then set `JERSEY_STORE_TEST_CONNECTION_STRING` to its connection string before running `dotnet test`.

The test host refuses to start if the connection string's database is not exactly `JerseyStoreTestDb`. Test setup removes product records with IDs above 10 before inserting deterministic fixtures; it does not delete or reset the database. Admin authentication tests use test-only in-memory signing and bootstrap credentials, and image uploads use a unique temporary directory.

Do not commit connection strings or credentials. The normal development connection string targets `JerseyStoreDb` and is not suitable for these tests.
