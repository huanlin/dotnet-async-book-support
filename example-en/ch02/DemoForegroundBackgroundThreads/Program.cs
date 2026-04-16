using System.Threading;
using System.Threading.Tasks;

// Example 1: use Thread (foreground thread)
var foregroundThread = new Thread(() =>
{
    Thread.Sleep(3000);
    Console.WriteLine("Foreground thread finished.");
});
// foregroundThread.IsBackground = true; // Can be set to background manually
foregroundThread.Start();
Console.WriteLine("The Main method (foreground) is about to exit, but the program will wait for the foreground thread to finish.");


// Example 2: use Task.Run (background thread)
_ = Task.Run(() =>
{
    Thread.Sleep(5000);
    // This line might never run
    Console.WriteLine("Background thread finished.");
});
Console.WriteLine("The Main method (foreground) is about to exit, and the program will not wait for the background thread.");
