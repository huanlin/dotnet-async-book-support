using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ex01_TaskContinuation
{
    class Program
    {
        static void Main(string[] args)
        {
            var taskA = Task.Run(() => Console.WriteLine("起始工作...."));

            Task taskB = taskA.ContinueWith( antecedentTask => 
                {
                    Console.WriteLine("從 Task A 接續 Task B.");
                    System.Threading.Thread.Sleep(4000); // 等待幾秒
                } );

            Task taskC = taskA.ContinueWith( antecedentTask =>
                {
                    Console.WriteLine("從 Task A 接續 Task C.");
                } );

            Task taskD = taskA.ContinueWith( antecedentTask =>
                {
                    Console.WriteLine("從 Task A 接續開始 Task D.");
                });

            Task.WaitAll(taskB, taskC);

            Console.WriteLine("程式結束");
        }
    }
}
