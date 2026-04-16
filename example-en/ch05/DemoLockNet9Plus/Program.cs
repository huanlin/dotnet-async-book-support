using System;
using System.Threading;
using System.Threading.Tasks;

int counter = 0;

// Use the dedicated Lock type instead of object.
Lock _lock = new Lock();

Task task1 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
    {
        // The syntax looks the same as before, but the compiler
        // generates more efficient code for Lock.
        lock (_lock)
        {
            counter++;
        }
    }
});

Task task2 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
    {
        // The EnterScope style.
        using (_lock.EnterScope())
        {
            // Enter the critical section.
            counter++;
        } // The lock is released automatically when leaving the block.
    }
});

Task.WaitAll(task1, task2);
Console.WriteLine($"The final counter value is: {counter}");
