using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;

namespace LoLPredict.Web.Controllers
{
    public class StaticFileController : ControllerBase
    {
        // GET: /
        [FunctionName(nameof(Index))]
        public async Task<IActionResult> Index([HttpTrigger("Get", Route = "/")] HttpRequest request)
        {
            var index = GetFilePath("index.html");

            return await ReturnFileIfExists(index);
        }

        // GET: {fileName}
        [FunctionName(nameof(RetrieveFile))]
        public async Task<IActionResult> RetrieveFile([HttpTrigger("Get", Route = "{fileName}")] HttpRequest request, string fileName)
        {
            var path = GetFilePath(fileName);

            return await ReturnFileIfExists(path);
        }

        // GET: /static/{js|css}{fileName}
        [FunctionName(nameof(RetrieveStaticFile))]
        public async Task<IActionResult> RetrieveStaticFile([HttpTrigger("Get", Route = "static/{resource:regex(js|css)}/{fileName}")] HttpRequest request, string resource, string fileName)
        {
            var resourcePath = Path.Combine("static", resource, fileName);
            var path = GetFilePath(resourcePath);

            return await ReturnFileIfExists(path);
        }

        private static string GetFilePath(string fileName)
        {
            var root = Directory.GetCurrentDirectory();

            var path = Path.Combine(root, "ClientApp", "build", fileName);

            return path;
        }

        private async Task<IActionResult> ReturnFileIfExists(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var stream = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(stream, MimeTypes.GetMimeType(filePath));
        }
    }
}
