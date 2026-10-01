# API integration tests

The API tests run migrations and seed deterministic fixtures in an isolated PostgreSQL database. Create a disposable database named `JerseyStoreTestDb`, then set `JERSEY_STORE_TEST_CONNECTION_STRING` to its connection string before running `dotnet test`.

The test host refuses to start if the connection string's database is not exactly `JerseyStoreTestDb`. Test setup removes only fixture products with IDs 100 and above; it does not delete or reset the database.

Do not commit connection strings or credentials. The normal development connection string targets `JerseyStoreDb` and is not suitable for these tests.