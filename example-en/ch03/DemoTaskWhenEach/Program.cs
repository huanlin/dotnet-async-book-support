using System;
using System.Collections.Generic;
using System.Threading.Tasks;

Console.WriteLine("Demonstrating Task.WhenEach (.NET 9) to process tasks as they complete");

await ProcessTasksAsTheyCompleteAsync();

// New in .NET 9+: clean and intuitive
async Task ProcessTasksAsTheyCompleteAsync()
{
    var tasks = new List<Task<int>>();
    for (int i = 1; i <= 5; i++)
    {
        tasks.Add(DoWorkAsync(i)); // Assume DoWorkAsync returns Task<int>
    }

    // Here, t represents a task that has already completed
    // The loop iterates in completion order, not in the original list order.
    await foreach (Task<int> t in Task.WhenEach(tasks))
    {
        try
        {
            int result = await t; // The await here is only to obtain the result or exception. It does not block.
            Console.WriteLine($"One task completed. Its result is: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"One task failed: {ex.Message}");
        }
    }
}

// Simulate work that takes a random amount of time
async Task<int> DoWorkAsync(int id)
{
    int delay = new Random().Next(500, 2000);
    await Task.Delay(delay);
    return id * 10;
}
