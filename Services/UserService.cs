using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebPortal.Data;
using NewsWebPortal.Models.Domain;
using NewsWebPortal.Models.Domains;
using NewsWebPortal.Models.DTO;
using System.Net.Http;

namespace NewsWebPortal.Services
{
    public interface IUserService
    {
        Task<List<NewsPaperDTO>> GetAllNewspapersAsync();
        Task<List<CountryDTO>> GetAllCountry();
        Task<NewsPaperDTO> RequestFromUserNewspaperAsync([FromForm] CreateRequestDTO createRequestDTO);
        Task<NewsPaperDTO> GetNewspaperByIdAsync(string id);
        Task<List<NewsPaperDTO>> GetAllNewspaperByCountryName(string s);
        Task<List<CountryDTO>> GetAllCountryByContinentalName(string continent);
        Task<List<NewsPaperDTO>> SearchNewspapersAsync(string q);
        Task<List<CountryDTO>> SearchCountryAsync(string q);

    }
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<NewsPaperDTO>> SearchNewspapersAsync(string q)
        {
            return await _context.Newspapers
                
                .Where(n => n.NewspaperName.ToLower().Contains(q.ToLower())
                         )
                .Select(n => new NewsPaperDTO
                {
                    Id = n.Id,
                    NewspaperName = n.NewspaperName,
                    CountryId = n.CountryId,
                    NewspaperLink = n.NewspaperLink,
                    Image = n.Image
                }).ToListAsync();
        }
        public async Task<List<CountryDTO>> SearchCountryAsync(string q)
        {
            return await _context.Countries

                .Where(n => n.Continent.ToLower().Contains(q.ToLower()) ||
                n.Name.ToLower().Contains(q.ToLower()) ||
                n.Description.ToLower().Contains(q.ToLower()))
                .Select(n => new CountryDTO
                {
                    Id = n.Id,
                    Continent = n.Continent,
                    Name = n.Name,
                    Flag = n.Flag,
                    Description = n.Description
                }).ToListAsync();
        }

        public async Task<List<NewsPaperDTO>> GetAllNewspapersAsync()
        {
            var baseURL = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            return await _context.Newspapers
                .Where(n => n.Status == "Accepted")
                .Distinct()
                .Select(n => new NewsPaperDTO
                {
                    Id = n.Id,
                    NewspaperName = n.NewspaperName,
                    NewspaperLink = n.NewspaperLink,
                    CountryId = n.CountryId,
                    NewspaperInfo = n.NewspaperInfo,
                    Image = $"{baseURL}/uploads/{n.Image}"
                })
                .ToListAsync();
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
        public async Task<NewsPaperDTO> RequestFromUserNewspaperAsync([FromForm] CreateRequestDTO createRequestDTO)
        {
            if (createRequestDTO.Image == null || createRequestDTO.Image.Length == 0)
                throw new ArgumentException("No image uploaded");

            if (string.IsNullOrEmpty(createRequestDTO.NewspaperName))
                throw new ArgumentException("Newspaper name is required");

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(createRequestDTO.Image.FileName);
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await createRequestDTO.Image.CopyToAsync(stream);
            }

            var newspaper = new NewsPaper
            {
                Id = Guid.NewGuid(),
                NewspaperName = createRequestDTO.NewspaperName,
                NewspaperLink = createRequestDTO.NewspaperLink,
                CountryId = createRequestDTO.CountryId,
                NewspaperInfo = createRequestDTO.NewspaperInfo,
                Image = uniqueFileName,
                Status = "Pending"
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
        public async Task<NewsPaperDTO> GetNewspaperByIdAsync(string id)
        {
            var result = await _context.Newspapers.FirstOrDefaultAsync(n => n.Id.ToString() == id);
            if (result == null)
                throw new ArgumentException("Newspaper not found");
            var baseURL = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            return new NewsPaperDTO
            {
                Id = result.Id,
                NewspaperName = result.NewspaperName,
                NewspaperLink = result.NewspaperLink,
                CountryId = result.CountryId,
                NewspaperInfo = result.NewspaperInfo,
                Image = $"{baseURL}/uploads/{result.Image}"
            };
        }

        public async Task<List<NewsPaperDTO>> GetAllNewspaperByCountryName(string s)
        {
            var result = await _context.Countries.FirstOrDefaultAsync(c => c.Name.ToLower() == s.ToLower());
            if (result == null)
            {
                throw new ArgumentException("not found");
            }
            var baseURL = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var nes = await _context.Newspapers
                .Where(n => n.CountryId == result.Id && n.Status == "Accepted")
                .Select(n => new NewsPaperDTO
                {
                    Id = n.Id,
                    NewspaperName = n.NewspaperName,
                    NewspaperLink = n.NewspaperLink,
                    CountryId = n.CountryId,
                    NewspaperInfo = n.NewspaperInfo,
                    Image = $"{baseURL}/uploads/{n.Image}"
                })
                .ToListAsync();
            return nes
         .GroupBy(n => n.NewspaperName.ToLower())
         .Select(g => g.First())
         .ToList();
        }

        public async Task<List<CountryDTO>> GetAllCountryByContinentalName(string continent)
        {
            var baseURL = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var result = await _context.Countries
                .Where(c => c.Continent.ToLower() == continent.ToLower())
                .Distinct()
                .Select(n => new CountryDTO
                {
                    Id = n.Id,
                    Continent = n.Continent,
                    Name = n.Name,
                    Description = n.Description,
                    Flag = $"{baseURL}/flags/{n.Flag}"
                }).ToListAsync();
            if (result == null || result.Count == 0)
                return new List<CountryDTO>(); 

            return result;
        }
    }
}
