using System.Collections.Generic;
using System.Threading.Tasks;
using LoLPredict.ModelPipeline.Domain;
using Microsoft.Extensions.Logging;

namespace LoLPredict.ModelPipeline
{
    public interface IModelCreationPipeline
    {
        Task CreateModels();
    }

    public class ModelCreationPipeline : IModelCreationPipeline
    {
        private readonly IEnumerable<IModelFactory> _modelFactories;
        private readonly IModelStorageService _modelStorageService;
        private readonly ILogger _log;

        public ModelCreationPipeline(IEnumerable<IModelFactory> modelFactories, IModelStorageService modelStorageService,
            ILogger<ModelCreationPipeline> log)
        {
            _modelFactories = modelFactories;
            _modelStorageService = modelStorageService;
            _log = log;
        }

        public async Task CreateModels()
        {
            foreach (var modelFactory in _modelFactories)
            {
                _log.LogInformation($"Creating model - {modelFactory.GetType().Name}");

                var model = await modelFactory.CreateModel();

                if (model != null)
                    await _modelStorageService.StoreModel(model);
            }
        }
    }
}
