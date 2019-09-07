using System.IO;
using System.Threading.Tasks;
using LoLPredict.GamePipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LoLPredict.Pipelines
{
    public class GamePipelineFunction : ControllerBase
    {
        private readonly IGamePipeline _gamePipeline;

        public GamePipelineFunction(IGamePipeline gamePipeline)
        {
            _gamePipeline = gamePipeline;
        }

        [FunctionName("GamePipeline")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _gamePipeline.LoadGames();
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
