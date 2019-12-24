using System.Collections.Generic;

namespace RiotApi.Models
{
    public class Team
    {
        public bool FirstDragon { get; set; }
        public bool FirstInhibitor { get; set; }
        public string Win { get; set; }
        public bool FirstRiftHarold { get; set; }
        public bool FirstBaron { get; set; }
        public int BaronKills { get; set; }
        public int RiftHaroldKills { get; set; }
        public bool FirstBlood { get; set; }
        public int TeamId { get; set; }
        public bool FirstTower { get; set; }
        public int InhibitorKills { get; set; }
        public int TowerKills { get; set; }
        public int DragonKills { get; set; }
        public IList<Ban> Bans { get; set; }
    }
}
