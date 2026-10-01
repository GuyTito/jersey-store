.PHONY: dev dev-backend dev-frontend

dev:
	@trap 'kill %1 %2; wait' INT TERM; \
	dotnet run --project backend/JerseyStore.Api/JerseyStore.Api.csproj --launch-profile JerseyStore.Api & \
	pnpm --filter frontend dev & \
	wait

dev-backend:
	dotnet run --project backend/JerseyStore.Api/JerseyStore.Api.csproj --launch-profile JerseyStore.Api

dev-frontend:
	pnpm --filter frontend dev