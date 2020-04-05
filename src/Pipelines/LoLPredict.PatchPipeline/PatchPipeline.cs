using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoLPredict.Database.Models;
using LoLPredict.Pipelines.DAL;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RiotApi;
using RiotApi.Models;

namespace LoLPredict.PatchPipeline
{
    public interface IPatchPipeline
    {
        Task<PatchData> UpdatePatch();
    }

    public class PatchPipeline : IPatchPipeline
    {
        private readonly IGameRepository _gameRepository;
        private readonly IRestClientWrapper _client;
        private readonly ILogger _log;

        public PatchPipeline(IGameRepository gameRepository, IRestClientFactory restClientFactory,
            IOptionsMonitor<Settings> options, ILogger<PatchPipeline> log)
        {
            _gameRepository = gameRepository;
            _log = log;
            _client = restClientFactory.CreateRestClient(options.CurrentValue.StaticDataEndpoint, string.Empty);
        }

        public async Task<PatchData> UpdatePatch()
        {
            // 1. Load the current live patch
            var livePatch = await _gameRepository.LoadLivePatch();

            // 2. Make a call to get the latest patch information
            var currentPatch = await LoadCurrentPatch();

            // 3. If the the latest patch if different than live,
            if (livePatch == null || !string.Equals(livePatch.Number(), currentPatch.V))
            {
                _log.LogInformation("Applying new patch");
                // Insert a new patch record
                await _gameRepository.InsertPatch(currentPatch.V, true);

                // Set the previous patch to not live
                if (livePatch != null)
                {
                    livePatch.Live = false;
                    await _gameRepository.UpdatePatch(livePatch);
                }

                // load all champions in the database
                var allChampions = await _gameRepository.LoadAllChampions();

                // load champion data from the current patch
                var champions = await LoadChampions(currentPatch.V);

                var newChampions = champions.Where(c => allChampions.All(_ => _.Id != c.Id));

                // Insert champion records
                await _gameRepository.InsertChampions(newChampions);

                return new PatchData
                {
                    Updated = true,
                    Version = currentPatch.V
                };
            }

            _log.LogInformation("Patch is up-to-date");

            return new PatchData
            {
                Updated = false,
                Version = livePatch.Number()
            };
        }

        private async Task<Realm> LoadCurrentPatch()
        {
            var response = await _client.GetAsync<Realm>("realms/na.json");

            return response;
        }

        private async Task<IEnumerable<Champion>> LoadChampions(string patchNumber)
        {
            var response = await _client.GetAsync<ChampionList>($"cdn/{ patchNumber }/data/en_US/champion.json");

            var champions = response.Data.Values.Select(MapChampionDataToChampion);

            return champions;
        }

        private static Champion MapChampionDataToChampion(ChampionData champ)
        {
            var patchComponents = champ.Version.CreatePatchComponents();

            return new Champion
            {
                Id = Convert.ToInt32(champ.Key),
                Name = champ.Name,
                Image = champ.Image.Full.Substring(0, champ.Image.Full.Length - 4),
                Major = patchComponents[0],
                Minor = patchComponents[1],
                Version = patchComponents[2]
            };
        }
    }
}
