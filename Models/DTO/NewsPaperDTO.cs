using NewsWebPortal.Models.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsWebPortal.Models.Domain
{
    public class NewsPaperDTO
    {
        public Guid Id { get; set; }
        public string NewspaperName { get; set; } = string.Empty;
        public string NewspaperLink { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        [ForeignKey("CountryId")]
        public string NewspaperInfo { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Status { get; set; }  = string.Empty;
    }
}
    