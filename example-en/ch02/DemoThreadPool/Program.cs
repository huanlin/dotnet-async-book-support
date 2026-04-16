using System.Threading;

var msg = $"Main thread ID: {Environment.CurrentManagedThreadId}";
Console.WriteLine(msg);

// Queue a work item to the thread pool
ThreadPool.QueueUserWorkItem(_ => DoWork());

Console.WriteLine("Main thread continues running...");
Thread.Sleep(3000); // Wait for the background work to finish, or the main program might exit first

void DoWork()
{
    Console.WriteLine($"Background thread ID: {Environment.CurrentManagedThreadId}");
    Console.WriteLine("Background work is in progress...");
    Thread.Sleep(2000); // Simulate 2 seconds of work
    Console.WriteLine("Background work is done.");
}
