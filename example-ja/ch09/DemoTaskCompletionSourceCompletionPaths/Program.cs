using System.Threading.Tasks;

Console.WriteLine("TaskCompletionSource で成功・失敗・キャンセルを対応付けるデモ");

foreach (CompletionMode mode in Enum.GetValues<CompletionMode>())
{
    Console.WriteLine($"\nモード: {mode}");

    try
    {
        var operation = new LegacyOperation();
        string result = await operation.RunAsTaskAsync(mode);
        Console.WriteLine($"結果: {result}");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("結果: キャンセルされました");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"結果: 失敗 ({ex.Message})");
    }
}

enum CompletionMode
{
    Success,
    Failure,
    Canceled
}

sealed class LegacyOperation
{
    public event EventHandler<LegacyOperationCompletedEventArgs>? Completed;

    public void Start(CompletionMode mode)
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(300);

            switch (mode)
            {
                case CompletionMode.Success:
                    Completed?.Invoke(this, new LegacyOperationCompletedEventArgs(result: "OK"));
                    break;
                case CompletionMode.Failure:
                    Completed?.Invoke(
                        this,
                        new LegacyOperationCompletedEventArgs(
                            error: new InvalidOperationException("シミュレートされた失敗")));
                    break;
                case CompletionMode.Canceled:
                    Completed?.Invoke(this, new LegacyOperationCompletedEventArgs(canceled: true));
                    break;
            }
        });
    }
}

sealed class LegacyOperationCompletedEventArgs : EventArgs
{
    public LegacyOperationCompletedEventArgs(
        string? result = null,
        Exception? error = null,
        bool canceled = false)
    {
        Result = result;
        Error = error;
        Canceled = canceled;
    }

    public string? Result { get; }

    public Exception? Error { get; }

    public bool Canceled { get; }
}

static class LegacyOperationExtensions
{
    public static Task<string> RunAsTaskAsync(
        this LegacyOperation operation,
        CompletionMode mode)
    {
        var tcs = new TaskCompletionSource<string>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        operation.Completed += OnCompleted;
        operation.Start(mode);

        return tcs.Task;

        void OnCompleted(object? sender, LegacyOperationCompletedEventArgs e)
        {
            operation.Completed -= OnCompleted;

            if (e.Canceled)
            {
                tcs.TrySetCanceled();
            }
            else if (e.Error is not null)
            {
                tcs.TrySetException(e.Error);
            }
            else
            {
                tcs.TrySetResult(e.Result!);
            }
        }
    }
}
