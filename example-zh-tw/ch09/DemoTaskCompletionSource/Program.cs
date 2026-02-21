using System;
using System.Threading.Tasks;

Console.WriteLine("示範 TaskCompletionSource 將事件包裝為 Task");

var timer = new System.Timers.Timer(2000); // 2 秒倒數
Console.WriteLine("開始計時 2 秒...");

await timer.WaitAsync();

Console.WriteLine("時間到！");

public static class TimerExtensions
{
    public static Task WaitAsync(this System.Timers.Timer timer)
    {
        var tcs = new TaskCompletionSource();
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
