using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// 使用 HttpClient 時，宣告為單一實例且可重複使用，避免 Socket 耗盡
using var httpClient = new HttpClient();

Console.WriteLine("示範 Task.WhenAny 實作逾時機制");

try
{
    string result = await DownloadWithTimeoutAsync("https://example.com");
    Console.WriteLine($"下載成功，取得 {result.Length} 個字元。");
}
catch (TimeoutException ex)
{
    Console.WriteLine($"發生逾時例外: {ex.Message}");
}
async Task<string> DownloadWithTimeoutAsync(string url)
{
    using var cts = new CancellationTokenSource();

    Task<string> downloadTask = httpClient.GetStringAsync(url, cts.Token);
    Task timeoutTask = Task.Delay(3000, cts.Token); // 3秒逾時

    // 等待兩者之一完成
    Task completedTask = await Task.WhenAny(downloadTask, timeoutTask);

    if (completedTask == timeoutTask)
    {
        // 逾時了！取消下載工作並拋出例外
        cts.Cancel();
        throw new TimeoutException("下載作業逾時。");
    }

    // 下載先完成，順便取消 timeoutTask，避免它繼續無意義地計時
    cts.Cancel();

    // 下載先完成，取得結果
    return await downloadTask;
}
