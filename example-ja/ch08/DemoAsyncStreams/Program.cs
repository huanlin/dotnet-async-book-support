using System;
using System.Collections.Generic;
using System.Threading.Tasks;

Console.WriteLine("非同期ストリームを消費する準備をしています...");

// await foreach でストリーム内の各項目を処理する
await foreach (var dataChunk in FetchPaginatedDataAsync())
{
    Console.WriteLine($"データを受信して処理しました: {dataChunk}");
}

Console.WriteLine("ストリームは最後まで消費されました。");

// 非同期イテレーターを実装する
static async IAsyncEnumerable<string> FetchPaginatedDataAsync()
{
    for (int page = 1; page <= 5; page++)
    {
        // 1 秒かかる非同期ネットワーク リクエストをシミュレートする
        await Task.Delay(1000);

        string dataChunk = $"これはページ {page} のデータです";

        // 1 つの項目を yield する
        // コンシューマーが次の項目を要求するまで、メソッドはここで一時停止する
        yield return dataChunk;
    }
}
