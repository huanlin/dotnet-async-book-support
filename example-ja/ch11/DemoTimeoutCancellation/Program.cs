Console.WriteLine("タイムアウトとキャンセルのセマンティクスのデモ");

var service = new OperationService();

Console.WriteLine("\n--- シナリオ 1: ユーザーが明示的にキャンセルする (キャンセル ボタンのクリックをシミュレート) ---");
using (var userCts = new CancellationTokenSource())
{
    // ユーザーが 450 ms 後に「キャンセル」をクリックする状況をシミュレートする
    _ = Task.Delay(450).ContinueWith(_ => userCts.Cancel());

    try
    {
        await service.RunOperationAsync(userCts.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("OperationCanceledException をキャッチしました: 呼び出し元が CancellationToken で操作をキャンセルしました。");
    }
}

Console.WriteLine("\n--- シナリオ 2: 呼び出し元が時間制限付き CancellationToken でキャンセルする ---");
using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(450)))
{
    try
    {
        await service.RunOperationAsync(timeoutCts.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("同じ OperationCanceledException をキャッチしました: 今回は呼び出し元が時間制限でキャンセルを発生させました。");
    }
}

Console.WriteLine("\n--- シナリオ 3: API がタイムアウトを明確に区別する必要がある場合は TimeoutException で包める ---");
try
{
    await service.RunWithExplicitTimeoutAsync(TimeSpan.FromMilliseconds(450));
}
catch (TimeoutException ex)
{
    Console.WriteLine($"TimeoutException をキャッチしました: {ex.Message}");
}

public sealed class OperationService
{
    public async Task RunOperationAsync(CancellationToken cancellationToken = default)
    {
        for (int i = 1; i <= 5; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"[Service] ステップ {i} を実行中...");
            await Task.Delay(200, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task RunWithExplicitTimeoutAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await RunOperationAsync(linkedCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("操作が API で明示的に定義されたタイムアウトを超えました。");
        }
    }
}
