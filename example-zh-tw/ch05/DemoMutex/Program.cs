using System;
using System.Threading;

Console.WriteLine("示範使用具名 Mutex 偵測是否已有另一個執行個體正在執行");

// 建立一個具名的 Mutex。名稱建議使用可跨平台辨識的唯一字串以避免衝突。
// 這裡明確限制在目前使用者、目前 session，避免其他使用者或其他 session 的處理序干擾同名 Mutex。
using Mutex mutex = new Mutex(
    false,
    "com.example.myawesomeapp.single-instance.A1B2C3D4",
    new NamedWaitHandleOptions
    {
        CurrentUserOnly = true,
        CurrentSessionOnly = true
    });

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
    // 防禦性處理：如果上一個持有此 Mutex 的執行緒異常結束，
    // 而具名 Mutex 物件仍存在並且目前這個執行個體是下一個等待者，
    // WaitOne 就可能拋出 AbandonedMutexException。
    // 收到這個例外代表目前已取得 Mutex，
    // 但先前持有者可能在共享狀態尚未清理完成時就異常結束。
    Console.WriteLine("偵測到上一次應用程式未正常關閉 (Mutex 被遺棄)。");
    
    Console.WriteLine("應用程式啟動成功，按 Enter 鍵離開...");
    Console.ReadLine();
    mutex.ReleaseMutex();
}
