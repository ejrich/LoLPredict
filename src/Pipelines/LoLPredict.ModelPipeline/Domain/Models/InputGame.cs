using Microsoft.ML.Data;

namespace LoLPredict.ModelPipeline.Domain.Models
{
    public class InputGame
    {
        public int BlueTop { get; set; }
        public int BlueJungle { get; set; }
        public int BlueMid { get; set; }
        public int BlueBottom { get; set; }
        public int BlueSupport { get; set; }
        public int RedTop { get; set; }
        public int RedJungle { get; set; }
        public int RedMid { get; set; }
        public int RedBottom { get; set; }
        public int RedSupport { get; set; }
        [ColumnName("Label")]
        public bool WinningTeam { get; set; }
    }
}
