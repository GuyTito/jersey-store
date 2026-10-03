using JerseyStore.Api.Contracts;

namespace JerseyStore.Api.Services;

public interface IAdminAuthService
{
    Task<AdminLoginResponse?> LoginAsync(AdminLoginRequest request, CancellationToken cancellationToken);
}
