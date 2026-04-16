using System;
using System.Net.Http;
using System.Threading.Tasks;

// Reuse a single HttpClient instance to avoid socket exhaustion.
using var httpClient = new HttpClient();

try
{
    string url = "https://this-host-does-not-exist.invalid";
    string content = await DownloadPageAsync(url);
    Console.WriteLine("Download succeeded!");
}
catch (HttpRequestException ex)
{
    Console.WriteLine("A network error occurred:");
    Console.WriteLine(ex.Message);
}

async Task<string> DownloadPageAsync(string url)
{
    // HttpClient may throw HttpRequestException if the request fails
    // or the host name cannot be resolved.
    string content = await httpClient.GetStringAsync(url);
    return content;
}
