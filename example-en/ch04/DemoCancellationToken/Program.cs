using System;
using System.Threading;
using System.Threading.Tasks;

// 1. Create a CancellationTokenSource.
using var cts = new CancellationTokenSource();

// 2. Get a CancellationToken from the CTS.
var token = cts.Token;

// 3. Pass the token to the asynchronous method
// (do not await it immediately, so it can run in the background).
Task workTask = DoSomeLongRunningWorkAsync(token);

// Simulate the user spending some time before deciding to cancel.
await Task.Delay(2500);

// 4. At some later point, when cancellation is needed...
Console.WriteLine("\n[Caller] The user decided to cancel the operation!");
cts.Cancel(); // Press the "Cancel" button.

try
{
    await workTask; // Wait for the background work to finish.
}
catch (OperationCanceledException) 
{
    // This is expected.
    Console.WriteLine("The caller caught OperationCanceledException.");
}
catch (Exception ex)
{
    // This is the real error case.
    Console.WriteLine($"The work failed: {ex.Message}");
}

static async Task DoSomeLongRunningWorkAsync(CancellationToken token)
{
    Console.WriteLine("Background work started...");
    try
    {
        for (int i = 0; i < 10; i++)
        {
            // Check whether cancellation has been requested.
            token.ThrowIfCancellationRequested();

            Console.WriteLine($"Working on part {i + 1}/10...");
            // Important: keep passing the token to any lower-level API
            // that supports cancellation.
            await Task.Delay(1000, token);
        }
        Console.WriteLine("Background work completed successfully.");
    }
    catch (OperationCanceledException)
    {
        // ThrowIfCancellationRequested throws OperationCanceledException,
        // and APIs such as Task.Delay often throw the derived type
        // TaskCanceledException.
        Console.WriteLine("Background work was canceled.");
        throw; // Usually this should be rethrown.
    }
}
