using System.Diagnostics;

Console.WriteLine("示範：ValueTask API 設計");

var api = new UserCacheProxy();
var sw = Stopwatch.StartNew();

Console.WriteLine(await api.GetUserAsync(1));
Console.WriteLine($"第一次呼叫耗時: {sw.ElapsedMilliseconds} ms");

sw.Restart();
Console.WriteLine(await api.GetUserAsync(1));
Console.WriteLine($"第二次呼叫耗時 (從快取): {sw.ElapsedMilliseconds} ms");

public sealed class UserCacheProxy
{
    private readonly Dictionary<int, User> _cache = new();

    public ValueTask<User> GetUserAsync(int id)
    {
        if (_cache.TryGetValue(id, out var user))
        {
            Console.WriteLine("[Service] 命中快取，直接同步回傳");
            return new ValueTask<User>(user);
        }

        return new ValueTask<User>(FetchAndCacheAsync(id));
    }

    private async Task<User> FetchAndCacheAsync(int id)
    {
        Console.WriteLine("[Service] 未命中快取，發起 I/O 請求...");
        await Task.Delay(1000).ConfigureAwait(false);
        var dbUser = new User(id, $"User{id}");
        _cache[id] = dbUser;
        return dbUser;
    }
}

public sealed record User(int Id, string Name)
{
    public override string ToString() => $"{Name} (Id = {Id})";
}
