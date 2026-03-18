using System.Windows.Forms;

ApplicationConfiguration.Initialize();
Application.Run(new MainForm());

sealed class MainForm : Form
{
    private readonly Button _openDialogButton = new()
    {
        Text = "開啟 ShowDialogAsync 範例",
        AutoSize = true,
        Location = new Point(20, 20)
    };

    private readonly Label _statusLabel = new()
    {
        AutoSize = true,
        Location = new Point(20, 60),
        Text = "狀態：等待測試"
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
            Log("請按下按鈕，觀察 ShowDialogAsync 回傳 Task 後，呼叫端仍可繼續執行 async 流程。");
    }

    private async void OpenDialogButton_Click(object? sender, EventArgs e)
    {
        _openDialogButton.Enabled = false;
        _logListBox.Items.Clear();
        _statusLabel.Text = "狀態：準備開啟對話方塊";

        using var subForm = new SubForm();

        try
        {
            Log("1. 呼叫 ShowDialogAsync 之前。");

            Task<DialogResult> dialogTask = subForm.ShowDialogAsync(this);

            Log("2. ShowDialogAsync 已立即回傳 Task。");
            _statusLabel.Text = "狀態：對話方塊已開啟，主方法繼續執行中";

            await DoOtherWorkWhileDialogIsOpenAsync(dialogTask);

            Log("4. 現在才真正等待對話方塊的結果。");
            DialogResult result = await dialogTask;

            _statusLabel.Text = $"狀態：對話方塊已關閉，結果是 {result}";
            Log($"5. await 結束，收到結果：{result}。");
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
                Log("3. 對話方塊已提早關閉，結束額外工作。");
                return;
            }

            Log($"3.{i} 呼叫端方法仍可繼續做其他 async 工作... ({i}/3)");
            _statusLabel.Text = $"狀態：主方法正在執行其他 async 工作 ({i}/3)";
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
        Text = "SubForm";
        ClientSize = new Size(380, 170);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        var descriptionLabel = new Label
        {
            AutoSize = true,
            Location = new Point(20, 20),
            Text = "這是一個使用 ShowDialogAsync 顯示的對話方塊。"
        };

        var noteLabel = new Label
        {
            AutoSize = true,
            Location = new Point(20, 50),
            Text = "請按 OK 或 Cancel，回到主表單觀察 await 後續流程。"
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
            Text = "Cancel",
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
