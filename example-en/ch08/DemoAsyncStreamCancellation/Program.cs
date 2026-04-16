using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3.5));

try
{
    // Consumer side: use .WithCancellation()
    await foreach (var dataChunk in FetchPaginatedDataAsync()
                                     .WithCancellation(cts.Token))
    {
        Console.WriteLine($"Received and processed data: {dataChunk}");
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("The stream processing was canceled or timed out.");
}

// Producer side: accept a CancellationToken and add [EnumeratorCancellation]
static async IAsyncEnumerable<string> FetchPaginatedDataAsync(
    [EnumeratorCancellation] CancellationToken token = default)
{
    for (int page = 1; page <= 5; page++)
    {
        // Pass the token to every async method that accepts it
        await Task.Delay(1000, token);

        // You can also check manually
        // token.ThrowIfCancellationRequested();

        string dataChunk = $"This is the data for page {page}";
        yield return dataChunk;
    }
}
