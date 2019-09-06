namespace RiotApi.Models
{
    public class LeagueSummoner
    {
        public string SummonerId { get; set; }
        public string SummonerName { get; set; }
        public int LeaguePoints { get; set; }
        public string Rank { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public bool Veteran { get; set; }
        public bool Inactive { get; set; }
        public bool FreshBlood { get; set; }
        public bool HotStreak { get; set; }
    }
}
