var demo = new LambdaDemo1();
demo.InitTimer();
Console.ReadLine();

public class LambdaDemo1
{
    private System.Timers.Timer? _timer;
    public void InitTimer()
    {
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (sender, args) => Console.WriteLine("X");
        _timer.Enabled = true;
    }
}


