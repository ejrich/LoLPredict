using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoLPredict.Database.Models
{
    [Table("Summoner")]
    public class Summoner
    {
        [Key]
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string Puuid { get; set; }
    }
}
