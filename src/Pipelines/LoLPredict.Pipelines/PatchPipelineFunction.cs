using System.Threading.Tasks;
using LoLPredict.PatchPipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LoLPredict.Pipelines
{
    public class PatchPipelineFunction : ControllerBase
    {
        private readonly IPatchPipeline _patchPipeline;

        public PatchPipelineFunction(IPatchPipeline patchPipeline)
        {
            _patchPipeline = patchPipeline;
        }

        [FunctionName("PatchPipeline")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var patch = await _patchPipeline.UpdatePatch();

            log.LogInformation($"Patch Updated: {patch.Updated}, Current Live Patch: {patch.Version}");

            return patch.Updated
                ? new OkObjectResult(patch)
                : new ObjectResult(patch) {StatusCode = 204};
        }
    }
}
