using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoLPredict.Database.Models
{
    [Table("Patch")]
    public class Patch
    {
        [Key, Column(Order = 0)]
        public int Major { get; set; }
        [Key, Column(Order = 1)]
        public int Minor { get; set; }
        [Key, Column(Order = 2)]
        public int Version { get; set; }
        public bool Live { get; set; }
        public bool Tournament { get; set; }
    }
}
