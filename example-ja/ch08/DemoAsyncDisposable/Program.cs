using System;
using System.Threading.Tasks;

Console.WriteLine("await using と IAsyncDisposable のデモ");

await ProcessDataAsync();

static async Task ProcessDataAsync()
{
    // DisposeAsync() が呼ばれるように await using を使う
    await using (var myAsyncResource = new MyAsyncResource())
    {
        Console.WriteLine("リソースを使用しています...");
    } // ここでは、myAsyncResource.DisposeAsync() を await するコードをコンパイラが自動生成する
}

// IAsyncDisposable を実装するクラス
public sealed class MyAsyncResource : IAsyncDisposable
{
    private bool _isDisposed = false;

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
        {
            return;
        }

        Console.WriteLine("非同期リソースのクリーンアップを開始します...");
        // バッファをネットワークへフラッシュする処理など、非同期クリーンアップ作業をシミュレートする
        await Task.Delay(500);
        Console.WriteLine("リソースのクリーンアップが完了しました。");

        _isDisposed = true;
        // ファイナライザーをもう呼ぶ必要がないことを GC に伝える (存在する場合)
        GC.SuppressFinalize(this);
    }
}
