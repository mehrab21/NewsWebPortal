using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsWebPortal.Data;
using NewsWebPortal.Models.Domain;
using NewsWebPortal.Models.Domains;
using NewsWebPortal.Models.DTO;

namespace NewsWebPortal.Services
{
    public interface INewspaperService
    {
        Task<List<NewsPaperDTO>> GetAllNewspapersAsync();
        Task<NewsPaperDTO> CreateNewspaperRequestsAsync([FromForm] CreateRequestDTO createRequestDTO);
        Task<CountryDTO> CreateCountryAsync([FromForm] CreateCountryDTO createCountryDTO);
        Task<List<CountryDTO>> GetAllCountry();
        Task<List<NewsPaperDTO>> GetNewspaperByStatus();
        Task<NewsPaperDTO> AcceptForPublic(Guid id);
        Task<NewsPaperDTO> DeleteForPublicAsync(Guid id);
        Task<NewsPaperDTO> GetNewspaperByIdAsync(Guid id);
    }
    public class NewspaperService : INewspaperService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NewspaperService(ApplicationDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<NewsPaperDTO> AcceptForPublic(Guid id)
        {
            var result = await _context.Newspapers.FirstOrDefaultAsync(n => n.Id == id);
            if (result == null)
                throw new ArgumentException("Newspaper not found");
            result.Status = "Accepted";
            await _context.SaveChangesAsync();
            return new NewsPaperDTO
            {
                Id = result.Id,
                NewspaperName = result.NewspaperName,
                NewspaperLink = result.NewspaperLink,
                CountryId = result.CountryId,
                NewspaperInfo = result.NewspaperInfo,
                Image = result.Image,
                Status = result.Status
            };
        }
        

        public Task<CountryDTO> CreateCountryAsync([FromForm] CreateCountryDTO createCountryDTO)
        {
            if (string.IsNullOrEmpty(createCountryDTO.Name))
                throw new ArgumentException("Country name is required");
            if (createCountryDTO.Flag == null || createCountryDTO.Flag.Length == 0)
                throw new ArgumentException("No flag image uploaded");
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(createCountryDTO.Flag.FileName);
            var uploadsFolder = Path.Combine(_env.WebRootPath, "flags");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                createCountryDTO.Flag.CopyTo(stream);
            }
            var country = new Country
            {
                Id = Guid.NewGuid(),
                Continent = createCountryDTO.Continent,
                Name = createCountryDTO.Name,
                Description = createCountryDTO.Description,
                Flag = uniqueFileName
            };
            _context.Countries.Add(country);
            _context.SaveChanges();
            var countryDTO = new CountryDTO
            {
                Id = country.Id,
                Continent = country.Continent,
                Name = country.Name,
                Description = country.Description,
                Flag = country.Flag
            };
            return Task.FromResult(countryDTO);
        }

        public async Task<NewsPaperDTO> CreateNewspaperRequestsAsync([FromForm] CreateRequestDTO newspaperRequest)
        {
            if (newspaperRequest.Image == null || newspaperRequest.Image.Length == 0)
                throw new ArgumentException("No image uploaded");

            if (string.IsNullOrEmpty(newspaperRequest.NewspaperName))
                throw new ArgumentException("Newspaper name is required");

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(newspaperRequest.Image.FileName);
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await newspaperRequest.Image.CopyToAsync(stream);
            }

            var newspaper = new NewsPaper
            {
                Id = Guid.NewGuid(),
                NewspaperName = newspaperRequest.NewspaperName,
                NewspaperLink = newspaperRequest.NewspaperLink,
                CountryId = newspaperRequest.CountryId,
                NewspaperInfo = newspaperRequest.NewspaperInfo,
                Image = uniqueFileName,
                Status = "Accepted"
            };

            _context.Newspapers.Add(newspaper);
            await _context.SaveChangesAsync();

            return new NewsPaperDTO
            {
                Id = newspaper.Id,
                NewspaperName = newspaper.NewspaperName,
                NewspaperLink = newspaper.NewspaperLink,
                CountryId = newspaper.CountryId,
                NewspaperInfo = newspaper.NewspaperInfo,
                Image = newspaper.Image,
                Status = newspaper.Status
            };
        }

        public Task<NewsPaperDTO> DeleteForPublicAsync(Guid id)
        {
            var result = _context.Newspapers.FirstOrDefault(n => n.Id == id);
            if (result == null)
                throw new ArgumentException("Newspaper not found");
            _context.Newspapers.Remove(result);
            _context.SaveChanges();
            return Task.FromResult(new NewsPaperDTO
            {
                Id = result.Id,
                NewspaperName = result.NewspaperName,
                NewspaperLink = result.NewspaperLink,
                CountryId = result.CountryId,
                NewspaperInfo = result.NewspaperInfo,
                Image = result.Image,
                Status = result.Status
            });
        }

        public async Task<List<CountryDTO>> GetAllCountry()
        {
            var baseURL = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            return await _context.Countries
                .Select(n => new CountryDTO
                {
                    Id = n.Id,
                    Continent = n.Continent,
                    Name = n.Name,
                    Description = n.Description,
                    Flag = $"{baseURL}/flags/{n.Flag}"
                })
                .ToListAsync();
        }

        public async Task<List<NewsPaperDTO>> GetAllNewspapersAsync()
        {
            var baseURL = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            return await _context.Newspapers
                .Where(n => n.Status =="Accepted")
                .Select(n => new NewsPaperDTO
                {
                    Id = n.Id,
                    NewspaperName = n.NewspaperName,
                    NewspaperLink = n.NewspaperLink,
                    CountryId = n.CountryId,
                    NewspaperInfo = n.NewspaperInfo,
                    Image = $"{baseURL}/uploads/{n.Image}",
                })
                .ToListAsync();
        }

        public Task<NewsPaperDTO> GetNewspaperByIdAsync(Guid id)
        {
            var newspaper = _context.Newspapers.FirstOrDefault(n => n.Id == id);
            if (newspaper == null)
                throw new ArgumentException("Newspaper not found");
            var baseURL = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var newspaperDTO = new NewsPaperDTO
            {
                Id = newspaper.Id,
                NewspaperName = newspaper.NewspaperName,
                NewspaperLink = newspaper.NewspaperLink,
                CountryId = newspaper.CountryId,
                NewspaperInfo = newspaper.NewspaperInfo,
                Image = $"{baseURL}/uploads/{newspaper.Image}",
                Status = newspaper.Status
                };
            return Task.FromResult(newspaperDTO);
        }

        public async Task<List<NewsPaperDTO>> GetNewspaperByStatus()
        {
            var baseURL = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            return await _context.Newspapers
               .Where(n => n.Status == "Pending")
               .Select(n => new NewsPaperDTO
               {
                   Id = n.Id,
                   NewspaperName = n.NewspaperName,
                   NewspaperLink = n.NewspaperLink,
                   Status = n.Status,
                   CountryId= n.CountryId,
                   Image = baseURL + "/uploads/" + n.Image,
                   NewspaperInfo = n.NewspaperInfo


               }).ToListAsync();
        }
    
    }
}
