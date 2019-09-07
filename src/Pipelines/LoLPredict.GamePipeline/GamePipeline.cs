using System.Threading.Tasks;
using LoLPredict.Pipelines.DAL;
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
        private readonly IGameRepository _gameRepository;

        public GamePipeline(IRestClientFactory restClientFactory, IGameRepository gameRepository)
        {
            _client = restClientFactory.CreateRestClient("API");
            _gameRepository = gameRepository;
        }

        public async Task LoadGames()
        {
            // 1. Load a specific league
            var league = await _client.GetAsync<LeagueList>("lol/league/v4/challengerleagues/by-queue/RANKED_SOLO_5x5");

            // 2. Load unique games
            foreach (var summoner in league.Entries)
            {
                var account = await GetAccount(summoner);

                var matches = await _client.GetAsync<MatchList>($"lol/match/v4/matchlists/by-account/{ account.AccountId }?queue=420");

                foreach (var match in matches.Matches)
                {
                    if (_gameRepository.GameResultExists(match.GameId)) continue;

                    var matchDetails = await _client.GetAsync<Match>($"lol/match/v4/matches/{ match.GameId }");

                    // TODO Store the game data
                }
            }

            // 3. Load the game, store picks and winner for model creation
        }

        private async Task<Database.Models.Summoner> GetAccount(LeagueSummoner summoner)
        {
            var account = _gameRepository.GetSummonerById(summoner.SummonerId);
            if (account != null) return account;

            var summonerAccount = await _client.GetAsync<RiotApi.Models.Summoner>($"lol/summoner/v4/summoners/{ summoner.SummonerId }");

            account = MapSummonerToAccount(summonerAccount);
            _gameRepository.InsertSummoner(account);

            return account;
        }

        private Database.Models.Summoner MapSummonerToAccount(RiotApi.Models.Summoner summonerAccount)
        {
            return new Database.Models.Summoner
            {
                Id = summonerAccount.Id,
                AccountId = summonerAccount.AccountId,
                Name = summonerAccount.Name,
                Puuid = summonerAccount.Puuid
            };
        }
    }
}
