using System;
using System.Threading;

Console.WriteLine("示範使用 Mutex 避免應用程式重複開啟 (Single Instance)");

// 建立一個有命名的 Mutex。名稱建議使用可跨平台辨識的唯一字串以避免衝突。
using Mutex mutex = new Mutex(false, "com.example.myawesomeapp.single-instance.A1B2C3D4");

// 嘗試獲取鎖，等待 0 毫秒（立即回傳結果）
try
{
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
}
catch (AbandonedMutexException)
{
    // 關鍵陷阱處理：如果上一個持有此 Mutex 的處理序異常崩潰 (Crash)
    // 來不及呼叫 ReleaseMutex，下一個嘗試 WaitOne 的應用程式
    // 就會收到 AbandonedMutexException。
    // 收到這個例外代表我們依然成功「接手」了這個鎖，可以繼續執行！
    Console.WriteLine("偵測到上一次應用程式未正常關閉 (Mutex 被遺棄)，已安全接手！");
    
    Console.WriteLine("應用程式啟動成功，按 Enter 鍵離開...");
    Console.ReadLine();
    mutex.ReleaseMutex();
}
