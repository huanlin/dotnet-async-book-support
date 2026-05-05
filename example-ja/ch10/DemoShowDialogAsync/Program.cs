using System.Windows.Forms;

ApplicationConfiguration.Initialize();
Application.Run(new MainForm());

sealed class MainForm : Form
{
    private readonly Button _openDialogButton = new()
    {
        Text = "ShowDialogAsync サンプルを開く",
        AutoSize = true,
        Location = new Point(20, 20)
    };

    private readonly Label _statusLabel = new()
    {
        AutoSize = true,
        Location = new Point(20, 60),
        Text = "状態: テスト待ち"
    };

    private readonly ListBox _logListBox = new()
    {
        Location = new Point(20, 95),
        Size = new Size(720, 250)
    };

    public MainForm()
    {
        Text = "DemoShowDialogAsync";
        ClientSize = new Size(760, 370);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        Controls.AddRange(
        [
            _openDialogButton,
            _statusLabel,
            _logListBox
        ]);

        _openDialogButton.Click += OpenDialogButton_Click;
        Shown += (_, _) =>
            Log("ボタンをクリックすると、ShowDialogAsync が Task を返したあと、呼び出し元が非同期フローを継続できることを確認できます。");
    }

    private async void OpenDialogButton_Click(object? sender, EventArgs e)
    {
        _openDialogButton.Enabled = false;
        _logListBox.Items.Clear();
        _statusLabel.Text = "状態: ダイアログを開く準備中";

        using var subForm = new SubForm();

        try
        {
            Log("1. ShowDialogAsync を呼び出す前。");

            Task<DialogResult> dialogTask = subForm.ShowDialogAsync(this);

            Log("2. ShowDialogAsync はすぐに Task を返しました。");
            _statusLabel.Text = "状態: ダイアログは開いており、メイン メソッドはまだ実行中です";

            await DoOtherWorkWhileDialogIsOpenAsync(dialogTask);

            Log("4. ここで初めてダイアログの結果を実際に await します。");
            DialogResult result = await dialogTask;

            _statusLabel.Text = $"状態: ダイアログが閉じられました。結果は {result} です";
            Log($"5. await が完了し、結果は {result} です。");
        }
        finally
        {
            _openDialogButton.Enabled = true;
        }
    }

    private async Task DoOtherWorkWhileDialogIsOpenAsync(Task dialogTask)
    {
        for (int i = 1; i <= 3; i++)
        {
            if (dialogTask.IsCompleted)
            {
                Log("3. ダイアログが早めに閉じられたため、追加作業を終了します。");
                return;
            }

            Log($"3.{i} 呼び出し元はまだ別の非同期作業を続けられます... ({i}/3)");
            _statusLabel.Text = $"状態: メイン メソッドは別の非同期作業を実行中です ({i}/3)";
            await Task.Delay(1000);
        }
    }

    private void Log(string message)
    {
        _logListBox.Items.Add($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        _logListBox.TopIndex = _logListBox.Items.Count - 1;
    }
}

sealed class SubForm : Form
{
    public SubForm()
    {
        Text = "サブフォーム";
        ClientSize = new Size(380, 170);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        var descriptionLabel = new Label
        {
            AutoSize = true,
            Location = new Point(20, 20),
            Text = "このダイアログは ShowDialogAsync を使って表示されています。"
        };

        var noteLabel = new Label
        {
            AutoSize = true,
            Location = new Point(20, 50),
            Text = "OK またはキャンセルをクリックし、メイン フォームに戻って await 後の流れを確認してください。"
        };

        var okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(190, 100),
            Size = new Size(75, 30)
        };

        var cancelButton = new Button
        {
            Text = "キャンセル",
            DialogResult = DialogResult.Cancel,
            Location = new Point(280, 100),
            Size = new Size(75, 30)
        };

        AcceptButton = okButton;
        CancelButton = cancelButton;

        Controls.AddRange(
        [
            descriptionLabel,
            noteLabel,
            okButton,
            cancelButton
        ]);
    }
}
