// TaskStatus の値と便利な状態プロパティを示す

Console.WriteLine("意図的に失敗するタスクを作成して開始します...");

Task myTask = Task.Run(() =>
{
    Console.WriteLine("タスクが開始され、まもなく例外をスローします...");
    Thread.Sleep(500);
    throw new InvalidOperationException("タスクが失敗しました。");
});                 

try
{
    // 最終状態を観察できるように、意図的にタスクの完了を待つ
    myTask.Wait();
}
catch (AggregateException)
{
    // 失敗した Task に Wait() を使うと、
    // 例外は AggregateException に包まれる。
    // ここではタスクの最終状態を観察できるように、例外をキャッチして無視する。
}

Console.WriteLine($"最終的なタスクの状態: {myTask.Status}");
Console.WriteLine($"IsFaulted の値: {myTask.IsFaulted}");
Console.WriteLine($"IsCompleted の値: {myTask.IsCompleted}");
