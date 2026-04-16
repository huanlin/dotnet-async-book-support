using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

// Reuse a single HttpClient instance to avoid socket exhaustion.
using var httpClient = new HttpClient();

Console.WriteLine("Demo: Task.Wait() throws AggregateException");

var faultyTask = DownloadPageAsync("https://this-host-does-not-exist.invalid");

try
{
    // Using .Wait() throws AggregateException.
    faultyTask.Wait();
}
catch (AggregateException ex)
{
    // We need to unwrap the real exception from the InnerExceptions collection.
    var realException = ex.InnerExceptions.First();
    Console.WriteLine(
        $"Caught AggregateException. The real error is: {realException.GetType().Name}");
}

Console.WriteLine("\nDemo: await Task.WhenAll with multiple exceptions (only one is propagated)");

try
{
    var task1 = ThrowAsync("Error 1");
    var task2 = ThrowAsync("Error 2");
    await Task.WhenAll(task1, task2);
}
catch (Exception ex)
{
    // Only one of the errors will be caught here.
    Console.WriteLine($"Caught: {ex.Message}");
}

Console.WriteLine("\nDemo: inspect Task.Exception after Task.WhenAll (get them all)");

var allTasks = Task.WhenAll(ThrowAsync("Error A"), ThrowAsync("Error B"));
try
{
    await allTasks;
}
catch
{
    // Inspect allTasks.Exception to get every error.
    foreach (var innerEx in allTasks.Exception!.InnerExceptions)
    {
        Console.WriteLine($"Error: {innerEx.Message}");
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
