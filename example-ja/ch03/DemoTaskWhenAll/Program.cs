using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

// ソケット枯渇を避けるため、HttpClient インスタンスを 1 つだけ再利用する
using var httpClient = new HttpClient();

Console.WriteLine("Task.WhenAll で複数のタスクを非同期に待機するデモ");

var sw = Stopwatch.StartNew();
await ConcurrentDownloadAsync();
sw.Stop();

Console.WriteLine($"合計経過時間: {sw.ElapsedMilliseconds} ms");

// ✓ 正しい例: より効率的な並行実行
async Task ConcurrentDownloadAsync()
{
    // 1. すべてのタスクを開始する (この例では、並行性を示すためだけに同じ URL へ 3 つのリクエストを送る)
    Task<string> task1 = httpClient.GetStringAsync("https://ippobooks.com/");
    Task<string> task2 = httpClient.GetStringAsync("https://ippobooks.com/");
    Task<string> task3 = httpClient.GetStringAsync("https://ippobooks.com/");

    // 2. すべてが完了するのを待つ
    string[] results = await Task.WhenAll(task1, task2, task3);
    
    Console.WriteLine($"すべてのダウンロードが完了しました。合計 {results.Length} 件の結果を取得しました。");
}
