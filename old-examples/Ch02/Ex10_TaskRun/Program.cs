using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ex10_TaskRun
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Task task = Task.Run(() =>
                {
                    for (int i = 0; i < 500; i++)
                    {
                        Console.Write("[" + Thread.CurrentThread.ManagedThreadId + "]");
                    }
                });

            for (int i = 0; i < 500; i++)
            {
                Console.Write(".");
            }

            task.Wait(); // 確保非同步工作執行完畢之後才往下繼續執行。
        }
    }
}