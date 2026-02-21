using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Ex05_AsyncAndThreads
{
    static class Program
    {
        static void Log(int num, string msg)
        {
            Console.WriteLine("({0}) T{1}: {2}", 
                num, Thread.CurrentThread.ManagedThreadId, msg);
        }

        static async Task Main()
        {
            Log(1, "正要起始非同步工作 MyDownloadPageAsync()。");

            var task = MyDownloadPageAsync("https://www.huanlintalk.com");

            Log(4, "已從 MyDownloadPageAsync() 返回，但尚未取得工作結果。");

            string content = await task;

            Log(6, "已經取得 MyDownloadPageAsync() 的結果。");

            Console.WriteLine("網頁內容總共為 {0} 個字元。", content.Length);
        }

        static async Task<string> MyDownloadPageAsync(string url)
        {
            Log(2, "正要呼叫 WebClient.DownloadStringTaskAsync()。");

            using (var webClient = new WebClient())
            {
                var task = webClient.DownloadStringTaskAsync(url);

                Log(3, "已起始非同步工作 DownloadStringTaskAsync()。");

                string content = await task;

                Log(5, "已經取得 DownloadStringTaskAsync() 的結果。");

                return content;
            }
        }
    }
}
