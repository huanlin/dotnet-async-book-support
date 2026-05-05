using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// .NET 10 では IAsyncEnumerable<T> 向けの LINQ 拡張メソッドが直接含まれている
await foreach (var item in FetchPaginatedDataAsync()
                 .Where(chunk => chunk.Contains("3") || chunk.Contains("5"))
                 .Select(chunk => chunk.ToUpper()))
{
    Console.WriteLine($"LINQ 処理後のデータ: {item}");
}

static async IAsyncEnumerable<string> FetchPaginatedDataAsync()
{
    for (int page = 1; page <= 5; page++)
    {
        await Task.Delay(500);
        yield return $"これはページ {page} のデータです";
    }
}
