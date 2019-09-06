using System.Threading.Tasks;
using RiotApi;
using RiotApi.Models;

namespace LoLPredict.GamePipeline
{
    public interface IGamePipeline
    {
        Task LoadGames();
    }

    public class GamePipeline : IGamePipeline
    {
        private readonly IRestClientWrapper _client;

        public GamePipeline(IRestClientFactory restClientFactory)
        {
            _client = restClientFactory.CreateRestClient("API");
        }

        public async Task LoadGames()
        {
            // 1. Load a specific league
            var league = await _client.GetAsync<LeagueList>("lol/league/v4/challengerleagues/by-queue/RANKED_SOLO_5x5");

            // 2. Load unique games
            foreach (var summoner in league.Entries)
            {
                // TODO Lookup the summoner account in the database before falling back to the API
                var account = await _client.GetAsync<Summoner>($"lol/summoner/v4/summoners/{ summoner.SummonerId }");
                // TODO Store the summoner account in the database

                var matches = await _client.GetAsync<MatchList>($"lol/match/v4/matchlists/by-account/{ account.AccountId }?queue=420");
                // TODO lookup the match Id to make sure we're not making extraneous API calls
            }

            // 3. Load the game, store picks and winner for model creation
        }
    }
}
