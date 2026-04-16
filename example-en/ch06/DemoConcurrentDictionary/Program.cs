using System.Collections.Concurrent;

DemoGetOrAddTrap();

Console.WriteLine("\n-------------------------------------------------------------");

DemoGetOrAddWithLazy();

static void DemoGetOrAddTrap()
{
    Console.WriteLine(
        "Demo: ConcurrentDictionary.GetOrAdd and valueFactory behavior");

    var cache = new ConcurrentDictionary<string, string>();

    // Two threads try to load data for the same key at the same time.
    Task.WaitAll(
        Task.Run(() => cache.GetOrAdd("user:1", key => LoadDataFromDb(key))),
        Task.Run(() => cache.GetOrAdd("user:1", key => LoadDataFromDb(key)))
    );

    // Trap: even though only one final value will be stored in the dictionary,
    // under contention LoadDataFromDb can still run twice.
    Console.WriteLine($"Value in cache: {cache["user:1"]}");

    static string LoadDataFromDb(string key)
    {
        Console.WriteLine($"Loading data for {key} from the database...");
        // This sample intentionally simulates synchronous work,
        // so Thread.Sleep is used here.
        // Writing Task.Delay(...).Wait() would only create the discouraged
        // sync-over-async pattern instead.
        Thread.Sleep(100);
        return "Some Data";
    }
}

static void DemoGetOrAddWithLazy()
{
    Console.WriteLine(
        "Common solution: delay the real initialization with Lazy<T>");

    var cache = new ConcurrentDictionary<string, Lazy<string>>();

    Task.WaitAll(
        Task.Run(() => GetCachedData("user:1")),
        Task.Run(() => GetCachedData("user:1"))
    );

    Console.WriteLine($"Value in cache: {cache["user:1"].Value}");

    string GetCachedData(string key)
    {
        // The factory creates only a lightweight Lazy<T> wrapper.
        // The real initialization is delayed until the surviving Lazy<T>.Value
        // is actually read.
        var lazyResult = cache.GetOrAdd(key,
            k => new Lazy<string>(
                () => LoadDataFromDb(k),
                LazyThreadSafetyMode.ExecutionAndPublication));

        // LoadDataFromDb runs only when Value is read,
        // and the surviving Lazy<T> ensures the initialization happens once.
        return lazyResult.Value;
    }

    static string LoadDataFromDb(string key)
    {
        Console.WriteLine($"Loading data for {key} from the database...");
        // Same idea as above: this sample is not using an async workflow here.
        Thread.Sleep(100);
        return "Safe Data";
    }
}
