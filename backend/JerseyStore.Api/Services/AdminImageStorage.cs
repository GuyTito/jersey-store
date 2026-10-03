using System.Buffers.Binary;

namespace JerseyStore.Api.Services;

public sealed class AdminImageStorage(
    IWebHostEnvironment environment,
    IConfiguration configuration,
    ILogger<AdminImageStorage> logger) : IAdminImageStorage
{
    private const int MaximumImageBytes = 5 * 1024 * 1024;
    private static readonly IReadOnlyDictionary<string, (string Extension, byte[] Signature)> SupportedTypes =
        new Dictionary<string, (string, byte[])>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/png"] = (".png", [0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a]),
            ["image/jpeg"] = (".jpg", [0xff, 0xd8, 0xff]),
            ["image/webp"] = (".webp", [0x52, 0x49, 0x46, 0x46]),
        };

    private readonly string _directory = Path.GetFullPath(
        configuration["ImageStorage:Directory"]
        ?? Path.Combine(environment.ContentRootPath, "uploads"));

    public async Task<string> StoreAsync(
        Stream content,
        string contentType,
        CancellationToken cancellationToken)
    {
        if (!SupportedTypes.TryGetValue(contentType, out var supported))
        {
            throw new InvalidDataException("Only PNG, JPEG, and WebP images are accepted.");
        }

        await using var buffer = new MemoryStream();
        var chunk = new byte[81920];
        int read;
        while ((read = await content.ReadAsync(chunk, cancellationToken)) > 0)
        {
            if (buffer.Length + read > MaximumImageBytes)
            {
                throw new InvalidDataException("The image exceeds the 5 MB upload limit.");
            }

            await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken);
        }

        var bytes = buffer.ToArray();
        if (!HasExpectedSignature(bytes, contentType, supported.Signature))
        {
            throw new InvalidDataException("The uploaded content does not match its declared image type.");
        }

        Directory.CreateDirectory(_directory);
        var fileName = $"{Guid.NewGuid():N}{supported.Extension}";
        var path = Path.Combine(_directory, fileName);
        try
        {
            await using var output = new FileStream(
                path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await output.WriteAsync(bytes, cancellationToken);
        }
        catch
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception cleanupException)
            {
                logger.LogError(cleanupException, "Failed to remove incomplete image upload {ImagePath}.", path);
            }

            throw;
        }

        return $"/uploads/{fileName}";
    }

    public Task DeleteAsync(string url, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!Uri.TryCreate(url, UriKind.Relative, out _) ||
            !url.StartsWith("/uploads/", StringComparison.Ordinal) ||
            Path.GetFileName(url) != url["/uploads/".Length..])
        {
            throw new InvalidOperationException("Refusing to delete a file outside the configured upload directory.");
        }

        var fileName = url["/uploads/".Length..];
        if (fileName.Length != 36 ||
            !Guid.TryParseExact(Path.GetFileNameWithoutExtension(fileName), "N", out _) ||
            Path.GetExtension(fileName) is not (".png" or ".jpg" or ".webp"))
        {
            throw new InvalidOperationException("Refusing to delete an unmanaged upload file.");
        }

        var path = Path.Combine(_directory, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    private static bool HasExpectedSignature(byte[] bytes, string contentType, byte[] signature)
    {
        if (bytes.Length < signature.Length ||
            !bytes.AsSpan(0, signature.Length).SequenceEqual(signature))
        {
            return false;
        }

        return contentType.ToLowerInvariant() switch
        {
            "image/png" => bytes.Length >= 33 &&
                bytes.AsSpan(12, 4).SequenceEqual("IHDR"u8) &&
                bytes.AsSpan(bytes.Length - 8, 4).SequenceEqual("IEND"u8) &&
                BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(16, 4)) > 0 &&
                BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(20, 4)) > 0,
            "image/jpeg" => bytes.Length >= 4 && bytes[^2] == 0xff && bytes[^1] == 0xd9,
            "image/webp" => bytes.Length >= 30 &&
                bytes.AsSpan(8, 4).SequenceEqual("WEBP"u8) &&
                BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4, 4)) <= bytes.Length - 8,
            _ => false,
        };
    }
}
