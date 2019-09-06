using LoLPredict.ModelPipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace LoLPredict.Pipelines
{
    public class ModelPipelineFunction : ControllerBase
    {
        private readonly IModelCreationPipeline _modelCreationPipeline;

        public ModelPipelineFunction(IModelCreationPipeline modelCreationPipeline)
        {
            _modelCreationPipeline = modelCreationPipeline;
        }

        [FunctionName("ModelPipeline")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            _modelCreationPipeline.CreateModels();

            return new OkObjectResult("Models successfully created");
        }
    }
}
