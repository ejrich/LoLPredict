using System.Collections.Generic;

namespace RiotApi.Models
{
    public class Match
    {
        public long GameId { get; set; }
        public int SeasonId { get; set; }
        public int QueueId { get; set; }
        public string GameVersion { get; set; }
        public string PlatformId { get; set; }
        public string GameMode { get; set; }
        public int MapId { get; set; }
        public string GameType { get; set; }
        public long GameDuration { get; set; }
        public long GameCreation { get; set; }
        public IList<ParticipantIdentity> ParticipantIdentities { get; set; }
        public IList<Participant> Participants { get; set; }
        public IList<Team> Teams { get; set; }
    }
}
