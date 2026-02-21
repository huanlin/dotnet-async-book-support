using System;
using System.Net;

namespace Ex01_Sync
{
    static class Program
    {
        static void Main()
        {
            string content = MyDownloadPage("https://www.huanlintalk.com");

            Console.WriteLine("網頁內容總共為 {0} 個字元。", content.Length);
        }

        static string MyDownloadPage(string url)
        {
            using (var wc = new WebClient())
            {
                return wc.DownloadString(url);
            }
        }
    }
}
