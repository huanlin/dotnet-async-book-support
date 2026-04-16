Console.WriteLine("Demonstrating when parameter validation should throw");

var downloader = new DownloadService();

try
{
    _ = downloader.DownloadFileAsync(string.Empty);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Successfully caught the synchronous ArgumentException: {ex.Message}");
}

public sealed class DownloadService
{
    private static readonly HttpClient httpClient = new();

    public Task<string> DownloadFileAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Url cannot be empty", nameof(url));

        return CoreDownloadAsync();

        async Task<string> CoreDownloadAsync()
        {
            return await httpClient
                .GetStringAsync(url, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
