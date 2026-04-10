Console.WriteLine("示範：參數驗證的拋出時機");

var downloader = new DownloadService();

try
{
    _ = downloader.DownloadFileAsync(string.Empty);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"成功捕捉同步 ArgumentException: {ex.Message}");
}

public sealed class DownloadService
{
    private static readonly HttpClient httpClient = new();

    public Task<string> DownloadFileAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Url 不可為空", nameof(url));

        return CoreDownloadAsync();

        async Task<string> CoreDownloadAsync()
        {
            return await httpClient
                .GetStringAsync(url, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
