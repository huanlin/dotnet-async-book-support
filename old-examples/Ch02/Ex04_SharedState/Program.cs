using System;
using System.Threading;

namespace Ex04_SharedState
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
        private int itemCount = 0;   // 已加入購物車的商品數量。

        public void Run()
        {
            var t1 = new Thread(AddToCart);
            var t2 = new Thread(AddToCart);

            t1.Start(300); // 試試改傳入 0
            t2.Start(100);
        }

        private void AddToCart(object simulateDelay)
        {
            itemCount++;

            /*
             * 用 Thread.Sleep 來模擬這項工作所花的時間，時間長短
             * 由呼叫端傳入的 simulateDelay 參數指定，以便藉由改變
             * 此參數來觀察共享變數值的變化。
             */
            Thread.Sleep((int)simulateDelay);
            Console.WriteLine("Items in cart: {0}", itemCount);
        }
    }
}