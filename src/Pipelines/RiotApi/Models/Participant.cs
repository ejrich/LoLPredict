namespace RiotApi.Models
{
    public class Participant
    {
        public int ParticipantId { get; set; }
        public int TeamId { get; set; }
        public int ChampionId { get; set; }
        public int Spell1Id { get; set; }
        public int Spell2Id { get; set; }
        public string HighestAchievedSeasonTier { get; set; }
        public TimeLine TimeLine { get; set; }
    }
}
