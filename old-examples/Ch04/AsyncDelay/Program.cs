using System;
using System.Threading.Tasks;

namespace AsyncDelay
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var task = MyTaskAsync();
            Console.WriteLine("已經返回 Main()");
            await task;
            Console.ReadLine();
        }

        private static async Task MyTaskAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            Console.WriteLine("非同步工作結束");
        }
    }
}