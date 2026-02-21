using System;
using System.Threading;

namespace Ex01_ThreadStart
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Thread t1 = new Thread(MyTask);
            t1.Start();

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