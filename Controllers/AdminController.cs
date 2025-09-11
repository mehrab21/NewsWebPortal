using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsWebPortal.Models;
using NewsWebPortal.Models.Domain;
using NewsWebPortal.Models.DTO;
using NewsWebPortal.Services;
using System.Diagnostics;

namespace NewsWebPortal.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
       
        private readonly INewspaperService _newspaperService;
        public AdminController(INewspaperService newspaperService)
        {
            _newspaperService = newspaperService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Countries = new SelectList(await _newspaperService.GetAllCountry(), "Id", "Name");
            var resul = await _newspaperService.GetAllNewspapersAsync();
            return View(resul);
        }
        [HttpGet]
        public async Task<IActionResult> fetchCountry()
        {
            var result = await _newspaperService.GetAllCountry();

            return View(result);
        }
        public async Task<IActionResult> AdNewsPaper()
        {
            var result = await _newspaperService.GetAllCountry();
            ViewBag.Countries = new SelectList(result, "Id", "Name");
            ViewBag.Sta = new SelectList(
              new List<string>
              {
                    "Accepted",
                    "Pending",
                    "Rejected",
              });
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AdNewsPaper([FromForm] CreateRequestDTO createRequestDTO)

        {
            var result = await _newspaperService.CreateNewspaperRequestsAsync(createRequestDTO);
            return RedirectToAction("index");
        }
        public async Task<IActionResult> EditNewsPaper(Guid id)
        {
            var country = await _newspaperService.GetAllCountry();
            ViewBag.Countries = new SelectList(country, "Id", "Name");
            ViewBag.Sta = new SelectList(
              new List<string>
              {
                    "Accepted",
                    "Pending",
                    "Rejected",
              });
            var result = await _newspaperService.GetNewspaperByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditNewsPaper([FromForm] CreateRequestDTO createRequest, Guid id)
        {
            var result = await _newspaperService.EditNewspaperAsync(createRequest, id);
            return RedirectToAction("Index");
        }


        public IActionResult AdNewCountry()
        {
            ViewBag.Continents = new SelectList(
                new List<string>
                {   "Asia",
                    "Europe",
                    "Africa",
                    "North America",
                    "South America",
                    "Oceania",
                });
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AdNewCountry([FromForm] CreateCountryDTO createCountryDTO)
        {
            var result = await _newspaperService.CreateCountryAsync(createCountryDTO);
            return RedirectToAction("fetchCountry");
        }
        public IActionResult RequestNewNewspaper()
        {
            return View();
        }
        public async Task<IActionResult> PendingList()
        {
            ViewBag.Countries = new SelectList(await _newspaperService.GetAllCountry(), "Id", "Name");
            var result = await _newspaperService.GetNewspaperByStatus();
            return View(result);
        }
        public async Task<IActionResult> Accept(Guid id)
        {
            var result = await _newspaperService.AcceptForPublic(id);
            return RedirectToAction("PendingList");
        }
        public async Task<IActionResult> Reject(Guid id)
        {
            var result = await _newspaperService.DeleteForPublicAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            var result = await _newspaperService.DeleteCountryAsync(id);
            return RedirectToAction("fetchCountry");
        }
        public async Task<IActionResult> EditCountry(Guid id)
        {
            var country = await _newspaperService.GetCountryByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            ViewBag.Continents = new SelectList(
                new List<string>
                {   "Asia",
                    "Europe",
                    "Africa",
                    "North America",
                    "South America",
                    "Oceania",
                }, country.Continent);
            return View(country);
        }
        [HttpPost]
        public async Task<IActionResult> EditCountry([FromForm] CreateCountryDTO createCountryDTO, Guid id)
        {
            var result = await _newspaperService.EditCountryAsync(createCountryDTO, id);
            return RedirectToAction("fetchCountry");
        }


    }
}
