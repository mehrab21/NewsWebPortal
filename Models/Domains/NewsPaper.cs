using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsWebPortal.Models.Domains
{
    public class NewsPaper
    {
        [Key]
        public Guid Id { get; set; }
        public string NewspaperName { get; set; } = string.Empty;
        public string NewspaperLink { get; set; } = string.Empty;
        
        public string NewspaperInfo { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        [ForeignKey("CountryId")]
        public Country Country { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
