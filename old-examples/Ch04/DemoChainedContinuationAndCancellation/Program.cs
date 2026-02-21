using System;
using System.Threading;
using System.Threading.Tasks;

namespace DemoChainedContinuationAndCancellation
{
    class Program
    {
        static void Main(string[] args)
        {
            Task taskA = Task.Run(DoSomething);
            var cancelManager = new CancellationTokenSource();
            Task taskB = taskA.ContinueWith(
                _ => Console.WriteLine("這裡不會執行"), cancelManager.Token);
                // 用底下這行取代上一行，便可確保 taskC 在 taskA 完成後才執行。
                //_ => Console.WriteLine("這裡不會執行"), cancelToken.Token, TaskContinuationOptions.LazyCancellation, TaskScheduler.Current);
            Task taskC = taskB.ContinueWith(
                _ => Console.WriteLine("TaskC 執行完畢。"));
            cancelManager.Cancel(); // 取消 taskB

            Console.ReadKey();
        }

        static void DoSomething()
        {
            Thread.Sleep(1500); 
            Console.WriteLine("TaskA 執行完畢。");
        }
    }
}
