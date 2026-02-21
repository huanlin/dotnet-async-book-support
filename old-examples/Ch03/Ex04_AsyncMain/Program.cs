using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Ex04_AsyncMain
{
    static class Program
    {
        static async Task Main()
        {
            var url = "https://www.huanlintalk.com";
            var content = await MyDownloadPageAsync(url);
            Console.WriteLine("網頁內容總共為 {0} 個字元。", content.Length);
        }

        static async Task<string> MyDownloadPageAsync(string url)
        {
            using (var wc = new WebClient())
            {
                return await wc.DownloadStringTaskAsync(url);
            }
        }
    }
}
