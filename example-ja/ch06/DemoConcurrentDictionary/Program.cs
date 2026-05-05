using System.Collections.Concurrent;

DemoGetOrAddTrap();

Console.WriteLine("\n-------------------------------------------------------------");

DemoGetOrAddWithLazy();

static void DemoGetOrAddTrap()
{
    Console.WriteLine(
        "デモ: ConcurrentDictionary.GetOrAdd と valueFactory の動作");

    var cache = new ConcurrentDictionary<string, string>();

    // 2 本のスレッドが同じキーのデータを同時に読み込もうとする。
    Task.WaitAll(
        Task.Run(() => cache.GetOrAdd("user:1", key => LoadDataFromDb(key))),
        Task.Run(() => cache.GetOrAdd("user:1", key => LoadDataFromDb(key)))
    );

    // 罠: ディクショナリに最終的に保存される値は 1 つだけでも、
    // 競合時には LoadDataFromDb が 2 回実行されることがある。
    Console.WriteLine($"キャッシュ内の値: {cache["user:1"]}");

    static string LoadDataFromDb(string key)
    {
        Console.WriteLine($"{key} のデータをデータベースから読み込んでいます...");
        // このサンプルでは意図的に同期作業をシミュレートするため、
        // ここでは Thread.Sleep を使っている。
        // Task.Delay(...).Wait() と書くと、非推奨の
        // sync-over-async パターンを作るだけになる。
        Thread.Sleep(100);
        return "何らかのデータ";
    }
}

static void DemoGetOrAddWithLazy()
{
    Console.WriteLine(
        "一般的な解決策: Lazy<T> で実際の初期化を遅延させる");

    var cache = new ConcurrentDictionary<string, Lazy<string>>();

    Task.WaitAll(
        Task.Run(() => GetCachedData("user:1")),
        Task.Run(() => GetCachedData("user:1"))
    );

    Console.WriteLine($"キャッシュ内の値: {cache["user:1"].Value}");

    string GetCachedData(string key)
    {
        // ファクトリは軽量な Lazy<T> ラッパーだけを作成する。
        // 実際の初期化は、生き残った Lazy<T>.Value が
        // 実際に読まれるまで遅延される。
        var lazyResult = cache.GetOrAdd(key,
            k => new Lazy<string>(
                () => LoadDataFromDb(k),
                LazyThreadSafetyMode.ExecutionAndPublication));

        // LoadDataFromDb は Value が読まれたときだけ実行され、
        // 生き残った Lazy<T> が初期化を 1 回だけにしてくれる。
        return lazyResult.Value;
    }

    static string LoadDataFromDb(string key)
    {
        Console.WriteLine($"{key} のデータをデータベースから読み込んでいます...");
        // 上と同じ考え方で、このサンプルでは async ワークフローを使っていない。
        Thread.Sleep(100);
        return "安全なデータ";
    }
}
