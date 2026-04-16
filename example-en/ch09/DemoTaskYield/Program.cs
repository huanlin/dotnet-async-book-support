using System.Threading.Tasks;

Console.WriteLine("Demonstrating Task.Yield() creating an async boundary");
Console.WriteLine($"Main thread id: {Environment.CurrentManagedThreadId}");

Console.WriteLine("1. Caller: invoking DemoAsync()");
Task task = DemoAsync();
Console.WriteLine("2. Caller: DemoAsync() has returned a Task");

await task;
Console.WriteLine("5. Caller: the Task has completed");

static async Task DemoAsync()
{
    Console.WriteLine($"3. Inside method: before await Task.Yield(), thread id = {Environment.CurrentManagedThreadId}");

    // Force an async boundary here so the caller can continue first.
    await Task.Yield();

    Console.WriteLine($"4. Inside method: after await Task.Yield(), thread id = {Environment.CurrentManagedThreadId}");
}
