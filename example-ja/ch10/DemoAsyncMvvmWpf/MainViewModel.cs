using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DemoAsyncMvvmWpf;

public partial class MainViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private string _loadedData = "まだデータは読み込まれていません。";

    [ObservableProperty]
    private string _statusMessage = "状態: 読み込み待ち";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadDataCommand))]
    private bool _isBusy;

    public ObservableCollection<string> LogEntries { get; } = [];

    public IAsyncRelayCommand LoadDataCommand { get; }

    public MainViewModel(IDataService dataService)
    {
        _dataService = dataService;
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync, CanLoadData);
        LogEntries.Add("ウィンドウの準備ができました。「データを読み込む」をクリックしてサンプルを開始してください。");
    }

    private bool CanLoadData() => !IsBusy;

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        StatusMessage = "状態: データを読み込み中...";
        LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] LoadDataAsync を開始しました。");

        try
        {
            string data = await _dataService.FetchDataAsync();
            LoadedData = data;
            StatusMessage = "状態: データの読み込みが完了しました";
            LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] データを取得し、UI を更新しました。");
        }
        catch (Exception ex)
        {
            StatusMessage = $"状態: エラーが発生しました - {ex.Message}";
            LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] 例外: {ex.GetType().Name} - {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            LogEntries.Add($"[{DateTime.Now:HH:mm:ss.fff}] LoadDataAsync が完了し、IsBusy を元に戻しました。");
        }
    }
}
