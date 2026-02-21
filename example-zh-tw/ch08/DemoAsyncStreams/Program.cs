using System;
using System.Collections.Generic;
using System.Threading.Tasks;

Console.WriteLine("準備開始消費非同步資料流...");

// 使用 await foreach 來逐一處理資料流中的項目
await foreach (var dataChunk in FetchPaginatedDataAsync())
{
    Console.WriteLine($"收到並處理資料: {dataChunk}");
}

Console.WriteLine("資料流消費完畢。");

// 實作一個非同步迭代器
static async IAsyncEnumerable<string> FetchPaginatedDataAsync()
{
    for (int page = 1; page <= 5; page++)
    {
        // 模擬非同步的網路請求，耗時 1 秒
        await Task.Delay(1000);

        string dataChunk = $"這是第 {page} 頁的資料";

        // 使用 yield return 交出一個項目
        // 此時方法會暫停，直到消費者請求下一個項目
        yield return dataChunk;
    }
}
