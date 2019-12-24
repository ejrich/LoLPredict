using System.Threading.Tasks;
using LoLPredict.GamePipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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

            return new OkObjectResult("Games successfully loaded");
        }
    }
}
