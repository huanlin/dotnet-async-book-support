using System;
using System.Threading.Tasks;

int counter = 0;

// Start two tasks that increment counter concurrently.
Task task1 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
    {
        counter++;
    }
});

Task task2 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
    {
        counter++;
    }
});

// Wait for both tasks to finish.
Task.WaitAll(task1, task2);

Console.WriteLine($"The final counter value is: {counter}");
