using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoLPredict.Database.Models
{
    [Table("Model")]
    public class Model
    {
        [Key]
        public int Id { get; set; }
        public string Patch { get; set; }
        public string Name { get; set; }
    }
}
