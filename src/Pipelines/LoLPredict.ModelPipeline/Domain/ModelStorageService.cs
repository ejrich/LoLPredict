using System;
using System.IO;
using LoLPredict.Database.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using Microsoft.Extensions.Logging;
using Microsoft.ML;

namespace LoLPredict.ModelPipeline.Domain
{
    public interface IModelStorageService
    {
        void StoreModel(GameModel model);
    }

    public class ModelStorageService : IModelStorageService
    {
        private readonly MLContext _context;
        private readonly GameContext _dataContext;
        private readonly ILogger _log;
        private readonly CloudFileClient _fileClient;

        public ModelStorageService(MLContext context, GameContext dataContext, ILogger log)
        {
            _context = context;
            _dataContext = dataContext;
            _log = log;

            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AZURE_CONNECTION_STRING", EnvironmentVariableTarget.Process));
            _fileClient = storageAccount.CreateCloudFileClient();
        }

        public void StoreModel(GameModel model)
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

            _dataContext.Models.Add(new Model
            {
                Patch = model.Patch,
                Name = file.Name
            });
            _dataContext.SaveChanges();
        }
    }
}
