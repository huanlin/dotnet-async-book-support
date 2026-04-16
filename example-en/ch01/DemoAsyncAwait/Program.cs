// Stage 3: the super-efficient smart chef (asynchronous programming)
// Demonstrates how async/await releases the thread for non-blocking waiting

using System.Diagnostics;

var msg = "The pizza shop is open! Stage 3: a super chef with a smart oven (asynchronous programming)";
Console.WriteLine(msg);
var sw = Stopwatch.StartNew();

// Start three asynchronous pizza-making tasks
// Note: calling an async method begins running on the current thread
// until it reaches the first incomplete await.
// It does not automatically jump to a new background thread at the start.
var p1 = MakePizzaAsync(1);
var p2 = MakePizzaAsync(2);
var p3 = MakePizzaAsync(3);

// Asynchronously wait for all pizza tasks to finish
await Task.WhenAll(p1, p2, p3);

sw.Stop();
msg = $"All pizzas are ready. Total time: {sw.ElapsedMilliseconds} ms";
Console.WriteLine(msg);

async Task MakePizzaAsync(int id)
{
    // Capture the current thread ID so we can observe thread changes
    int threadId = Environment.CurrentManagedThreadId;
    var str = $"[Chef {threadId}] Starting pizza {id}, waiting for the dough to rise first...";
    Console.WriteLine(str);

    // Simulate a wait that can be asynchronous, such as waiting for the dough
    // to rise or for ingredients to arrive. This does not block the thread.
    await Task.Delay(500);

    threadId = Environment.CurrentManagedThreadId;
    str = $"[Chef {threadId}] Dough is ready. Pizza {id} goes into the oven; timer set, off to do something else!";
    Console.WriteLine(str);

    // Simulate a time-consuming operation that waits for an external response:
    // baking in the oven. While awaiting, the thread is not blocked.
    // The system can use that thread for other work until the wait is over.
    await Task.Delay(2000);

    threadId = Environment.CurrentManagedThreadId;
    str = $"[Chef {threadId}] Ding! Pizza {id} is ready, coming back to take it out.";
    Console.WriteLine(str);
}
