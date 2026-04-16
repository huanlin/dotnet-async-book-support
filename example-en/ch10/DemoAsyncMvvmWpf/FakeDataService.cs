namespace DemoAsyncMvvmWpf;

public sealed class FakeDataService : IDataService
{
    public async Task<string> FetchDataAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1500, cancellationToken);

        return """
            The DataService has completed its simulated I/O-bound work.
            You can imagine this data as something loaded from a Web API or a database.
            While the operation was waiting, the UI stayed responsive, and the button was temporarily disabled because of IsBusy.
            """;
    }
}
