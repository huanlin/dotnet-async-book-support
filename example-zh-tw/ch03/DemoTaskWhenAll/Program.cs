using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

// 使用 HttpClient 時，宣告為單一實例且可重複使用，避免 Socket 耗盡
using var sharedClient = new HttpClient();

Console.WriteLine("示範 Task.WhenAll 同步等待多個工作");

var sw = Stopwatch.StartNew();
await ConcurrentDownloadAsync();
sw.Stop();

Console.WriteLine($"總耗時: {sw.ElapsedMilliseconds} ms");

// ✓ 正確示範：併發執行，效率高
async Task ConcurrentDownloadAsync()
{
    // 1. 啟動所有工作 (Fire tasks)
    Task<string> task1 = sharedClient.GetStringAsync("https://example.com/");
    Task<string> task2 = sharedClient.GetStringAsync("https://example.com/");
    Task<string> task3 = sharedClient.GetStringAsync("https://example.com/");

    // 2. 等待全部完成 (Await all)
    string[] results = await Task.WhenAll(task1, task2, task3);
    
    Console.WriteLine($"所有下載皆已完成，共取得 {results.Length} 個結果。");
}
