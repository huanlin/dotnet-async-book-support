// 示範使用 lock 保護共享計數器，避免競態條件（race condition）

var counter = new ThreadSafeCounter();

Task task1 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
        counter.Increment();
});

Task task2 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
        counter.Increment();
});

Task.WaitAll(task1, task2);
Console.WriteLine($"計數器的最終結果是: {counter.Value}"); // 結果永遠是 2000000

public class ThreadSafeCounter
{
    private int _count = 0;
    private readonly object _lock = new object(); // 鎖物件

    public void Increment()
    {
        lock (_lock) // 進入關鍵區段
        {
            _count++;
        }
    }

    public int Value
    {
        get
        {
            lock (_lock)
            {
                return _count;
            }
        }
    }
}
