using System.Windows.Forms;

ApplicationConfiguration.Initialize();
Application.Run(new MainForm());

sealed class MainForm : Form
{
    private readonly Button _runButton = new()
    {
        Text = "Start background work",
        AutoSize = true,
        Location = new Point(20, 20)
    };

    private readonly CheckBox _throwExceptionCheckBox = new()
    {
        Text = "Intentionally throw in InvokeAsync",
        AutoSize = true,
        Location = new Point(180, 24)
    };

    private readonly Label _statusLabel = new()
    {
        AutoSize = true,
        Location = new Point(20, 60),
        Text = "Status: waiting for the test"
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
            Log("Click “Start background work” to observe how Control.InvokeAsync behaves.");
        };
    }

    private async void RunButton_Click(object? sender, EventArgs e)
    {
        _runButton.Enabled = false;
        _logListBox.Items.Clear();
        _statusLabel.Text = "Status: background work is running...";
        UpdateUiThreadLabel();

        try
        {
            Log($"The button event handler is running on UI thread {Environment.CurrentManagedThreadId}.");

            // Deliberately use Task.Run here to simulate the case where we are already on a background thread.
            await Task.Run(async () =>
            {
                int startingWorkerThreadId = Environment.CurrentManagedThreadId;
                await this.InvokeAsync(() =>
                    Log($"The background work started on Thread Pool thread {startingWorkerThreadId} and is doing some non-UI work first..."));

                await Task.Delay(1500);
                int currentWorkerThreadId = Environment.CurrentManagedThreadId;
                string status = $"Background work completed. Current background thread: {currentWorkerThreadId}";

                await this.InvokeAsync(() =>
                    Log($"The background work is about to call InvokeAsync. Starting thread: {startingWorkerThreadId}, current thread: {currentWorkerThreadId}."));

                await this.InvokeAsync(() =>
                {
                    Log($"The InvokeAsync delegate is now running on UI thread {Environment.CurrentManagedThreadId}.");
                    _statusLabel.Text = $"Status: {status}";
                    UpdateUiThreadLabel();

                    if (_throwExceptionCheckBox.Checked)
                    {
                        throw new InvalidOperationException("This exception was intentionally thrown inside the UI delegate.");
                    }
                });

                int afterInvokeThreadId = Environment.CurrentManagedThreadId;
                await this.InvokeAsync(() =>
                    Log($"InvokeAsync has completed, and control returned to the background flow. Current background thread: {afterInvokeThreadId}."));
            });

            Log("The outer await has completed, which means the UI update has also completed.");
        }
        catch (Exception ex)
        {
            _statusLabel.Text = $"Status: caught an exception - {ex.Message}";
            Log($"The exception was propagated back to the caller: {ex.GetType().Name} - {ex.Message}");
        }
        finally
        {
            _runButton.Enabled = true;
        }
    }

    private void UpdateUiThreadLabel()
    {
        _uiThreadLabel.Text = $"UI thread ID: {Environment.CurrentManagedThreadId}";
    }

    private void Log(string message)
    {
        _logListBox.Items.Add($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        _logListBox.TopIndex = _logListBox.Items.Count - 1;
    }
}
