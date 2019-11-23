using System;
using System.Linq;
using System.Threading.Tasks;
using LoLPredict.Database.Models;
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

        private const bool BLUE = false;
        private const bool RED = true;

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

                    // 3. Load the game, store picks and winner for model creation
                    var matchDetails = await _client.GetAsync<Match>($"lol/match/v4/matches/{ match.GameId }");

                    var game = TranslateMatchToGame(matchDetails);
                    if (game != null)
                        _gameRepository.InsertGameResult(game);
                }
            }
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

        private static Database.Models.Summoner MapSummonerToAccount(RiotApi.Models.Summoner summonerAccount)
        {
            return new Database.Models.Summoner
            {
                Id = summonerAccount.Id,
                AccountId = summonerAccount.AccountId,
                Name = summonerAccount.Name,
                Puuid = summonerAccount.Puuid
            };
        }

        private static GameResult TranslateMatchToGame(Match match)
        {
            var result = new GameResult
            {
                Id = match.GameId,
                Patch = string.Join(".", match.GameVersion.Split('.').Take(2)),
                BlueTop = GetChampionId(match, BLUE, "TOP", "SOLO"),
                BlueJungle = GetChampionId(match, BLUE, "JUNGLE", "NONE"),
                BlueMid = GetChampionId(match, BLUE, "MIDDLE", "SOLO"),
                BlueBottom = GetChampionId(match, BLUE, "BOTTOM", "DUO_CARRY"),
                BlueSupport = GetChampionId(match, BLUE, "BOTTOM", "DUO_SUPPORT"),
                RedTop = GetChampionId(match, RED, "TOP", "SOLO"),
                RedJungle = GetChampionId(match, RED, "JUNGLE", "NONE"),
                RedMid = GetChampionId(match, RED, "MIDDLE", "SOLO"),
                RedBottom = GetChampionId(match, RED, "BOTTOM", "DUO_CARRY"),
                RedSupport = GetChampionId(match, RED, "BOTTOM", "DUO_SUPPORT"),
            };
            var winner = match.Teams.FirstOrDefault(_ => _.Win == "Win");
            if (winner == null || result.AnyUnassignedRoles()) return null;

            result.Winner = winner.TeamId == 200;

            return result;
        }

        private static int GetChampionId(Match match, bool team, string lane, string role)
        {
            Func<int, bool> teamComparer;
            if (team)
                teamComparer = x => x > 5;
            else
                teamComparer = x => x <= 5;

            var player = match.Participants.FirstOrDefault(_ =>
                _.TimeLine.Lane == lane && _.TimeLine.Role == role && teamComparer(_.ParticipantId)); 

            return player?.ChampionId ?? 0;
        }
    }
}
