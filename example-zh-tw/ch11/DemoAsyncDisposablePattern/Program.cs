Console.WriteLine("示範：IAsyncDisposable 的資源釋放");

await using (var wrapper = new NetworkStreamWrapper(GetStream()))
{
    await wrapper.SendAsync("Hello!");
}

Console.WriteLine("展示完畢。");

static Stream GetStream() => new MemoryStream();

public sealed class NetworkStreamWrapper : IAsyncDisposable
{
    private Stream? _stream;

    public NetworkStreamWrapper(Stream stream)
    {
        _stream = stream;
        Console.WriteLine("[Wrapper] 建立網路連線");
    }

    public async Task SendAsync(string message)
    {
        ObjectDisposedException.ThrowIf(_stream is null, typeof(NetworkStreamWrapper));

        Console.WriteLine($"[Wrapper] 發送訊息: {message}");
        byte[] payload = System.Text.Encoding.UTF8.GetBytes(message);
        await _stream.WriteAsync(payload).ConfigureAwait(false);
        await Task.Delay(100).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_stream is Stream stream)
        {
            _stream = null;

            Console.WriteLine("[Wrapper] 準備釋放資源... (非同步清理開始)");
            await Task.Delay(300).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
            await stream.DisposeAsync().ConfigureAwait(false);
            Console.WriteLine("[Wrapper] 資源已安全且非同步地釋放");
        }
    }
}
