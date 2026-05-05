using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3.5));

try
{
    // コンシューマー側: .WithCancellation() を使う
    await foreach (var dataChunk in FetchPaginatedDataAsync()
                                     .WithCancellation(cts.Token))
    {
        Console.WriteLine($"データを受信して処理しました: {dataChunk}");
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("ストリーム処理はキャンセルされたか、タイムアウトしました。");
}

// プロデューサー側: CancellationToken を受け取り、[EnumeratorCancellation] を追加する
static async IAsyncEnumerable<string> FetchPaginatedDataAsync(
    [EnumeratorCancellation] CancellationToken token = default)
{
    for (int page = 1; page <= 5; page++)
    {
        // トークンを受け取れるすべての非同期メソッドへ渡す
        await Task.Delay(1000, token);

        // 手動で確認することもできる
        // token.ThrowIfCancellationRequested();

        string dataChunk = $"これはページ {page} のデータです";
        yield return dataChunk;
    }
}
