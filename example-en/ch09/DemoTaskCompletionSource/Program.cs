using System;
using System.Threading.Tasks;

Console.WriteLine("Demonstrating TaskCompletionSource wrapping an event as a Task");

using var timer = new System.Timers.Timer(2000); // 2-second countdown
Console.WriteLine("Starting a 2-second timer...");

await timer.WaitAsync();

Console.WriteLine("Time's up!");

public static class TimerExtensions
{
    public static Task WaitAsync(this System.Timers.Timer timer)
    {
        // Avoid running the continuation after await synchronously on the timer callback path.
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
