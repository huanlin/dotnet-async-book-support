using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DemoAsyncMvvmWpf;

public partial class MainViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private string _loadedData = "尚未載入資料。";

    [ObservableProperty]
    private string _statusMessage = "狀態：等待載入";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadDataCommand))]
    private bool _isBusy;

    public ObservableCollection<string> LogEntries { get; } = [];

    public IAsyncRelayCommand LoadDataCommand { get; }

    public MainViewModel(IDataService dataService)
    {
        _dataService = dataService;
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync, CanLoadData);
        LogEntries.Add("畫面初始化完成，按下「載入資料」開始測試。");
    }

    private bool CanLoadData() => !IsBusy;

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        StatusMessage = "狀態：正在載入資料...";
        LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] LoadDataAsync 開始。");

        try
        {
            string data = await _dataService.FetchDataAsync();
            LoadedData = data;
            StatusMessage = "狀態：資料載入完成";
            LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] 已取得資料並更新畫面。");
        }
        catch (Exception ex)
        {
            StatusMessage = $"狀態：發生錯誤 - {ex.Message}";
            LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] 例外：{ex.GetType().Name} - {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] LoadDataAsync 結束，IsBusy 已恢復。");
        }
    }
}
