using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// Reuse a single HttpClient instance to avoid socket exhaustion
using var httpClient = new HttpClient();

Console.WriteLine("Demonstrating timeout control with Task.WhenAny");

try
{
    string result = await DownloadWithTimeoutAsync("https://example.com");
    Console.WriteLine($"Download succeeded. Retrieved {result.Length} characters.");
}
catch (TimeoutException ex)
{
    Console.WriteLine($"A timeout exception occurred: {ex.Message}");
}
async Task<string> DownloadWithTimeoutAsync(string url)
{
    using var cts = new CancellationTokenSource();

    Task<string> downloadTask = httpClient.GetStringAsync(url, cts.Token);
    Task timeoutTask = Task.Delay(3000, cts.Token); // 3-second timeout

    // Wait until either one completes
    Task completedTask = await Task.WhenAny(downloadTask, timeoutTask);

    if (completedTask == timeoutTask)
    {
        // Timed out. Cancel the download and throw an exception.
        cts.Cancel();
        throw new TimeoutException("The download operation timed out.");
    }

    // The download finished first. Cancel timeoutTask so it does not keep counting meaninglessly.
    cts.Cancel();

    // The download finished first, so return the result.
    return await downloadTask;
}
