using System;
using System.Threading;
using System.Threading.Tasks;

// 1. CancellationTokenSource を作成する。
using var cts = new CancellationTokenSource();

// 2. CTS から CancellationToken を取得する。
var token = cts.Token;

// 3. 非同期メソッドへトークンを渡す
// (すぐには await しない。バックグラウンドで実行できるようにするため)。
Task workTask = DoSomeLongRunningWorkAsync(token);

// ユーザーがキャンセルを決めるまで少し時間を使う状況をシミュレートする。
await Task.Delay(2500);

// 4. あとでキャンセルが必要になった時点で...
Console.WriteLine("\n[呼び出し元] ユーザーが操作をキャンセルすることにしました!");
cts.Cancel(); // 「キャンセル」ボタンを押す。

try
{
    await workTask; // バックグラウンド作業の完了を待つ。
}
catch (OperationCanceledException) 
{
    // これは想定どおり。
    Console.WriteLine("呼び出し元が OperationCanceledException をキャッチしました。");
}
catch (Exception ex)
{
    // こちらは本当のエラー ケース。
    Console.WriteLine($"作業が失敗しました: {ex.Message}");
}

static async Task DoSomeLongRunningWorkAsync(CancellationToken token)
{
    Console.WriteLine("バックグラウンド作業を開始しました...");
    try
    {
        for (int i = 0; i < 10; i++)
        {
            // キャンセルが要求されているか確認する。
            token.ThrowIfCancellationRequested();

            Console.WriteLine($"パート {i + 1}/10 を処理中...");
            // 重要: キャンセルをサポートする下位レベル API には、
            // 引き続きトークンを渡す。
            await Task.Delay(1000, token);
        }
        Console.WriteLine("バックグラウンド作業が正常に完了しました。");
    }
    catch (OperationCanceledException)
    {
        // ThrowIfCancellationRequested は OperationCanceledException をスローし、
        // Task.Delay などの API は、多くの場合その派生型である
        // TaskCanceledException をスローする。
        Console.WriteLine("バックグラウンド作業はキャンセルされました。");
        throw; // 通常は再スローすべき。
    }
}
