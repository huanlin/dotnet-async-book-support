using System;
using System.Threading;
using System.Threading.Tasks;

// 3 秒後に自動的にキャンセルされる CTS を作成する。
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

try
{
    Console.WriteLine("最大 3 秒まで実行できるジョブを開始します...");
    // この作業は内部的に 10 秒かかる。
    await DoSomeLongRunningWorkAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("タイムアウトしたため、作業はキャンセルされました!");
}

static async Task DoSomeLongRunningWorkAsync(CancellationToken token)
{
    Console.WriteLine("バックグラウンド作業を開始しました...");
    for (int i = 0; i < 10; i++)
    {
        token.ThrowIfCancellationRequested();
        Console.WriteLine($"パート {i + 1}/10 を処理中...");
        await Task.Delay(1000, token);
    }
    Console.WriteLine("バックグラウンド作業が正常に完了しました。");
}
