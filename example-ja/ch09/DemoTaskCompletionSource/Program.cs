using System;
using System.Threading.Tasks;

Console.WriteLine("TaskCompletionSource でイベントを Task としてラップするデモ");

using var timer = new System.Timers.Timer(2000); // 2 秒のカウントダウン
Console.WriteLine("2 秒タイマーを開始します...");

await timer.WaitAsync();

Console.WriteLine("時間です!");

public static class TimerExtensions
{
    public static Task WaitAsync(this System.Timers.Timer timer)
    {
        // タイマー コールバックの経路上で、await 後の継続を同期的に実行しないようにする。
        var tcs = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously);
        timer.Elapsed += OnElapsed;
        timer.AutoReset = false;
        timer.Enabled = true;

        return tcs.Task;

        void OnElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Elapsed -= OnElapsed;
            tcs.SetResult();
        }
    }
}
