using System.Collections.Generic;

namespace RiotApi.Models
{
    public class LeagueList
    {
        public string Tier { get; set; }
        public string LeagueId { get; set; }
        public string Queue { get; set; }
        public string Name { get; set; }
        public IList<LeagueSummoner> Entries { get; set; }
    }
}
