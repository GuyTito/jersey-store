namespace JerseyStore.Api.Services;

public interface IAdminUserBootstrapper
{
    Task EnsureAdminAsync(CancellationToken cancellationToken = default);
}
