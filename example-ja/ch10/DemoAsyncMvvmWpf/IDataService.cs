namespace DemoAsyncMvvmWpf;

public interface IDataService
{
    Task<string> FetchDataAsync(CancellationToken cancellationToken = default);
}
