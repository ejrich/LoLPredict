using System.Collections.Generic;
using System.Threading.Tasks;
using LoLPredict.Database.Models;
using LoLPredict.Web.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;

namespace LoLPredict.Web.Controllers
{
    public class ChampionController : ControllerBase
    {
        private readonly IGameRepository _gameRepository;

        public ChampionController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        // GET: api/v1/champion
        [FunctionName(nameof(RetrieveAllChampions))]
        public async Task<IEnumerable<Champion>> RetrieveAllChampions([HttpTrigger("Get",
            Route = "api/v1/champion/{patch}")] HttpRequest request, string patch)
        {
            return await _gameRepository.LoadChampionsByPatch(patch);
        }
    }
}
