using Microsoft.EntityFrameworkCore;
using NewsWebPortal.Models.Domain;
using NewsWebPortal.Models.Domains;
using NewsWebPortal.Models.DTO;

namespace NewsWebPortal.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<NewsPaper> Newspapers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<NewsWebPortal.Models.Domain.NewsPaperDTO> NewsPaperDTO { get; set; } = default!;
        public DbSet<NewsWebPortal.Models.DTO.CountryDTO> CountryDTO { get; set; } = default!;
        }
}
