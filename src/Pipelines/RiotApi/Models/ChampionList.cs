using System.Collections.Generic;

namespace RiotApi.Models
{
    public class ChampionList
    {
        public string Type { get; set; }
        public string Version { get; set; }
        public Dictionary<string, ChampionData> Data { get; set; }
    }
}
