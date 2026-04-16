using System;
using System.Threading.Tasks;

Console.WriteLine("Demonstrating await using and IAsyncDisposable");

await ProcessDataAsync();

static async Task ProcessDataAsync()
{
    // Use await using to ensure DisposeAsync() is called
    await using (var myAsyncResource = new MyAsyncResource())
    {
        Console.WriteLine("Using the resource...");
    } // Here, the compiler automatically generates code to await myAsyncResource.DisposeAsync()
}

// A class that implements IAsyncDisposable
public sealed class MyAsyncResource : IAsyncDisposable
{
    private bool _isDisposed = false;

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
        {
            return;
        }

        Console.WriteLine("Starting asynchronous resource cleanup...");
        // Simulate asynchronous cleanup work, such as flushing a buffer to the network
        await Task.Delay(500);
        Console.WriteLine("Resource cleanup completed.");

        _isDisposed = true;
        // Tell the GC there is no need to call the finalizer anymore (if one exists)
        GC.SuppressFinalize(this);
    }
}
