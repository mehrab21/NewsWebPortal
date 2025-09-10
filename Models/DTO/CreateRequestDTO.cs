using Microsoft.Build.Framework;


namespace NewsWebPortal.Models.DTO
{
    public class CreateRequestDTO
    {
        [Required]
        public string NewspaperName { get; set; } = string.Empty;
        [Required]
        public string NewspaperLink { get; set; } = string.Empty;
        [Required]
        public Guid CountryId { get; set; }

        public string NewspaperInfo { get; set; } = string.Empty;
        [Required]
        public IFormFile Image { get; set; }
    }
}
