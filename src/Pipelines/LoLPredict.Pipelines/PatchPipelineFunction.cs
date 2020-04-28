using System.Threading.Tasks;
using LoLPredict.PatchPipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
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
        public async Task Run([TimerTrigger("0 0 6 * * *")]TimerInfo timer, ILogger log)
        {
            var patch = await _patchPipeline.UpdatePatch();

            log.LogInformation($"Patch Updated: {patch.Updated}, Current Live Patch: {patch.Version}");
        }
    }
}
