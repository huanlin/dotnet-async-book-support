using System;
using System.Threading.Tasks;

Console.WriteLine("示範 await using 與 IAsyncDisposable");

await ProcessDataAsync();

static async Task ProcessDataAsync()
{
    // 使用 await using 來確保 DisposeAsync() 會被呼叫
    await using (var myAsyncResource = new MyAsyncResource())
    {
        Console.WriteLine("使用資源中...");
    } // 在此處，編譯器會自動產生程式碼來 await myAsyncResource.DisposeAsync()
}

// 一個實作了 IAsyncDisposable 的類別
public class MyAsyncResource : IAsyncDisposable
{
    private bool _isDisposed = false;

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
        {
            return;
        }

        Console.WriteLine("開始非同步清理資源...");
        // 模擬一個非同步的清理工作，例如將緩衝區 flush 到網路
        await Task.Delay(500);
        Console.WriteLine("資源清理完畢。");

        _isDisposed = true;
        // 告訴 GC 不再需要呼叫 finalizer (如果有的話)
        GC.SuppressFinalize(this);
    }
}
