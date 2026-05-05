using System.Diagnostics;

Console.WriteLine("ValueTask API 設計のデモ");

var api = new UserCacheProxy();
var sw = Stopwatch.StartNew();

Console.WriteLine(await api.GetUserAsync(1));
Console.WriteLine($"1 回目の呼び出しの経過時間: {sw.ElapsedMilliseconds} ms");

sw.Restart();
Console.WriteLine(await api.GetUserAsync(1));
Console.WriteLine($"2 回目の呼び出しの経過時間 (キャッシュから): {sw.ElapsedMilliseconds} ms");

public sealed class UserCacheProxy
{
    private readonly Dictionary<int, User> _cache = new();

    public ValueTask<User> GetUserAsync(int id)
    {
        if (_cache.TryGetValue(id, out var user))
        {
            Console.WriteLine("[Service] キャッシュ ヒット。同期的に返します");
            return new ValueTask<User>(user);
        }

        return new ValueTask<User>(FetchAndCacheAsync(id));
    }

    private async Task<User> FetchAndCacheAsync(int id)
    {
        Console.WriteLine("[Service] キャッシュ ミス。I/O リクエストを開始します...");
        await Task.Delay(1000).ConfigureAwait(false);
        var dbUser = new User(id, $"ユーザー{id}");
        _cache[id] = dbUser;
        return dbUser;
    }
}

public sealed record User(int Id, string Name)
{
    public override string ToString() => $"{Name} (Id = {Id})";
}
