using System.Text.RegularExpressions;
using JerseyStore.Api.Data;
using JerseyStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JerseyStore.Api.Services;

public interface IStoreSettingsService
{
    Task<string> GetWhatsAppNumberAsync(CancellationToken cancellationToken);
    Task<string?> SetWhatsAppNumberAsync(string input, CancellationToken cancellationToken);
}

public sealed partial class StoreSettingsService(
    ApplicationDbContext dbContext,
    IConfiguration configuration) : IStoreSettingsService
{
    public async Task<string> GetWhatsAppNumberAsync(CancellationToken cancellationToken)
    {
        var stored = await dbContext.StoreSettings
            .AsNoTracking()
            .Where(setting => setting.Key == StoreSetting.WhatsAppNumberKey)
            .Select(setting => setting.Value)
            .FirstOrDefaultAsync(cancellationToken);

        return stored
            ?? configuration["Store:WhatsAppNumber"]
            ?? throw new InvalidOperationException("Store:WhatsAppNumber must be configured.");
    }

    /// <returns>The normalized number, or null when the input is not a valid international number.</returns>
    public async Task<string?> SetWhatsAppNumberAsync(string input, CancellationToken cancellationToken)
    {
        var normalized = Normalize(input);
        if (normalized is null)
        {
            return null;
        }

        var setting = await dbContext.StoreSettings.FindAsync([StoreSetting.WhatsAppNumberKey], cancellationToken);
        if (setting is null)
        {
            dbContext.StoreSettings.Add(new StoreSetting
            {
                Key = StoreSetting.WhatsAppNumberKey,
                Value = normalized,
            });
        }
        else
        {
            setting.Value = normalized;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return normalized;
    }

    private static string? Normalize(string input)
    {
        var compact = SeparatorPattern().Replace(input.Trim(), "");
        return InternationalNumberPattern().IsMatch(compact) ? compact : null;
    }

    [GeneratedRegex(@"[\s\-().]")]
    private static partial Regex SeparatorPattern();

    [GeneratedRegex(@"^\+[1-9]\d{7,14}$")]
    private static partial Regex InternationalNumberPattern();
}
