Console.WriteLine("パラメーター検証でいつ例外をスローすべきかのデモ");

var downloader = new DownloadService();

try
{
    _ = downloader.DownloadFileAsync(string.Empty);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"同期的な ArgumentException を正しくキャッチしました: {ex.Message}");
}

public sealed class DownloadService
{
    private static readonly HttpClient httpClient = new();

    public Task<string> DownloadFileAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Url を空にすることはできません", nameof(url));

        return CoreDownloadAsync();

        async Task<string> CoreDownloadAsync()
        {
            return await httpClient
                .GetStringAsync(url, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
