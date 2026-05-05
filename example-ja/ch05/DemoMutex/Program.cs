using System;
using System.Threading;

Console.WriteLine(
    "デモ: 名前付き Mutex を使って、別のインスタンスがすでに実行中かどうかを検出します。");

// 名前付きミューテックスを作成する。名前はクロスプラットフォームで扱いやすい一意な
// 衝突を避けられる文字列にする。
// 現在のユーザーと現在のセッションに明示的に限定し、
// ほかのユーザーやセッションのプロセスが同じミューテックス名に干渉できないようにする。
using Mutex mutex = new Mutex(
    false,
    "com.example.myawesomeapp.single-instance.A1B2C3D4",
    new NamedWaitHandleOptions
    {
        CurrentUserOnly = true,
        CurrentSessionOnly = true
    });

// ロックの取得を試みる。待機時間は 0 ミリ秒にして、
// 呼び出しがすぐ戻るようにする。
try
{
    if (!mutex.WaitOne(0))
    {
        Console.WriteLine(
            "アプリケーションはすでに実行中です。二重に起動しないでください。");
        return; // アプリケーションを終了する
    }

    try
    {
        Console.WriteLine(
            "アプリケーションが正常に起動しました。終了するには Enter キーを押してください...");
        Console.ReadLine();
    }
    finally
    {
        // 抜けるときにミューテックスが必ず解放されるようにする。
        mutex.ReleaseMutex();
    }
}
catch (AbandonedMutexException)
{
    // 防御的な処理: ミューテックスの前の所有者が異常終了し、
    // 待機者や開いたハンドルがあったため名前付きミューテックス オブジェクトが生き残っていた場合、
    // 次に成功した待機者は
    // AbandonedMutexException を受け取る可能性がある。
    // この例外を受け取ると、現在のインスタンスがミューテックスを所有したことになるが、
    // 保護対象の共有状態がまだ安全であることは保証されず、
    // 一貫していることを保証しない。
    Console.WriteLine(
        "前回のアプリケーション インスタンスが正常に終了しなかったことを検出しました (放棄されたミューテックス)。");

    // ... ここに状態検証や修復ロジックを置ける ...
    try
    {
        Console.WriteLine(
            "アプリケーションが正常に起動しました。終了するには Enter キーを押してください...");
        Console.ReadLine();
    }
    finally
    {
        mutex.ReleaseMutex();
    }
}
