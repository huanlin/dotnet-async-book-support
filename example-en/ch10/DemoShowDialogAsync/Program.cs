using System.Windows.Forms;

ApplicationConfiguration.Initialize();
Application.Run(new MainForm());

sealed class MainForm : Form
{
    private readonly Button _openDialogButton = new()
    {
        Text = "Open ShowDialogAsync sample",
        AutoSize = true,
        Location = new Point(20, 20)
    };

    private readonly Label _statusLabel = new()
    {
        AutoSize = true,
        Location = new Point(20, 60),
        Text = "Status: waiting for the test"
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
            Log("Click the button to see that once ShowDialogAsync returns a Task, the caller can continue its async flow.");
    }

    private async void OpenDialogButton_Click(object? sender, EventArgs e)
    {
        _openDialogButton.Enabled = false;
        _logListBox.Items.Clear();
        _statusLabel.Text = "Status: preparing to open the dialog";

        using var subForm = new SubForm();

        try
        {
            Log("1. Before calling ShowDialogAsync.");

            Task<DialogResult> dialogTask = subForm.ShowDialogAsync(this);

            Log("2. ShowDialogAsync returned a Task immediately.");
            _statusLabel.Text = "Status: the dialog is open, and the main method is still running";

            await DoOtherWorkWhileDialogIsOpenAsync(dialogTask);

            Log("4. Only now do we actually await the dialog result.");
            DialogResult result = await dialogTask;

            _statusLabel.Text = $"Status: the dialog is closed, and the result is {result}";
            Log($"5. The await finished, and the result is: {result}.");
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
                Log("3. The dialog was closed early, so the extra work is ending.");
                return;
            }

            Log($"3.{i} The caller can still continue other async work... ({i}/3)");
            _statusLabel.Text = $"Status: the main method is doing other async work ({i}/3)";
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
            Text = "This dialog is shown by using ShowDialogAsync."
        };

        var noteLabel = new Label
        {
            AutoSize = true,
            Location = new Point(20, 50),
            Text = "Click OK or Cancel, then return to the main form to observe the flow after await."
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
