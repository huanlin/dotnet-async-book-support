using System;
using System.Threading.Tasks;

namespace Ex02_TaskContinuationOptions
{
    class Program
    {
        static void Main(string[] args)
        {
            Task taskA =
                Task.Run(
                    () => Div(10, 5) );  // 若把第二個參數改成 0，接續的工作會變成 taskOnFailed。

            Task taskOnFailed =
                taskA.ContinueWith(
                    antecedentTask =>
                    {
                        Console.WriteLine("Task A 已失敗! IsFaulted={0}", antecedentTask.IsFaulted);
                    },
                    TaskContinuationOptions.OnlyOnFaulted);

            Task taskOnCanceled =
                taskA.ContinueWith(
                    antecedentTask =>
                    {
                        Console.WriteLine("Task A 已取消! IsCanceled={0}", antecedentTask.IsCanceled);
                    },
                    TaskContinuationOptions.OnlyOnCanceled);

            Task taskOnCompleted =
                taskA.ContinueWith(
                    antecedentTask =>
                    {
                        Console.WriteLine("Task A 已完成! IsCompleted={0}", antecedentTask.IsCompleted);
                    },
                    TaskContinuationOptions.OnlyOnRanToCompletion);

            taskOnCompleted.Wait();
        }

        static int Div(int dividend, int divisor)
        {
            return dividend / divisor;
        }
    }
}
