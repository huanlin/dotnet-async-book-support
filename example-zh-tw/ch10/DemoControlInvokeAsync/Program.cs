using System.Windows.Forms;

ApplicationConfiguration.Initialize();
Application.Run(new MainForm());

sealed class MainForm : Form
{
    private readonly Button _runButton = new()
    {
        Text = "開始背景工作",
        AutoSize = true,
        Location = new Point(20, 20)
    };

    private readonly CheckBox _throwExceptionCheckBox = new()
    {
        Text = "在 InvokeAsync 委派中故意拋出例外",
        AutoSize = true,
        Location = new Point(160, 24)
    };

    private readonly Label _statusLabel = new()
    {
        AutoSize = true,
        Location = new Point(20, 60),
        Text = "狀態：等待測試"
    };

    private readonly Label _uiThreadLabel = new()
    {
        AutoSize = true,
        Location = new Point(20, 90)
    };

    private readonly ListBox _logListBox = new()
    {
        Location = new Point(20, 125),
        Size = new Size(720, 240)
    };

    public MainForm()
    {
        Text = "DemoControlInvokeAsync";
        ClientSize = new Size(760, 390);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        Controls.AddRange(
        [
            _runButton,
            _throwExceptionCheckBox,
            _statusLabel,
            _uiThreadLabel,
            _logListBox
        ]);

        _runButton.Click += RunButton_Click;
        Shown += (_, _) =>
        {
            UpdateUiThreadLabel();
            Log("請按下「開始背景工作」觀察 Control.InvokeAsync 的行為。");
        };
    }

    private async void RunButton_Click(object? sender, EventArgs e)
    {
        _runButton.Enabled = false;
        _logListBox.Items.Clear();
        _statusLabel.Text = "狀態：背景工作執行中...";
        UpdateUiThreadLabel();

        try
        {
            Log($"按鈕事件處理常式執行於 UI 執行緒 {Environment.CurrentManagedThreadId}。");

            // 這裡刻意用 Task.Run 來模擬「我們已經在背景執行緒上」的情境。
            await Task.Run(async () =>
            {
                int startingWorkerThreadId = Environment.CurrentManagedThreadId;
                await this.InvokeAsync(() =>
                    Log($"背景工作起始於 Thread Pool 執行緒 {startingWorkerThreadId}，先做一些非 UI 工作..."));

                await Task.Delay(1500);
                int currentWorkerThreadId = Environment.CurrentManagedThreadId;
                string status = $"背景工作完成，目前背景執行緒：{currentWorkerThreadId}";

                await this.InvokeAsync(() =>
                    Log($"背景工作準備呼叫 InvokeAsync。起始執行緒：{startingWorkerThreadId}，目前執行緒：{currentWorkerThreadId}。"));

                await this.InvokeAsync(() =>
                {
                    Log($"InvokeAsync 委派目前執行於 UI 執行緒 {Environment.CurrentManagedThreadId}。");
                    _statusLabel.Text = $"狀態：{status}";
                    UpdateUiThreadLabel();

                    if (_throwExceptionCheckBox.Checked)
                    {
                        throw new InvalidOperationException("這是刻意在 UI 委派中拋出的例外。");
                    }
                });

                int afterInvokeThreadId = Environment.CurrentManagedThreadId;
                await this.InvokeAsync(() =>
                    Log($"InvokeAsync 已完成，控制權回到背景流程。目前背景執行緒：{afterInvokeThreadId}。"));
            });

            Log("外層 await 已完成，表示 UI 更新也已經完成。");
        }
        catch (Exception ex)
        {
            _statusLabel.Text = $"狀態：捕捉到例外 - {ex.Message}";
            Log($"例外已傳回呼叫端：{ex.GetType().Name} - {ex.Message}");
        }
        finally
        {
            _runButton.Enabled = true;
        }
    }

    private void UpdateUiThreadLabel()
    {
        _uiThreadLabel.Text = $"UI 執行緒 ID：{Environment.CurrentManagedThreadId}";
    }

    private void Log(string message)
    {
        _logListBox.Items.Add($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        _logListBox.TopIndex = _logListBox.Items.Count - 1;
    }
}
