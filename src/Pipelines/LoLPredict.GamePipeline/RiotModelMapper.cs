using System;
using System.Linq;
using LoLPredict.Database.Models;
using RiotApi.Models;

namespace LoLPredict.GamePipeline
{
    public interface IRiotModelMapper
    {
        Database.Models.Summoner MapSummonerToAccount(RiotApi.Models.Summoner summonerAccount);
        GameResult TranslateMatchToGame(Match match);
    }

    public class RiotModelMapper : IRiotModelMapper
    {
        private const bool BLUE = false;
        private const bool RED = true;

        public Database.Models.Summoner MapSummonerToAccount(RiotApi.Models.Summoner summonerAccount)
        {
            return new Database.Models.Summoner
            {
                Id = summonerAccount.Id,
                AccountId = summonerAccount.AccountId,
                Name = summonerAccount.Name,
                Puuid = summonerAccount.Puuid
            };
        }

        public GameResult TranslateMatchToGame(Match match)
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
