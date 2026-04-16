using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

// Reuse a single HttpClient instance to avoid socket exhaustion
using var httpClient = new HttpClient();

Console.WriteLine("Demonstrating Task.WhenAll to await multiple tasks asynchronously");

var sw = Stopwatch.StartNew();
await ConcurrentDownloadAsync();
sw.Stop();

Console.WriteLine($"Total elapsed time: {sw.ElapsedMilliseconds} ms");

// ✓ Correct example: concurrent execution with better efficiency
async Task ConcurrentDownloadAsync()
{
    // 1. Start all tasks (this example sends three requests to the same URL only to demonstrate concurrency)
    Task<string> task1 = httpClient.GetStringAsync("https://ippobooks.com/");
    Task<string> task2 = httpClient.GetStringAsync("https://ippobooks.com/");
    Task<string> task3 = httpClient.GetStringAsync("https://ippobooks.com/");

    // 2. Wait for all of them to complete
    string[] results = await Task.WhenAll(task1, task2, task3);
    
    Console.WriteLine($"All downloads have completed. Retrieved {results.Length} results in total.");
}
