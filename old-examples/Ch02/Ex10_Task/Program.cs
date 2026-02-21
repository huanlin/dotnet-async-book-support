using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ex10_Task
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var task = new Task(MyTask);
            task.Start();

            for (int i = 0; i < 500; i++)
            {
                Console.Write(".");
            }
        }

        private static void MyTask()
        {
            for (int i = 0; i < 500; i++)
            {
                Console.Write("[" + Thread.CurrentThread.ManagedThreadId + "]");
            }
        }
    }
}