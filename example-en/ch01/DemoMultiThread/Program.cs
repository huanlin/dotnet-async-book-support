// Stage 2: more flexible multitasking chefs (multithreading)
// Demonstrates concurrent work with multiple threads

using System.Diagnostics;

Console.WriteLine("The pizza shop is open! Stage 2: multiple chefs at work (multithreading)");
var sw = Stopwatch.StartNew();

// Create three separate threads, representing three different chefs
Thread chef1 = new(() => MakePizza(1));
Thread chef2 = new(() => MakePizza(2));
Thread chef3 = new(() => MakePizza(3));

// Start all chefs at the same time
chef1.Start();
chef2.Start();
chef3.Start();

// The main thread (restaurant manager) waits for all chefs to finish
chef1.Join();
chef2.Join();
chef3.Join();

sw.Stop();
Console.WriteLine($"All pizzas are ready. Total time: {sw.ElapsedMilliseconds} ms");

void MakePizza(int id)
{
    int threadId = Environment.CurrentManagedThreadId;

    Console.WriteLine($"[Chef {threadId}] Starting prep for pizza {id}...");
    Thread.Sleep(500); // Simulate prep time for chopping and kneading

    Console.WriteLine($"[Chef {threadId}] Pizza {id} is in the oven. Waiting...");

    // Thread.Sleep is still a blocking operation, but each pizza has its own
    // dedicated chef (thread), so while one chef waits, the others can keep
    // working on their own pizzas.
    Thread.Sleep(2000);

    Console.WriteLine($"[Chef {threadId}] Pizza {id} is done! Taking it out.");
}
