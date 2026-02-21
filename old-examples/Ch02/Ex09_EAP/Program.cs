using System;
using System.Net;
using System.Threading;

namespace Ex09_EAP
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("=== 同步版本 ===");
            DemoSync();

            Console.WriteLine("=== 非同步版本 ===");
            DemoAsync();
        }

        private static void DemoSync()
        {
            using (var client = new WebClient())
            {
                string result = client.DownloadString(new Uri("http://huan-lin.blogspot.com"));
                Console.WriteLine("下載的網頁內容長度為 {0} 字元。", result.Length);
            }
        }

        private static void DemoAsync()
        {
            using (var client = new WebClient())
            {
                client.DownloadStringCompleted += WebDownloadStringCompleted;
                client.DownloadStringAsync(new Uri("http://huan-lin.blogspot.com"));

                Console.WriteLine("控制流程回到主執行緒，執行其他工作...");
                Thread.Sleep(2000);   // 模擬執行其他工作需要花費的時間。
            }
        }

        private static void WebDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Note: 可以在這裡撰寫更新 UI 的程式碼，而無須額外寫程式切換至 UI 執行緒（參考 Ex09_EAP_WinForms.csproj）。
            if (e.Cancelled)
            {
                Console.WriteLine("非同步工作已取消!");
            }
            else if (e.Error != null)
            {
                Console.WriteLine("非同步工作發生錯誤：" + e.Error.Message);
            }
            else
            {
                Console.WriteLine("下載的網頁內容長度為 {0} 字元。", e.Result.Length);
            }
        }
    }
}