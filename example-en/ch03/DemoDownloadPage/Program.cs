using System.Net.Http;
using System.Threading.Tasks;

// Reuse a single HttpClient instance to avoid socket exhaustion
using var httpClient = new HttpClient();

Console.WriteLine("Using async/await to download a web page and count its characters");

string url = "https://example.com";
int count = await DownloadPageAndCountCharsAsync(url);

Console.WriteLine($"The character count for {url} is: {count}");

async Task<int> DownloadPageAndCountCharsAsync(string url)
{
    // 1. Wait asynchronously for the download to complete, without blocking the thread
    string content = await httpClient.GetStringAsync(url);
    // 2. Once the download is done, execution continues from here
    return content.Length;
}
