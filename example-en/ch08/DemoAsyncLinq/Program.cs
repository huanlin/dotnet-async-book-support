using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// .NET 10 includes LINQ extension methods for IAsyncEnumerable<T> directly
await foreach (var item in FetchPaginatedDataAsync()
                 .Where(chunk => chunk.Contains("3") || chunk.Contains("5"))
                 .Select(chunk => chunk.ToUpper()))
{
    Console.WriteLine($"Data after LINQ processing: {item}");
}

static async IAsyncEnumerable<string> FetchPaginatedDataAsync()
{
    for (int page = 1; page <= 5; page++)
    {
        await Task.Delay(500);
        yield return $"This is the data for page {page}";
    }
}
