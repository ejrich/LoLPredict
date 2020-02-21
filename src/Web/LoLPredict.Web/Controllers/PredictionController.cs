using System.Threading.Tasks;
using LoLPredict.Web.Domain;
using LoLPredict.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.ML;

namespace LoLPredict.Web.Controllers
{
    public class PredictionController : ControllerBase
    {
        private readonly MLContext _context;
        private readonly IModelRepository _modelRepository;

        public PredictionController(MLContext context, IModelRepository modelRepository)
        {
            _context = context;
            _modelRepository = modelRepository;
        }

        // POST: api/v1/prediction/
        [FunctionName(nameof(MakePrediction))]
        public async Task<IActionResult> MakePrediction([HttpTrigger(AuthorizationLevel.Anonymous,
            "Post", Route = "api/v1/prediction")] HttpRequest request)
        {
            var predictionRequest = await request.Convert<PredictionRequest>();

            var model = await _modelRepository.LoadModel(predictionRequest.Patch);

            if (model == null) return new NotFoundResult();

            // Return prediction engine
            var predictionEngine = _context.Model.CreatePredictionEngine<Draft, GamePrediction>(model);

            return new OkObjectResult(predictionEngine.Predict(predictionRequest.Draft));
        }
    }
}
