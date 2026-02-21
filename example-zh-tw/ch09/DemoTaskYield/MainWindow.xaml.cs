using System;
using System.Threading.Tasks;
using System.Windows;

namespace DemoTaskYield
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnProcessButtonClick(object sender, RoutedEventArgs e)
        {
            ProcessButton.IsEnabled = false;
            StatusText.Text = "處理中...";
            
            for (int i = 0; i < 100_000; i++)
            {
                // ... 執行一些複雜的同步計算 ...
                double result = Math.Sqrt(i) * Math.Sin(i);
                
                if (i % 1000 == 0)
                {
                    // 讓 UI 有機會更新、回應使用者輸入
                    StatusText.Text = $"處理中... {i} / 100000";
                    await Task.Yield();
                }
            }
            
            StatusText.Text = "處理完成！";
            ProcessButton.IsEnabled = true;
        }
    }
}