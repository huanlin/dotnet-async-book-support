using System.Threading;

namespace Ex06_BackgroundThread
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Thread t = new Thread(MyTask);
            t.IsBackground = true;
            t.Start();

            // 若 t 是前景執行緒，此應用程式不會結束，除非手動將它關閉;
            // 若 t 是背景執行緒，此應用程式會立刻結束。
        }

        private static void MyTask()
        {
            while (true)
                ;
        }
    }
}