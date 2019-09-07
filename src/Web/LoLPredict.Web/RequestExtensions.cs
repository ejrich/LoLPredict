using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LoLPredict.Web
{
    public static class RequestExtensions
    {
        public static async Task<T> Convert<T>(this HttpRequest request)
        {
            using (var content = new StreamReader(request.Body))
            {
                var body = await content.ReadToEndAsync();

                return JsonConvert.DeserializeObject<T>(body);
            }
        }
    }
}
