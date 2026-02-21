using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Ex06_AspNetAppDeadlock.Controllers
{
    public class DemoDeadlockController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage DownloadPage()
        {
            var task = MyDownloadPageAsync("https://www.huanlintalk.com");
            var content = task.Result;
            return Request.CreateResponse($"網頁內容總共為 {content.Length} 個字元。");
        }

        static HttpClient _httpClient = new HttpClient();

        private async Task<string> MyDownloadPageAsync(string url)
        {
            string content = await _httpClient.GetStringAsync(url); // deadlock!
            return content;

            // return System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
        }

        // 解法一：使用 Task.ConfigureAwait(false)。此解法有副作用，不是頂好的解法。
        // 解法二：從頭到尾都使用 async 方法。這是正解。
    }
}
