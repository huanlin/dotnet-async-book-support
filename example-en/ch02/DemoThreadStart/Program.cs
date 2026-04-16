using System.Threading;

var msg = $"Main thread ID: {Environment.CurrentManagedThreadId}";
Console.WriteLine(msg);

// Create a new thread to execute DoWork
var newThread = new Thread(DoWork);
newThread.Start();

Console.WriteLine("Main thread continues running...");

void DoWork()
{
    var msg = $"Worker thread ID: {Environment.CurrentManagedThreadId}";
    Console.WriteLine(msg);
    Console.WriteLine("Work is in progress...");
    Thread.Sleep(2000); // Simulate 2 seconds of work
    Console.WriteLine("Work is done.");
}
