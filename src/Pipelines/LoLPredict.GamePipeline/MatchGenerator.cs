using System.Threading.Tasks;
using LoLPredict.Pipelines.DAL;
using Microsoft.Extensions.Options;
using RiotApi;
using RiotApi.Models;

namespace LoLPredict.GamePipeline
{
    public interface IMatchGenerator
    {
        Task Generate(string league);
        Task Generate(string league, string tier);
    }

    public class MatchGenerator : IMatchGenerator
    {
        private readonly IRestClientWrapper _client;
        private readonly IGameRepository _gameRepository;
        private readonly IRiotModelMapper _riotModelMapper;

        public MatchGenerator(IRestClientFactory restClientFactory, IGameRepository gameRepository,
            IRiotModelMapper riotModelMapper, IOptionsMonitor<Settings> options)
        {
            _client = restClientFactory.CreateRestClient(options.CurrentValue.RiotApiUrl, options.CurrentValue.RiotApiToken);
            _gameRepository = gameRepository;
            _riotModelMapper = riotModelMapper;
        }

        public async Task Generate(string league)
        {
            // 1. Load a specific league
            var leagueList = await _client.GetAsync<LeagueList>($"lol/league/v4/{ league }/by-queue/RANKED_SOLO_5x5");

            // 2. Load unique games
            foreach (var summoner in leagueList.Entries)
            {
                var account = await GetAccount(summoner);

                var matches = await _client.GetAsync<MatchList>($"lol/match/v4/matchlists/by-account/{ account.AccountId }?queue=420");

                foreach (var match in matches.Matches)
                {
                    if (await _gameRepository.GameResultExists(match.GameId)) continue;

                    // 3. Load the game, store picks and winner for model creation
                    var matchDetails = await _client.GetAsync<Match>($"lol/match/v4/matches/{ match.GameId }");

                    var game = _riotModelMapper.TranslateMatchToGame(matchDetails);
                    if (game != null)
                        await _gameRepository.InsertGameResult(game);
                }
            }
        }

        public async Task Generate(string league, string tier)
        {
            // TODO load league and tier
        }

        private async Task<Database.Models.Summoner> GetAccount(LeagueSummoner summoner)
        {
            var account = await _gameRepository.GetSummonerById(summoner.SummonerId);
            if (account != null) return account;

            var summonerAccount = await _client.GetAsync<RiotApi.Models.Summoner>($"lol/summoner/v4/summoners/{ summoner.SummonerId }");

            account = _riotModelMapper.MapSummonerToAccount(summonerAccount);
            await _gameRepository.InsertSummoner(account);

            return account;
        }
    }
}
