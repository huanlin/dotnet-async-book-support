namespace DemoAsyncMvvmWpf;

public sealed class FakeDataService : IDataService
{
    public async Task<string> FetchDataAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1500, cancellationToken);

        return """
            DataService 已完成模擬的 I/O-bound 工作。
            這段資料可以想像成是從 Web API 或資料庫載入而來。
            在等待期間，UI 仍保持回應，按鈕也會因為 IsBusy 而暫時禁用。
            """;
    }
}
