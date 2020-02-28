using System.IO;
using System.Threading.Tasks;
using LoLPredict.Web.DAL;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using Microsoft.Extensions.Options;
using Microsoft.ML;

namespace LoLPredict.Web.Domain
{
    public interface IModelRepository
    {
        Task<ITransformer> LoadModel(string patch);
    }

    public class ModelRepository : IModelRepository
    {
        private readonly MLContext _context;
        private readonly IGameRepository _gameRepository;
        private readonly Settings _settings;

        public ModelRepository(MLContext context, IGameRepository gameRepository, IOptionsMonitor<Settings> options)
        {
            _context = context;
            _gameRepository = gameRepository;
            _settings = options.CurrentValue;
        }

        public async Task<ITransformer> LoadModel(string patch)
        {
            var modelName = await _gameRepository.LoadLatestModel(patch);

            if (modelName == null) return null;

            var storageAccount = CloudStorageAccount.Parse(_settings.AzureStorageConnectionString); 
            var fileClient = storageAccount.CreateCloudFileClient();
            var share = fileClient.GetShareReference("patches");
            var dir = share.GetRootDirectoryReference();
            var patchDir = dir.GetDirectoryReference(patch);
            var file = patchDir.GetFileReference(modelName.Name);

            if (!file.Exists()) return null;

            // Load model from file
            using (var stream = new MemoryStream())
            {
                await file.DownloadToStreamAsync(stream);
                var model = _context.Model.Load(stream, out var schema);
                
                return model;
            }
        }
    }
}
