using System;
using System.Net;
using System.Threading.Tasks;

namespace Ex02_Async
{
    static class Program
    {
        static void Main()
        {
            var task = MyDownloadPageAsync("https://www.huanlintalk.com");
            string content = task.Result;
            Console.WriteLine("網頁內容總共為 {0} 個字元。", content.Length);
        }

        static async Task<string> MyDownloadPageAsync(string url)
        {
            using (var wc = new WebClient())
            {
                var task = wc.DownloadStringTaskAsync(url);
                string content = await task;
                return content;
            }
        }
    }
}
