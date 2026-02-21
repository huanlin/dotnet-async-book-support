using System;
using System.Threading;

namespace Ex07_ThreadPool
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(MyTask);

            for (int i = 0; i < 500; i++)
            {
                Console.Write(".");
            }
        }

        private static void MyTask(object state)
        {
            for (int i = 0; i < 500; i++)
            {
                Console.Write("[" + Thread.CurrentThread.ManagedThreadId + "]");
            }
        }
    }
}