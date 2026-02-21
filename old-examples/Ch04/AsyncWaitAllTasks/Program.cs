using System;
using System.Threading.Tasks;

namespace AsyncWaitAllTasks
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(DemoWaitAll);
            Task.Run(DemoWaitAllResults);

            Console.ReadLine();
        }

        private static async Task DemoWaitAll()
        {
            Console.WriteLine("DemoWaitAll() 開始");

            Task task1 = Task.Delay(1000);
            Task task2 = Task.Delay(2000);
            Task task3 = Task.Delay(3000);

            await Task.WhenAll(task1, task2, task3);

            Console.WriteLine("DemoWaitAll() 結束");
        }

        private static async Task DemoWaitAllResults()
        {
            Console.WriteLine("DemoWaitAllResults() 開始");

            Task<int> task1 = Task.FromResult(3);
            Task<int> task2 = Task.FromResult(4);
            Task<int> task3 = Task.FromResult(5);

            int[] results = await Task.WhenAll<int>(task1, task2, task3);

            foreach (int num in results)
            {
                Console.WriteLine("{0}", num);
            }
            Console.WriteLine("DemoWaitAllResults() 結束");
        }
    }
}