Console.WriteLine("Demonstrating timeout and cancellation semantics");

var service = new OperationService();

Console.WriteLine("\n--- Scenario 1: the user cancels explicitly (simulating a user clicking Cancel) ---");
using (var userCts = new CancellationTokenSource())
{
    // Simulate the user clicking “Cancel” after 450 ms
    _ = Task.Delay(450).ContinueWith(_ => userCts.Cancel());

    try
    {
        await service.RunOperationAsync(userCts.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Caught OperationCanceledException: the caller canceled the operation by using CancellationToken.");
    }
}

Console.WriteLine("\n--- Scenario 2: the caller cancels with a time-limited CancellationToken ---");
using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(450)))
{
    try
    {
        await service.RunOperationAsync(timeoutCts.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Caught the same OperationCanceledException: this time the caller triggered cancellation with a time limit.");
    }
}

Console.WriteLine("\n--- Scenario 3: if the API explicitly needs to distinguish timeout, it can wrap it as TimeoutException ---");
try
{
    await service.RunWithExplicitTimeoutAsync(TimeSpan.FromMilliseconds(450));
}
catch (TimeoutException ex)
{
    Console.WriteLine($"Caught TimeoutException: {ex.Message}");
}

public sealed class OperationService
{
    public async Task RunOperationAsync(CancellationToken cancellationToken = default)
    {
        for (int i = 1; i <= 5; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"[Service] Running step {i}...");
            await Task.Delay(200, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task RunWithExplicitTimeoutAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await RunOperationAsync(linkedCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("The operation exceeded the timeout explicitly defined by the API.");
        }
    }
}
