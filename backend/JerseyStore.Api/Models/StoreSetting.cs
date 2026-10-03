namespace JerseyStore.Api.Models;

public sealed class StoreSetting
{
    public const string WhatsAppNumberKey = "WhatsAppNumber";

    public required string Key { get; set; }

    public required string Value { get; set; }
}
