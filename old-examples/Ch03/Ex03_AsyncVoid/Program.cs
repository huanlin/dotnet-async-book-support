using System;
using System.Threading.Tasks;

namespace Ex03_AsyncVoid
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TestVoidAsync();
            }
            catch (Exception ex) // 捕捉不到 TestVoidAsync 方法所拋出的異常!
            {
                Console.WriteLine(ex);  
            }
        }

        static async void TestVoidAsync()
        {
            await Task.Delay(0); // 只是為了讓此方法有用到 await 陳述式。
            throw new Exception("error");
        }
    }
}
