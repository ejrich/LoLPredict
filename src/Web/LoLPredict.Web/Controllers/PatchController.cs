using System.Collections.Generic;
using System.Threading.Tasks;
using LoLPredict.Database.Models;
using LoLPredict.Web.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;

namespace LoLPredict.Web.Controllers
{
    public class PatchController : ControllerBase
    {
        private readonly IGameRepository _gameRepository;

        public PatchController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        // GET: api/v1/patch
        [FunctionName(nameof(RetrieveAllPatches))]
        public async Task<IEnumerable<Patch>> RetrieveAllPatches([HttpTrigger("Get",
            Route = "api/v1/patch")] HttpRequest request)
        {
            return await _gameRepository.LoadPatches();
        }
    }
}
