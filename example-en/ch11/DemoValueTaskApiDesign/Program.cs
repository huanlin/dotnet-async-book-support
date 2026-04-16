using System.Diagnostics;

Console.WriteLine("Demonstrating ValueTask API design");

var api = new UserCacheProxy();
var sw = Stopwatch.StartNew();

Console.WriteLine(await api.GetUserAsync(1));
Console.WriteLine($"First call elapsed time: {sw.ElapsedMilliseconds} ms");

sw.Restart();
Console.WriteLine(await api.GetUserAsync(1));
Console.WriteLine($"Second call elapsed time (from cache): {sw.ElapsedMilliseconds} ms");

public sealed class UserCacheProxy
{
    private readonly Dictionary<int, User> _cache = new();

    public ValueTask<User> GetUserAsync(int id)
    {
        if (_cache.TryGetValue(id, out var user))
        {
            Console.WriteLine("[Service] Cache hit; returning synchronously");
            return new ValueTask<User>(user);
        }

        return new ValueTask<User>(FetchAndCacheAsync(id));
    }

    private async Task<User> FetchAndCacheAsync(int id)
    {
        Console.WriteLine("[Service] Cache miss; starting the I/O request...");
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
