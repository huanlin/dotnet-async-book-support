using System;
using System.Threading;

namespace Ex05_Lock
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            new SharedStateDemo().Run();
            Console.ReadLine();
        }
    }

    public class SharedStateDemo
    {
        private int itemCount = 0;
        private object locker = new Object(); // 用於獨佔鎖定的物件

        public void Run()
        {
            var t1 = new Thread(AddToCart);
            var t2 = new Thread(AddToCart);

            t1.Start(300);
            t2.Start(100);
        }

        private void AddToCart(object simulateDelay)
        {
            Console.WriteLine("Enter thread {0}", // 顯示目前所在的執行緒編號
                Thread.CurrentThread.ManagedThreadId);
            lock (locker)  // 利用 locker 物件來鎖定程式區塊
            {
                itemCount++;

                Thread.Sleep((int)simulateDelay);
                Console.WriteLine("Items in cart: {0} on thread {1}",
                    itemCount, Thread.CurrentThread.ManagedThreadId);
            }
        }
    }
}