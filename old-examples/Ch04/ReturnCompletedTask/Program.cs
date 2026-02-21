using System;
using System.Threading.Tasks;

namespace ReturnCompletedTask
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                IMyCache myCache = new MyCache();
                var s = await myCache.GetDataAsync();
                System.Console.WriteLine(s);
            });

            Console.ReadLine();
        }
    }

    internal interface IMyCache
    {
        Task<string> GetDataAsync();
    }

    internal class MyCache : IMyCache
    {
        public Task<string> GetDataAsync()
        {
            return Task.FromResult("hello");
        }

        public Task<string> GetDataAsync_BeforeDotNet45()
        {
            var tcs = new TaskCompletionSource<string>();
            tcs.SetResult("hello");
            return tcs.Task;
        }
    }
}