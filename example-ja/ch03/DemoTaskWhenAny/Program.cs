using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// ソケット枯渇を避けるため、HttpClient インスタンスを 1 つだけ再利用する
using var httpClient = new HttpClient();

Console.WriteLine("Task.WhenAny によるタイムアウト制御のデモ");

try
{
    string result = await DownloadWithTimeoutAsync("https://example.com");
    Console.WriteLine($"ダウンロードに成功しました。{result.Length} 文字を取得しました。");
}
catch (TimeoutException ex)
{
    Console.WriteLine($"タイムアウト例外が発生しました: {ex.Message}");
}
async Task<string> DownloadWithTimeoutAsync(string url)
{
    using var cts = new CancellationTokenSource();

    Task<string> downloadTask = httpClient.GetStringAsync(url, cts.Token);
    Task timeoutTask = Task.Delay(3000, cts.Token); // 3 秒のタイムアウト

    // どちらか一方が完了するまで待つ
    Task completedTask = await Task.WhenAny(downloadTask, timeoutTask);

    if (completedTask == timeoutTask)
    {
        // タイムアウトした。ダウンロードをキャンセルし、例外をスローする。
        cts.Cancel();
        throw new TimeoutException("ダウンロード操作がタイムアウトしました。");
    }

    // ダウンロードが先に完了した。timeoutTask が無意味にカウントし続けないようにキャンセルする。
    cts.Cancel();

    // ダウンロードが先に完了したので、結果を返す。
    return await downloadTask;
}
