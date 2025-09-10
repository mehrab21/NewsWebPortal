using NewsWebPortal.Models.Domains;

namespace NewsWebPortal.Models.DTO
{
    public class CreateCountryDTO
    {
        public string Continent { get; set; } = null!;
        public string Name { get; set; } = null!;
        public IFormFile Flag { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ICollection<NewsPaper> Newspapers { get; set; } = new List<NewsPaper>();
    }
}
