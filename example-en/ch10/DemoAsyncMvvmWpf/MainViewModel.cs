using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DemoAsyncMvvmWpf;

public partial class MainViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private string _loadedData = "No data has been loaded yet.";

    [ObservableProperty]
    private string _statusMessage = "Status: waiting to load";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadDataCommand))]
    private bool _isBusy;

    public ObservableCollection<string> LogEntries { get; } = [];

    public IAsyncRelayCommand LoadDataCommand { get; }

    public MainViewModel(IDataService dataService)
    {
        _dataService = dataService;
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync, CanLoadData);
        LogEntries.Add("The window is ready. Click “Load data” to start the sample.");
    }

    private bool CanLoadData() => !IsBusy;

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        StatusMessage = "Status: loading data...";
        LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] LoadDataAsync started.");

        try
        {
            string data = await _dataService.FetchDataAsync();
            LoadedData = data;
            StatusMessage = "Status: data load completed";
            LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] Data was retrieved and the UI was updated.");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Status: an error occurred - {ex.Message}";
            LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] Exception: {ex.GetType().Name} - {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] LoadDataAsync finished, and IsBusy was restored.");
        }
    }
}
