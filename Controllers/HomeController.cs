using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsWebPortal.Models.Domain;
using NewsWebPortal.Models.DTO;
using NewsWebPortal.Services;

namespace NewsWebPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IGeoLocationService _geoService;
        public HomeController(IUserService userService, IGeoLocationService geoService)
        {
            _userService = userService;
            _geoService = geoService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "8.8.8.8";
            
            if (ip == "::1")
            {
                //ip = "103.100.234.28"; bangladesh ip for check
                //ip = "8.8.8.8"; //us ip for check
                ip = "27.104.0.1"; // singapure ip for check
            }

            string? country = await _geoService.GetCountryByIPAsync(ip);
            string Countryname = country ?? "Unknown";
            ViewBag.CountryIp = Countryname;
            var newspapers = new List<NewsPaperDTO>();
            if (!string.IsNullOrEmpty(country))
            {
                newspapers = await _userService.GetAllNewspaperByCountryName(country);
            }


            if (newspapers.Count == 0)
            {
                ViewBag.Message = $"No newspapers found for your country: {country}";
            }
            var countries = await _userService.GetAllCountry();
            ViewBag.country = new SelectList(countries, "Id", "Name");

            return View(newspapers);

        }

        public async Task<IActionResult> AddNewMedia()
        {
            var countries = await _userService.GetAllCountry();
            ViewBag.country = new SelectList(countries, "Id", "Name");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewMedia([FromForm] CreateRequestDTO createRequestDTO)
        {
            var result = await _userService.RequestFromUserNewspaperAsync(createRequestDTO);
            return RedirectToAction("index");
        }
        [HttpGet]
        public async Task<IActionResult> Info(Guid id)
        {
            string newid = id.ToString();
            var result = await _userService.GetNewspaperByIdAsync(newid);
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> CountrySort(string q)
        {
            ViewBag.CountryName = q;
            if (string.IsNullOrEmpty(q))
            {
                ViewBag.Message = "Invalid request. Country is missing.";
                return View(new List<NewsPaperDTO>()); // return empty list
            }

            var countries = await _userService.GetAllCountry();
            ViewBag.country = new SelectList(countries, "Id", "Name");

            var result = await _userService.GetAllNewspaperByCountryName(q);
            if (result.Count == 0)
            {
                ViewBag.Message = $"No Newspaper found for continent '{q}'";
            }
           
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> ContinentalSort(string continent)
        {

            var result = await _userService.GetAllCountryByContinentalName(continent);
            if (result.Count == 0)
            {
                ViewBag.Message = $"No countries found for continent '{continent}'";
            }
            return View(result);
        }
        public async Task<IActionResult> Search(string q)
        {
            var newspaperResults = await _userService.SearchNewspapersAsync(q);
            var countryResults = await _userService.SearchCountryAsync(q);


            var combinedResults = newspaperResults.Select(n => new
            {
                Type = "Newspaper",
                Id = n.Id,
                Name = n.NewspaperName,
                Link = n.NewspaperLink
            })
            .Union(countryResults.Select(c => new
            {
                Type = "Country",
                Id = c.Id,
                Name = c.Name,
                Link = ""
            }))
            .ToList();

            if (!combinedResults.Any())
            {
                ViewBag.Message = $"No newspapers or countries found for '{q}'";
            }

            return View(combinedResults);
        }


    }
}
