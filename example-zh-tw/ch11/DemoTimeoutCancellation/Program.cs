Console.WriteLine("示範 timeout 與 cancellation 語意");

var service = new OperationService();

Console.WriteLine("\n--- 情境一：使用者主動取消（模擬使用者點擊取消按鈕）---");
using (var userCts = new CancellationTokenSource())
{
    // 模擬使用者在 450ms 後點擊「取消」按鈕
    _ = Task.Delay(450).ContinueWith(_ => userCts.Cancel());

    try
    {
        await service.RunOperationAsync(userCts.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("捕捉到 OperationCanceledException：呼叫端使用 CancellationToken 取消了操作。");
    }
}

Console.WriteLine("\n--- 情境二：呼叫端用帶逾時的 CancellationToken 取消 ---");
using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(450)))
{
    try
    {
        await service.RunOperationAsync(timeoutCts.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("同樣捕捉到 OperationCanceledException：這次是呼叫端用時間限制觸發取消。");
    }
}

Console.WriteLine("\n--- 情境三：如果 API 明確需要區分 timeout，可自行包成 TimeoutException ---");
try
{
    await service.RunWithExplicitTimeoutAsync(TimeSpan.FromMilliseconds(450));
}
catch (TimeoutException ex)
{
    Console.WriteLine($"捕捉到 TimeoutException：{ex.Message}");
}

public sealed class OperationService
{
    public async Task RunOperationAsync(CancellationToken cancellationToken = default)
    {
        for (int i = 1; i <= 5; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"[Service] 執行第 {i} 個步驟...");
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
            throw new TimeoutException("操作超過了 API 明確定義的 timeout。");
        }
    }
}
