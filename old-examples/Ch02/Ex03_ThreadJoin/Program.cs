using System;
using System.Threading;

namespace Ex03_ThreadJoin
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Thread t1 = new Thread(MyTask);
            Thread t2 = new Thread(MyTask);
            Thread t3 = new Thread(MyTask);

            t1.Start("T1");
            t2.Start("T2");
            t3.Start("T3");

            t1.Join();
            t2.Join();
            t3.Join();

            Console.WriteLine("t1, t2, t3 已執行完畢");
            Console.ReadKey();
        }

        private static void MyTask(object param)
        {
            Console.WriteLine("{0} 已開始執行 MyTask()", param);
            Thread.Sleep(3000);
            Console.WriteLine("{0} 即將完成工作", param);
        }
    }
}