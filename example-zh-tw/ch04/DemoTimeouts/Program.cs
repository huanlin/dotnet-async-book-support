using System;
using System.Threading;
using System.Threading.Tasks;

// 建立一個 3 秒後會自動取消的 CTS
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

try
{
    Console.WriteLine("開始執行一個最多只能跑 3 秒的工作...");
    // 這個工作內部需要跑 10 秒
    await DoSomeLongRunningWorkAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("工作因為逾時而被取消了！");
}

static async Task DoSomeLongRunningWorkAsync(CancellationToken token)
{
    // 模擬一個需要跑 10 秒的工作
    for (int i = 0; i < 10; i++)
    {
        token.ThrowIfCancellationRequested();
        Console.WriteLine($"處理中... {i + 1}/10");
        await Task.Delay(1000, token);
    }
    Console.WriteLine("工作完成。");
}
