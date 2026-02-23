using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3.5));

try
{
    // 消費端：使用 .WithCancellation()
    await foreach (var dataChunk in FetchPaginatedDataAsync(cts.Token)
                                     .WithCancellation(cts.Token))
    {
        Console.WriteLine($"收到並處理資料: {dataChunk}");
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("資料流處理因為取消或逾時而中止。");
}

// 生產端：接受 CancellationToken 並加上 [EnumeratorCancellation] 屬性
static async IAsyncEnumerable<string> FetchPaginatedDataAsync(
    [EnumeratorCancellation] CancellationToken token = default)
{
    for (int page = 1; page <= 5; page++)
    {
        // 將 token 傳遞給所有可接受它的非同步方法
        await Task.Delay(1000, token);

        // 也可以手動檢查
        // token.ThrowIfCancellationRequested();

        string dataChunk = $"這是第 {page} 頁的資料";
        yield return dataChunk;
    }
}
