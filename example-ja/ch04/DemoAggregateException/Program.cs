using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

// ソケット枯渇を避けるため、HttpClient インスタンスを 1 つだけ再利用する。
using var httpClient = new HttpClient();

Console.WriteLine("デモ: Task.Wait() は AggregateException をスローします");

var faultyTask = DownloadPageAsync("https://this-host-does-not-exist.invalid");

try
{
    // .Wait() を使うと AggregateException がスローされる。
    faultyTask.Wait();
}
catch (AggregateException ex)
{
    // 実際の例外を InnerExceptions コレクションから取り出す必要がある。
    var realException = ex.InnerExceptions.First();
    Console.WriteLine(
        $"AggregateException をキャッチしました。実際のエラーは: {realException.GetType().Name}");
}

Console.WriteLine("\nデモ: 複数の例外がある await Task.WhenAll (伝播されるのは 1 つだけ)");

try
{
    var task1 = ThrowAsync("エラー 1");
    var task2 = ThrowAsync("エラー 2");
    await Task.WhenAll(task1, task2);
}
catch (Exception ex)
{
    // ここではエラーのうち 1 つだけがキャッチされる。
    Console.WriteLine($"キャッチしました: {ex.Message}");
}

Console.WriteLine("\nデモ: Task.WhenAll の後で Task.Exception を調べる (すべて取得する)");

var allTasks = Task.WhenAll(ThrowAsync("エラー A"), ThrowAsync("エラー B"));
try
{
    await allTasks;
}
catch
{
    // すべてのエラーを取得するには allTasks.Exception を調べる。
    foreach (var innerEx in allTasks.Exception!.InnerExceptions)
    {
        Console.WriteLine($"エラー: {innerEx.Message}");
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
