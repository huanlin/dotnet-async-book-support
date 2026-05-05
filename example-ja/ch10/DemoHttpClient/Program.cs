using System.Diagnostics;

// このサンプルでは、プログラムの存続期間全体で HttpClient を 1 回だけ作成し、
// 短時間で頻繁に作成・破棄することを避ける。
using var httpClient = new HttpClient();

Console.WriteLine("HttpClient の ResponseHeadersRead とストリーミングのデモ");

// 目的: 大きめのファイルをダウンロードする。ネットワーク変動の影響を受けにくい公開 10 MB テスト ファイルを使う。
string url = "https://proof.ovh.net/files/10Mb.dat";
string tempFilePath = Path.GetTempFileName();

Console.WriteLine($"ファイルをダウンロードする準備をしています: {url}");
Console.WriteLine($"保存先パス: {tempFilePath}");

try
{
    var sw = Stopwatch.StartNew();
    using var cancellationSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    await DownloadLargeFileAsync(url, tempFilePath, cancellationSource.Token);
    sw.Stop();
    
    // ファイル サイズを確認する
    var fileInfo = new FileInfo(tempFilePath);
    Console.WriteLine($"\nダウンロードが完了しました! 経過時間: {sw.ElapsedMilliseconds} ms, ファイル サイズ: {fileInfo.Length / 1024 / 1024} MB");
}
catch (OperationCanceledException)
{
    Console.WriteLine("失敗: ダウンロードがタイムアウトしたかキャンセルされました。ネットワークが遅い場合は、CancellationTokenSource のタイムアウトを調整してください。");
}
catch (Exception ex)
{
    Console.WriteLine($"エラーが発生しました: {ex.Message}");
}
finally
{
    // 一時ファイルをクリーンアップする
    if (File.Exists(tempFilePath))
    {
        File.Delete(tempFilePath);
        Console.WriteLine("一時ファイルを削除しました。");
    }
}

async Task DownloadLargeFileAsync(
    string fileUrl,
    string destinationPath,
    CancellationToken cancellationToken = default)
{
    // 重要な引数: HttpCompletionOption.ResponseHeadersRead
    // これは HttpClient に、HTTP ヘッダーを読み終えた時点ですぐ戻るよう指示する。
    // その後の本文読み取りは、キャンセルやタイムアウト用の CancellationToken で制御される。
    Console.WriteLine("HTTP リクエストを送信し、ヘッダーを待っています...");
    using var response = await httpClient.GetAsync(
        fileUrl,
        HttpCompletionOption.ResponseHeadersRead,
        cancellationToken);
    
    // HTTP ステータス コードが成功を示す 2xx であることを確認する
    response.EnsureSuccessStatusCode();

    // サーバーが Content-Length を提供していれば、受信ストリームの総長を取得する
    long? totalBytes = response.Content.Headers.ContentLength;
    string contentLengthText = totalBytes.HasValue
        ? $"{totalBytes.Value / 1024 / 1024} MB"
        : "提供されていません";
    Console.WriteLine($"サーバーが報告したファイル サイズ (Content-Length): {contentLengthText}");
    
    Console.WriteLine("ネットワーク ストリームから読み取り、ディスクへ継続的に書き込みます (バックプレッシャー制御付きストリーミング)...");
    
    // レスポンスからライブの非同期データ ストリームを取得する
    using var networkStream = await response.Content.ReadAsStreamAsync(cancellationToken);
    
    // ローカル ファイル ストリームを作成する
    // 重要点: useAsync: true を明示し、ランタイムが非同期 I/O パスを優先するようにする
    using var fileStream = new FileStream(
        destinationPath, 
        FileMode.Create, 
        FileAccess.Write, 
        FileShare.None, 
        bufferSize: 81920, 
        useAsync: true);

    // CopyToAsync は内部で固定サイズのメモリ バッファを自動管理する。
    // バッファがネットワークからのデータで満たされると、そのデータをディスクへ書き込む。
    // ディスク書き込み中、ディスクのほうが遅ければネットワーク読み取りは一時停止する。
    // これがバックプレッシャーであり、メモリ使用量の急増を防ぐ。
    await networkStream.CopyToAsync(fileStream, 81920, cancellationToken);
}
