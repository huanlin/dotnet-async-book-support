using System.Windows.Forms;

ApplicationConfiguration.Initialize();
Application.Run(new MainForm());

sealed class MainForm : Form
{
    private readonly Button _runButton = new()
    {
        Text = "バックグラウンド作業を開始",
        AutoSize = true,
        Location = new Point(20, 20)
    };

    private readonly CheckBox _throwExceptionCheckBox = new()
    {
        Text = "InvokeAsync 内で意図的に例外をスロー",
        AutoSize = true,
        Location = new Point(180, 24)
    };

    private readonly Label _statusLabel = new()
    {
        AutoSize = true,
        Location = new Point(20, 60),
        Text = "状態: テスト待ち"
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
            Log("「バックグラウンド作業を開始」をクリックして、Control.InvokeAsync の動作を観察します。");
        };
    }

    private async void RunButton_Click(object? sender, EventArgs e)
    {
        _runButton.Enabled = false;
        _logListBox.Items.Clear();
        _statusLabel.Text = "状態: バックグラウンド作業を実行中...";
        UpdateUiThreadLabel();

        try
        {
            Log($"ボタンのイベント ハンドラーは UI スレッド {Environment.CurrentManagedThreadId} で実行されています。");

            // すでにバックグラウンド スレッド上にいる状況をシミュレートするため、ここでは意図的に Task.Run を使う。
            await Task.Run(async () =>
            {
                int startingWorkerThreadId = Environment.CurrentManagedThreadId;
                await this.InvokeAsync(() =>
                    Log($"バックグラウンド作業はスレッド プール スレッド {startingWorkerThreadId} で開始され、まず UI 以外の作業を行っています..."));

                await Task.Delay(1500);
                int currentWorkerThreadId = Environment.CurrentManagedThreadId;
                string status = $"バックグラウンド作業が完了しました。現在のバックグラウンド スレッド: {currentWorkerThreadId}";

                await this.InvokeAsync(() =>
                    Log($"バックグラウンド作業が InvokeAsync を呼び出そうとしています。開始時のスレッド: {startingWorkerThreadId}, 現在のスレッド: {currentWorkerThreadId}。"));

                await this.InvokeAsync(() =>
                {
                    Log($"InvokeAsync デリゲートは現在 UI スレッド {Environment.CurrentManagedThreadId} で実行されています。");
                    _statusLabel.Text = $"状態: {status}";
                    UpdateUiThreadLabel();

                    if (_throwExceptionCheckBox.Checked)
                    {
                        throw new InvalidOperationException("この例外は UI デリゲート内で意図的にスローされました。");
                    }
                });

                int afterInvokeThreadId = Environment.CurrentManagedThreadId;
                await this.InvokeAsync(() =>
                    Log($"InvokeAsync が完了し、制御はバックグラウンド フローへ戻りました。現在のバックグラウンド スレッド: {afterInvokeThreadId}。"));
            });

            Log("外側の await が完了しました。つまり UI 更新も完了しています。");
        }
        catch (Exception ex)
        {
            _statusLabel.Text = $"状態: 例外をキャッチしました - {ex.Message}";
            Log($"例外が呼び出し元へ伝播されました: {ex.GetType().Name} - {ex.Message}");
        }
        finally
        {
            _runButton.Enabled = true;
        }
    }

    private void UpdateUiThreadLabel()
    {
        _uiThreadLabel.Text = $"UI スレッド ID: {Environment.CurrentManagedThreadId}";
    }

    private void Log(string message)
    {
        _logListBox.Items.Add($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        _logListBox.TopIndex = _logListBox.Items.Count - 1;
    }
}
