using System.Threading;
using System.Threading.Tasks;

Console.WriteLine($"Main thread ID: {Environment.CurrentManagedThreadId}");

Console.WriteLine("Getting ready to use Task.Run for background work...");

// Hand the work to the thread pool and get the Task object
Task task = Task.Run(() =>
{
    Console.WriteLine($"Background thread ID: {Environment.CurrentManagedThreadId}");
    Console.WriteLine("Background work is in progress...");
    Thread.Sleep(2000); // Used here only to simulate a time-consuming synchronous operation
    Console.WriteLine("Background work is done.");
});

Console.WriteLine("Main thread has called Task.Run and is free to do other things...");

// Wait for the Task to complete
task.Wait();

Console.WriteLine("Confirmed that the Task has completed. The main program is about to exit.");
