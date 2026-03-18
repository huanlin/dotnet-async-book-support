using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// .NET 10 可直接使用 IAsyncEnumerable<T> 的 LINQ 擴充方法
await foreach (var item in FetchPaginatedDataAsync()
                 .Where(chunk => chunk.Contains("3") || chunk.Contains("5"))
                 .Select(chunk => chunk.ToUpper()))
{
    Console.WriteLine($"經過 LINQ 處理後的資料: {item}");
}

static async IAsyncEnumerable<string> FetchPaginatedDataAsync()
{
    for (int page = 1; page <= 5; page++)
    {
        await Task.Delay(500);
        yield return $"這是第 {page} 頁的資料";
    }
}
