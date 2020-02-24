using System;
using System.IO;
using System.Threading.Tasks;
using LoLPredict.Database.Models;
using LoLPredict.Pipelines.DAL;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ML;

namespace LoLPredict.ModelPipeline.Domain
{
    public interface IModelStorageService
    {
        Task StoreModel(GameModel model);
    }

    public class ModelStorageService : IModelStorageService
    {
        private readonly MLContext _context;
        private readonly GameContext _dataContext;
        private readonly ILogger _log;
        private readonly CloudFileClient _fileClient;

        public ModelStorageService(MLContext context, GameContext dataContext,
            IOptionsMonitor<Settings> options, ILogger log)
        {
            _context = context;
            _dataContext = dataContext;
            _log = log;

            var storageAccount = CloudStorageAccount.Parse(options.CurrentValue.AzureStorageConnectionString);
            _fileClient = storageAccount.CreateCloudFileClient();
        }

        public async Task StoreModel(GameModel model)
        {
            var share = _fileClient.GetShareReference("patches");
             share.CreateIfNotExists();

            var rootDirectory = share.GetRootDirectoryReference();

            var patchDirectory = rootDirectory.GetDirectoryReference(model.Patch);
            patchDirectory.CreateIfNotExists();

            var formattedDate = DateTime.Now.ToString("yyMMdd_HHmm");
            var file = patchDirectory.GetFileReference($"{model.Patch}_{formattedDate}.zip");

            _log.LogInformation($"Uploading model - {file.Name}");

            using (var stream = new MemoryStream())
            {
                _context.Model.Save(model.Model, null, stream);
                var byteArray = stream.GetBuffer();
                file.UploadFromByteArray(byteArray, 0, Convert.ToInt32(stream.Length));
            }

            await _dataContext.Models.AddAsync(new Model
            {
                Patch = model.Patch,
                Name = file.Name
            });
            await _dataContext.SaveChangesAsync();
        }
    }
}
