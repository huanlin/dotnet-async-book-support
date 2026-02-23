using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

// 使用 HttpClient 時，宣告為單一實例且可重複使用，避免 Socket 耗盡
using var httpClient = new HttpClient();

Console.WriteLine("示範 Task.Wait() 拋出 AggregateException");

var faultyTask = DownloadPageAsync("https://this-url-does-not-exist.com");

try
{
    // 使用 .Wait() 會拋出 AggregateException
    faultyTask.Wait();
}
catch (AggregateException ex)
{
    // 我們需要從 .InnerExceptions 集合中解開真正的例外
    var realException = ex.InnerExceptions.First();
    Console.WriteLine($"捕捉到 AggregateException，真正的錯誤是：{realException.GetType().Name}");
}

Console.WriteLine("\n示範 Task.WhenAll 多重例外使用 await (只拋出第一個)");

try
{
    var task1 = ThrowAsync("Error 1");
    var task2 = ThrowAsync("Error 2");
    await Task.WhenAll(task1, task2);
}
catch (Exception ex)
{
    // 這裡只會捕捉到 "Error 1" (或 "Error 2"，取決於順序)
    Console.WriteLine($"捕捉到：{ex.Message}");
}

Console.WriteLine("\n示範 Task.WhenAll 多重例外檢查 Task.Exception (取得所有)");

var allTasks = Task.WhenAll(ThrowAsync("Error A"), ThrowAsync("Error B"));
try
{
    await allTasks;
}
catch
{
    // 檢查 allTasks.Exception 來取得所有錯誤
    foreach (var innerEx in allTasks.Exception.InnerExceptions)
    {
        Console.WriteLine($"錯誤：{innerEx.Message}");
    }
}
async Task<string> DownloadPageAsync(string url)
{
    return await httpClient.GetStringAsync(url);
}

async Task ThrowAsync(string message)
{
    await Task.Yield();
    throw new InvalidOperationException(message);
}
