using System;
using System.Threading;

Console.WriteLine("示範使用 Mutex 避免應用程式重複開啟 (Single Instance)");

// 建立一個有命名的 Mutex。名稱建議使用可跨平台辨識的唯一字串以避免衝突。
using Mutex mutex = new Mutex(false, "com.example.myawesomeapp.single-instance.A1B2C3D4");

// 嘗試獲取鎖，等待 0 毫秒（立即回傳結果）
if (!mutex.WaitOne(0))
{
    Console.WriteLine("應用程式已經在執行中了！請勿重複開啟。");
    return; // 離開應用程式
}

try
{
    Console.WriteLine("應用程式啟動成功，按 Enter 鍵離開...");
    Console.ReadLine();
}
finally
{
    // 確保離開時釋放 Mutex
    mutex.ReleaseMutex();
}
