using System;
using System.Collections.Generic;
using System.Threading.Tasks;

Console.WriteLine("Preparing to consume the asynchronous stream...");

// Use await foreach to process each item in the stream
await foreach (var dataChunk in FetchPaginatedDataAsync())
{
    Console.WriteLine($"Received and processed data: {dataChunk}");
}

Console.WriteLine("The stream has been fully consumed.");

// Implement an async iterator
static async IAsyncEnumerable<string> FetchPaginatedDataAsync()
{
    for (int page = 1; page <= 5; page++)
    {
        // Simulate an asynchronous network request that takes 1 second
        await Task.Delay(1000);

        string dataChunk = $"This is the data for page {page}";

        // Yield one item
        // The method pauses here until the consumer requests the next item
        yield return dataChunk;
    }
}
