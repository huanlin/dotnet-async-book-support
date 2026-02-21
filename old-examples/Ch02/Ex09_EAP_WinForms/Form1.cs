using System.Net;

namespace Ex09_EAP_WinForms2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Log(string s, params object[] args)
        {
            listBox1.Items.Add(string.Format(s, args));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Log("UI 執行緒 ID: {0}", Thread.CurrentThread.ManagedThreadId);
            using (var client = new WebClient())
            {
                client.DownloadStringCompleted += WebDownloadStringCompleted;
                client.DownloadStringAsync(new Uri("http://huan-lin.blogspot.com"));
            }
        }
        private void WebDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Note: 可以在這裡撰寫更新 UI 的程式碼，而無須額外寫程式切換至 UI 執行緒。
            Log("非同步工作完成事件觸發於執行緒 ID: {0}", Thread.CurrentThread.ManagedThreadId);
            label1.Text = string.Format("下載的網頁內容長度為 {0} 字元。", e.Result.Length);
        }
    }
}
