using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// Allow at most 3 operations to enter this asynchronous work at the same time.
SemaphoreSlim _semaphore = new SemaphoreSlim(3, 3);

var tasks = new List<Task>();
for (int i = 1; i <= 10; i++)
{
    int taskId = i;
    tasks.Add(PerformExpensiveOperationAsync(taskId));
}

await Task.WhenAll(tasks);
Console.WriteLine("All tasks have finished.");

async Task PerformExpensiveOperationAsync(int id)
{
    Console.WriteLine($"Task {id} is waiting to enter...");
    // Wait asynchronously for a slot.
    await _semaphore.WaitAsync();
    try
    {
        Console.WriteLine($"--> Task {id} entered and is running...");
        await Task.Delay(2000); // Simulate expensive work
    }
    finally
    {
        // Always release the slot.
        _semaphore.Release();
        Console.WriteLine($"<-- Task {id} left.");
    }
}
