using System.Diagnostics;

Console.WriteLine("非同期 FileStream I/O とバックプレッシャーの考え方のデモ");

string sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
string destinationPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

try
{
    // 1. まず約 500 MB のダミー テスト ファイルを作成する
    Console.WriteLine($"テスト用のソース ファイルを作成しています ({sourcePath})...");
    CreateDummyFile(sourcePath, 500 * 1024 * 1024); // 500 MB
    
    Console.WriteLine($"非同期ファイル コピーを開始します ({destinationPath})...");
    var sw = Stopwatch.StartNew();
    
    await CopyFileWithAsyncStream(sourcePath, destinationPath);
    
    sw.Stop();
    Console.WriteLine($"\nコピーが完了しました! 経過時間: {sw.ElapsedMilliseconds} ms");
}
finally
{
    // 一時ファイルをクリーンアップするs
    if (File.Exists(sourcePath)) File.Delete(sourcePath);
    if (File.Exists(destinationPath)) File.Delete(destinationPath);
}

// 同期書き込みでテスト ファイルを生成する
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
    // 重要点 1: 読み取り用に useAsync: true を明示する
    using var sourceStream = new FileStream(
        source, 
        FileMode.Open, 
        FileAccess.Read, 
        FileShare.Read, 
        bufferSize: 81920, 
        useAsync: true);

    // 重要点 2: 書き込み用に useAsync: true を明示する
    using var destinationStream = new FileStream(
        dest, 
        FileMode.Create, 
        FileAccess.Write, 
        FileShare.None, 
        bufferSize: 81920, 
        useAsync: true);

    // 重要点 3: CopyToAsync によるバックプレッシャー制御
    // 500 MB 全体を一度に RAM へ読み込まず、81920 バイトなどの固定サイズ バッファを使う。
    // バッファを満たす -> 読み取りを一時停止する -> ディスクへ書き込む -> 書き込みを待つ -> 読み取りを続ける。
    // これにより、メモリ使用量はファイル サイズに比例して増えず、固定バッファ程度に保たれる。
    
    // CopyToAsync へ独自のバッファ サイズを渡すこともできる
    await sourceStream.CopyToAsync(destinationStream, bufferSize: 81920);
}
