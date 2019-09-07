using System.ComponentModel.DataAnnotations.Schema;

namespace LoLPredict.Database.Models
{
    [Table("Champion")]
    public class Champion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Patch { get; set; }
    }
}
