using System.Threading.Tasks;

namespace AsyncFactoryMethod
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
        }
    }

    internal class Foo
    {
        private Foo()   // 把建構函式宣告成 private，不讓外界使用。
        {
        }

        // 外界必須透過此方法才能建立物件。
        public static Task<Foo> CreateInstanceAsync()
        {
            var foo = new Foo();          // 建立物件。
            return foo.InitializeAsync(); // 初始化。
        }

        private async Task<Foo> InitializeAsync()
        {
            //await DomSomeTaskAsync();
            return this;
        }
    }
}