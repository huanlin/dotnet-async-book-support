using System.Windows;

namespace DemoAsyncMvvmWpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(new FakeDataService());
    }
}
