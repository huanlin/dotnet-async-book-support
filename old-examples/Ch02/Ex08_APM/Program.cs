using System;
using System.IO;
using System.Threading;

namespace Ex08_APM
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("=== 同步版本 ===");
            DemoSync();

            Console.WriteLine("=== 非同步版本 ===");
            DemoAsync();
            Console.WriteLine("主程式結束。");
        }

        private static void DemoSync()
        {
            using (var fs = new FileStream(@"C:\temp\foo.txt", FileMode.Open))
            {
                byte[] content = new byte[fs.Length];
                int bytesRead = fs.Read(content, 0, (int)fs.Length);    // 一次讀取整個檔案的內容。
                Console.WriteLine("一共讀取了 {0} bytes。", bytesRead);
            }
        }

        private static void DemoAsync()
        {
            using (var fs = new FileStream(@"C:\temp\foo.txt", FileMode.Open))
            {
                byte[] content = new byte[fs.Length];
                IAsyncResult ar = fs.BeginRead(content, 0, (int)fs.Length, ReadCompletionCallback, null);

                Console.WriteLine("控制流程回到主執行緒，執行其他工作...");
                Thread.Sleep(1500);   // 模擬執行其他工作需要花費的時間。

                // 等到需要取得結果時，呼叫 EndRead 方法（會 block 當前執行緒）
                int bytesRead = fs.EndRead(ar);
                Console.WriteLine("一共讀取了 {0} bytes。", bytesRead);
            }
        }

        private static void ReadCompletionCallback(IAsyncResult asyncResult)
        {
            Console.WriteLine("進入 ReadCompletionCallback() 方法。");
        }
    }
}