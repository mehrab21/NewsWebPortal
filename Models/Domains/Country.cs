using System.ComponentModel.DataAnnotations;

namespace NewsWebPortal.Models.Domains
{
    public class Country
    {
        [Key]
        public Guid Id { get; set; }
        public string Continent { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Flag { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ICollection<NewsPaper> Newspapers { get; set; } = new List<NewsPaper>();
    }
}

