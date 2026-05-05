Console.WriteLine("IAsyncDisposable によるリソース クリーンアップのデモ");

await using (var wrapper = new NetworkStreamWrapper(GetStream()))
{
    await wrapper.SendAsync("こんにちは!");
}

Console.WriteLine("デモが完了しました。");

static Stream GetStream() => new MemoryStream();

public sealed class NetworkStreamWrapper : IAsyncDisposable
{
    private Stream? _stream;

    public NetworkStreamWrapper(Stream stream)
    {
        _stream = stream;
        Console.WriteLine("[Wrapper] ネットワーク接続を確立しています");
    }

    public async Task SendAsync(string message)
    {
        ObjectDisposedException.ThrowIf(_stream is null, typeof(NetworkStreamWrapper));

        Console.WriteLine($"[Wrapper] メッセージを送信しています: {message}");
        byte[] payload = System.Text.Encoding.UTF8.GetBytes(message);
        await _stream.WriteAsync(payload).ConfigureAwait(false);
        await Task.Delay(100).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_stream is Stream stream)
        {
            _stream = null;

            Console.WriteLine("[Wrapper] リソース解放の準備をしています... (非同期クリーンアップ開始)");
            await Task.Delay(300).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
            await stream.DisposeAsync().ConfigureAwait(false);
            Console.WriteLine("[Wrapper] リソースは安全に非同期で解放されました");
        }
    }
}
