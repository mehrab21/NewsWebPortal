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
                ip = "8.8.8.8"; // IPv4 loopback
            }
            // Step 2: Get user country
            string? country = await _geoService.GetCountryByIPAsync(ip);
            ViewBag.CountryIp = country;
            // Step 3: Get newspapers for that country
            var newspapers = new List<NewsPaperDTO>();
            if (!string.IsNullOrEmpty(country))
            {
                newspapers = await _userService.GetAllNewspaperByCountryName(country);
            }

            // Optional: Show message if no newspapers
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
            ViewBag.country = new SelectList(countries,"Id", "Name");
           
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
        public async Task<IActionResult> BangladeshCountrySort(string q)
        {
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
        public async Task<IActionResult> ContinentalSort(string q)
        {
         
            var result = await _userService.GetAllCountryByContinentalName(q);
            if (result.Count == 0)
            {
                ViewBag.Message = $"No countries found for continent '{q}'";
            }
            return View(result);
        }
    }
}
