using Microsoft.AspNetCore.Mvc;
using WebApplication4.Data;
using WebApplication4.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication4.Controllers
{
    [Authorize]
    public class FlatController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public FlatController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IActionResult Index(int? price)
        {
            string key = "flats_" + price;

            if (!_cache.TryGetValue(key, out List<Flat> flats))
            {
                flats = _context.Flats.ToList();

                if (price != null)
                {
                    flats = flats
                        .Where(x => x.Price <= price)
                        .ToList();
                }

                _cache.Set(key, flats, TimeSpan.FromSeconds(60));
            }

            return View(flats);
        }

        public IActionResult Details(int id)
        {
            var flat = _context.Flats.FirstOrDefault(x => x.Id == id);

            if (flat == null)
                return NotFound();

            return View(flat);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Flat flat)
        {
            _context.Flats.Add(flat);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}