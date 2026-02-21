using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsyncWaitAllTasks
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(DemoWaitAny);
            Console.ReadLine();
        }

        private static async Task DemoWaitAny()
        {
            Console.WriteLine("DemoWaitAny() 開始");

            var client = new HttpClient();

            Task<string> task1 = client.GetStringAsync("http://microsoft.com");
            Task<string> task2 = client.GetStringAsync("http://facebook.com");
            Task<string> task3 = client.GetStringAsync("http://google.com");

            Task<string> completedTask = await Task.WhenAny(task1, task2, task3);

            string content = await completedTask;
            Console.WriteLine("網頁長度: " + content.Length);
            Console.WriteLine("DemoWaitAny() 結束");
        }
    }
}