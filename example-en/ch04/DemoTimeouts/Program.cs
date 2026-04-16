using System;
using System.Threading;
using System.Threading.Tasks;

// Create a CTS that automatically cancels after 3 seconds.
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

try
{
    Console.WriteLine("Starting a job that is allowed to run for at most 3 seconds...");
    // This work takes 10 seconds internally.
    await DoSomeLongRunningWorkAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("The work was canceled because it timed out!");
}

static async Task DoSomeLongRunningWorkAsync(CancellationToken token)
{
    Console.WriteLine("Background work started...");
    for (int i = 0; i < 10; i++)
    {
        token.ThrowIfCancellationRequested();
        Console.WriteLine($"Working on part {i + 1}/10...");
        await Task.Delay(1000, token);
    }
    Console.WriteLine("Background work completed successfully.");
}
