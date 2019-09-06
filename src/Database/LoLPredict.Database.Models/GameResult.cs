using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoLPredict.Database.Models
{
    [Table("GameResult")]
    public class GameResult
    {
        [Key]
        public long Id { get; set; }
        public string Patch { get; set; }
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
        public bool Winner { get; set; }
    }
}
