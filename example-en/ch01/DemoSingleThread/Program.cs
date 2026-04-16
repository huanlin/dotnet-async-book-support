// Stage 1: the synchronous single chef (synchronous single-threading)
// Demonstrates how a single thread gets blocked

using System.Diagnostics;

Console.WriteLine("The pizza shop is open! Stage 1: only one chef (single-threaded)");
var sw = Stopwatch.StartNew();

// Simulate making three pizzas one after another
MakePizza(1);
MakePizza(2);
MakePizza(3);

sw.Stop();
Console.WriteLine($"All pizzas are ready. Total time: {sw.ElapsedMilliseconds} ms");

void MakePizza(int id)
{
    Console.WriteLine($"[Single chef] Starting dough prep for pizza {id}...");
    Thread.Sleep(500); // Simulate prep time for chopping and kneading

    Console.WriteLine($"[Single chef] Pizza {id} is in the oven. Waiting...");

    // Thread.Sleep simulates a blocking operation here.
    // The chef can only wait. He cannot prep the next pizza or answer the phone.
    Thread.Sleep(2000);

    Console.WriteLine($"[Single chef] Pizza {id} is done! Taking it out.");
}
