using System.Collections.Generic;

namespace RiotApi.Models
{
    public class MatchList
    {
        public int EndIndex { get; set; }
        public int StartIndex { get; set; }
        public int TotalGames { get; set; }
        public IList<SummonerMatch> Matches { get; set; }
    }
}
