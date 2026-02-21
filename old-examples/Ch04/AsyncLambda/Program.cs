using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsyncLambda
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // 此委派方法需傳入一個 string，且回傳一個 Task<string>。
            Func<string, Task<string>> downloadPageAsync =
                async (string url) =>       // 非同步 Lambda
                {
                    using (var client = new HttpClient())
                    {
                        return await client.GetStringAsync(url);
                    }
                };

            var task = downloadPageAsync("http://www.huan-lin.blogspot.com");

            task.Wait(); // 等待工作執行完畢。

            Console.WriteLine("網頁長度: {0}", task.Result.Length);
        }
    }
}