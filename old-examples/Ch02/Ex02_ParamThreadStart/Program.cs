using System;
using System.Threading;

namespace Ex02_ParamThreadStart
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Thread t1 = new Thread(MyTask);
            Thread t2 = new Thread(MyTask);
            Thread t3 = new Thread(MyTask);

            t1.Start("X");
            t2.Start("Y");
            t3.Start("Z");

            for (int i = 0; i < 500; i++)
            {
                Console.Write(".");
            }
        }

        private static void MyTask(object param)
        {
            for (int i = 0; i < 500; i++)
            {
                Console.Write(param);
            }
        }
    }
}