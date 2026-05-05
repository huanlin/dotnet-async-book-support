// lock が共有カウンターをどのように保護し、
// 競合状態を防ぐかを示す。

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
Console.WriteLine($"最終的なカウンター値: {counter.Value}"); // 常に 2000000

public class ThreadSafeCounter
{
    private int _count = 0;
    private readonly object _lock = new object(); // ロック用オブジェクト

    public void Increment()
    {
        lock (_lock) // クリティカル セクションに入る
        {
            _count++;
        }
    }

    public int Value
    {
        get
        {
            // この特定の例では、getter をロックすることは厳密には必須ではない。
            lock (_lock)
            {
                return _count;
            }
        }
    }
}
