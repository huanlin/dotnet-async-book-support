using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// 使用 HttpClient 的最佳實務：宣告為單一實例且可重複使用，避免 Socket 耗盡
using var sharedClient = new HttpClient();

Console.WriteLine("示範 Task.WhenAny 實作超時機制");

try
{
    string result = await DownloadWithTimeoutAsync("https://example.com");
    Console.WriteLine($"下載成功，取得 {result.Length} 個字元。");
}
catch (TimeoutException ex)
{
    Console.WriteLine($"發生超時例外: {ex.Message}");
}
async Task<string> DownloadWithTimeoutAsync(string url)
{
    using var cts = new CancellationTokenSource();

    Task<string> downloadTask = sharedClient.GetStringAsync(url, cts.Token);
    Task timeoutTask = Task.Delay(3000); // 3秒超時

    // 等待兩者之一完成
    Task completedTask = await Task.WhenAny(downloadTask, timeoutTask);

    if (completedTask == timeoutTask)
    {
        // 超時了！取消下載工作並拋出例外
        cts.Cancel();
        throw new TimeoutException("下載作業超時。");
    }

    // 下載先完成，取得結果
    return await downloadTask;
}
