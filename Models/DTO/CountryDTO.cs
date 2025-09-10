using NewsWebPortal.Models.Domains;
using System.ComponentModel.DataAnnotations;

namespace NewsWebPortal.Models.DTO
{
    public class CountryDTO
    {
        [Key]
        public Guid Id { get; set; }
        public string Continent { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Flag { get; set; } = null!;
        public string Description { get; set; } = null!;
        
    }
}
