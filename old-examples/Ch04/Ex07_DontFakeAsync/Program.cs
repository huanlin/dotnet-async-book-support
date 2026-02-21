using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ex07_DontFakeAsync
{
    class Program
    {
        static void Main(string[] args)
        {
            var t1 = MyDownloadPageAsync("http://www.google.com");
            t1.Wait();

            var t2 = MyDownloadPageFakedAsync("http://www.google.com");
            t2.Wait();
        }

        static async Task MyDownloadPageAsync(string url)
        {
            var client = new HttpClient();
            string content = await client.GetStringAsync(url);
            Console.WriteLine("網頁長度: " + content.Length);
        }

        static async Task MyDownloadPageFakedAsync(string url)
        {
            var client = new WebClient();
            var task = Task.Run(() =>
            {
                string content = client.DownloadString(url);
                Console.WriteLine("網頁長度: " + content.Length);
            });
            await task;
        }
    }
}
