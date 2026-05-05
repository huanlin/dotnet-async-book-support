namespace DemoAsyncMvvmWpf;

public sealed class FakeDataService : IDataService
{
    public async Task<string> FetchDataAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1500, cancellationToken);

        return """
            DataService は、シミュレートされた I/O バウンド作業を完了しました。
            このデータは、Web API やデータベースから読み込まれたものだと想像してください。
            操作の待機中も UI は応答し続け、IsBusy によってボタンは一時的に無効化されました。
            """;
    }
}
