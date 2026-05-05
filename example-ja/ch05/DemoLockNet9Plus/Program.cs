using System;
using System.Threading;
using System.Threading.Tasks;

int counter = 0;

// object ではなく専用の Lock 型を使う。
Lock _lock = new Lock();

Task task1 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
    {
        // 構文は以前と同じに見えるが、コンパイラは
        // Lock 向けにより効率的なコードを生成する。
        lock (_lock)
        {
            counter++;
        }
    }
});

Task task2 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
    {
        // EnterScope スタイル。
        using (_lock.EnterScope())
        {
            // クリティカル セクションに入る.
            counter++;
        } // ブロックを抜けると、ロックは自動的に解放される。
    }
});

Task.WaitAll(task1, task2);
Console.WriteLine($"最終的なカウンター値: {counter}");
