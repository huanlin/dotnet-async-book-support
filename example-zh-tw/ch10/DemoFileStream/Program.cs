using System.Diagnostics;

Console.WriteLine("示範 FileStream 非同步讀寫與 Backpressure (背壓) 概念");

string sourcePath = Path.GetTempFileName();
string destinationPath = Path.GetTempFileName();

try
{
    // 1. 先產生一個約 500 MB 的垃圾測試檔案
    Console.WriteLine($"創造測試用來源檔案 ({sourcePath})...");
    CreateDummyFile(sourcePath, 500 * 1024 * 1024); // 500 MB
    
    Console.WriteLine($"開始非同步複製檔案至 ({destinationPath})...");
    var sw = Stopwatch.StartNew();
    
    await CopyFileWithAsyncStream(sourcePath, destinationPath);
    
    sw.Stop();
    Console.WriteLine($"\n✅ 複製完成！耗時: {sw.ElapsedMilliseconds} ms");
}
finally
{
    // 清理暫存檔
    if (File.Exists(sourcePath)) File.Delete(sourcePath);
    if (File.Exists(destinationPath)) File.Delete(destinationPath);
}

// 產生測試檔案 (同步寫入)
void CreateDummyFile(string path, int sizeInBytes)
{
    var buffer = new byte[81920];
    new Random().NextBytes(buffer);
    
    using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
    for (int i = 0; i < sizeInBytes / buffer.Length; i++)
    {
        fs.Write(buffer, 0, buffer.Length);
    }
}

async Task CopyFileWithAsyncStream(string source, string dest)
{
    // ★ 關鍵點一：讀取時開啟 useAsync: true
    using var sourceStream = new FileStream(
        source, 
        FileMode.Open, 
        FileAccess.Read, 
        FileShare.Read, 
        bufferSize: 81920, 
        useAsync: true);

    // ★ 關鍵點二：寫入時開啟 useAsync: true
    using var destinationStream = new FileStream(
        dest, 
        FileMode.Create, 
        FileAccess.Write, 
        FileShare.None, 
        bufferSize: 81920, 
        useAsync: true);

    // ★ 關鍵點三：CopyToAsync 提供的背壓 (Backpressure) 控制
    // 它不會一次把 500MB 全讀進 RAM，而是配置一塊 buffer (例如 81920 bytes)。
    // 讀滿 buffer -> 暫停讀取 -> 觸發寫入硬碟 -> 等待寫入完成 -> 繼續讀取網路/來源。
    // 這讓程式的記憶體使用量維持完美的一條水平線 (Flat line)，即使來源與目標的速度差異極大也能安然度過。
    
    // 我們可以傳入自己定義的 buffer size ให้ CopyToAsync
    await sourceStream.CopyToAsync(destinationStream, bufferSize: 81920);
}
