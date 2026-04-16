// Demonstrate how lock protects a shared counter
// and prevents a race condition.

var counter = new ThreadSafeCounter();

Task task1 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
        counter.Increment();
});

Task task2 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
        counter.Increment();
});

Task.WaitAll(task1, task2);
Console.WriteLine($"The final counter value is: {counter.Value}"); // Always 2000000

public class ThreadSafeCounter
{
    private int _count = 0;
    private readonly object _lock = new object(); // Lock object

    public void Increment()
    {
        lock (_lock) // Enter the critical section
        {
            _count++;
        }
    }

    public int Value
    {
        get
        {
            // In this specific example, locking the getter is not strictly required.
            lock (_lock)
            {
                return _count;
            }
        }
    }
}
