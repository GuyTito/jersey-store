namespace JerseyStore.Api.Services;

public interface IAdminImageStorage
{
    Task<string> StoreAsync(Stream content, string contentType, CancellationToken cancellationToken);
    Task DeleteAsync(string url, CancellationToken cancellationToken);
}
