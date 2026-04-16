Console.WriteLine("Demonstrating resource cleanup with IAsyncDisposable");

await using (var wrapper = new NetworkStreamWrapper(GetStream()))
{
    await wrapper.SendAsync("Hello!");
}

Console.WriteLine("Demo complete.");

static Stream GetStream() => new MemoryStream();

public sealed class NetworkStreamWrapper : IAsyncDisposable
{
    private Stream? _stream;

    public NetworkStreamWrapper(Stream stream)
    {
        _stream = stream;
        Console.WriteLine("[Wrapper] Establishing the network connection");
    }

    public async Task SendAsync(string message)
    {
        ObjectDisposedException.ThrowIf(_stream is null, typeof(NetworkStreamWrapper));

        Console.WriteLine($"[Wrapper] Sending message: {message}");
        byte[] payload = System.Text.Encoding.UTF8.GetBytes(message);
        await _stream.WriteAsync(payload).ConfigureAwait(false);
        await Task.Delay(100).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_stream is Stream stream)
        {
            _stream = null;

            Console.WriteLine("[Wrapper] Preparing to release resources... (asynchronous cleanup begins)");
            await Task.Delay(300).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
            await stream.DisposeAsync().ConfigureAwait(false);
            Console.WriteLine("[Wrapper] The resource has been released safely and asynchronously");
        }
    }
}
